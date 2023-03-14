using EMP.Service.Dtos;
using EMP.Service.Implements;
using EMP.Service.Interfaces;
using EMP.Website.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversalLibrary.Models;

namespace EMP.Website.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EqmAFortyCompareController : ControllerBase
    {
        private readonly string _fileFolder;
        private IWebHostEnvironment _env;
        private IEqmAFortyCompareService _service;
        private IExcelService _excelService;
        private readonly IConfiguration _config;

        public EqmAFortyCompareController(
            IEqmAFortyCompareService service,
            IExcelService excelService,
            IWebHostEnvironment env,
            IConfiguration config
            )
        {
            _service = service;
            _excelService = excelService;
            _env = env;
            _fileFolder = Path.Combine($@"{_env.WebRootPath}", @"Content\Upload");
            _env = env;
            _config = config;
        }

        /// <summary>
        /// 檔案上傳
        /// </summary>
        /// <param name="postFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<string>> Upload([FromForm]IFormFile postFile)
        {
            var result = new Result<string>() { Success = true };
            result.Success = false;

            try
            {
                if (postFile != null)
                {
                    //Create a Folder.
                    if (!Directory.Exists(_fileFolder))
                    {
                        Directory.CreateDirectory(_fileFolder);
                    }

                    //Save the uploaded Excel file.
                    string fileName = Path.GetFileName(postFile.FileName);

                    string newFileName = $"{DateTime.Now:yyyyMMddhhmmss}_{fileName}";
                    string fullFilePath = Path.Combine(_fileFolder, newFileName);
                    using (FileStream stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        postFile.CopyTo(stream);
                    }

                    result.Content = fullFilePath;
                }
                else
                {
                    result.Message = "請選擇上傳檔案.";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        [HttpPost]
        public async Task<Result<string>> DataCheck([FromBody] ReqGetDiffList data)
        {
            var result = new Result<string>() { Success = true };

            if (data.StartDate == null || data.EndDate == null)
            {
                result.Content = "日期欄位必須有值.";
                return result;
            }

            if (!DateTime.TryParse(data.StartDate.ToString(), out var sd) || !DateTime.TryParse(data.EndDate.ToString(), out var ed))
            {
                result.Content = "日期欄位格式錯誤.";
                return result;
            }

            if (DateTime.Compare(data.StartDate, data.EndDate) == 1)
            {
                result.Content = "起始日期必須小於結束日期.";
                return result;
            }

            var limitEndDate = data.StartDate.AddDays(30);
            if (data.EndDate > limitEndDate)
            {
                result.Content = "日期查詢區間不能超過一個月.";
                return result;
            }

            if (string.IsNullOrEmpty(data.FilePath))
            {
                result.Content = "請先上傳A40檔案.";
                return result;
            }

            try
            {             
                result.Content = "";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        [HttpPost]
        public async Task<FileStreamResult> ExportPK([FromBody]ReqGetDiffList data)
        {
            FileStreamResult fs;
            var pkResult = new List<StockInfo>();
            var A40Stock = new List<StockInfo>();
            try
            {
                /*取得EQP庫存表
                 *1.指定A40工號 => 未歸還數量 + (可借用庫存[idle] + Fail庫存[junk]))
                 *2.不指定A40工號 => 未歸還數量
                 */
                var EmployeeIdMapping = _config.GetSection("EmployeeIdMapping").Get<List<EmployeeIdMapping>>();
                var EqpStock = _service.GetEqpStock(data.StartDate, data.EndDate, EmployeeIdMapping, data.PNs).Result;

                if (EqpStock != null && EqpStock.Count > 0)
                {
                    //讀取A40 Excel，取得A40庫存表
                    var dataA40 = await _excelService.ReadExcel(data.FilePath, null, 3);
                    if (dataA40 != null && dataA40.Rows.Count > 0)
                    {
                        A40Stock = _service.GetA40Stock(dataA40).Result;
                    }

                    //PK(EqpStock left join A40Stock)
                    pkResult = await _service.GetStockResult(EqpStock, A40Stock);
                }

                fs = await _excelService.ExportExcel(pkResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return fs;
        }

        /// <summary>
        /// (測試API用)從EQM取得異動PN清單
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<List<DiffPnInfo>>> GetDiffDatas([FromBody] ReqGetDiffList data)
        {
            var result = new Result<List<DiffPnInfo>>() { Success = true };

            try
            {
                var listPNs = new List<string>();
                if (!string.IsNullOrEmpty(data.PNs))
                {
                    listPNs = data.PNs.IndexOf(";") > 0 ? data.PNs.Split(';').ToList() : new List<string> { data.PNs };
                }

                var res = _service.GetDiffPartNo(DateOnly.FromDateTime(data.StartDate), DateOnly.FromDateTime(data.EndDate), listPNs).Result;
                result.Content = res;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// (測試API用)取得EQP Lend資料
        /// </summary>
        /// <param name="PN"></param>
        /// <returns></returns>
        [HttpGet("{PN}")]
        public async Task<Result<List<EQPLendInfo>>> GetILendData(string PN)
        {
            var result = new Result<List<EQPLendInfo>>() { Success = true };

            try
            {
                var res = _service.FetchEqpLendInfo(new List<string>() { PN });
                result.Content = res;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// (測試API用)取得EQP Idle資料
        /// </summary>
        /// <param name="PN"></param>
        /// <returns></returns>
        [HttpGet("{PN}")]
        public async Task<Result<List<EQPIdleInfo>>> GetIdleData(string PN)
        {
            var result = new Result<List<EQPIdleInfo>>() { Success = true };

            try
            {
                var res = _service.FetchEqpIdleInfo(new List<string>() { PN });
                result.Content = res;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}