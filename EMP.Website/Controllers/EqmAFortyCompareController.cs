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

        public EqmAFortyCompareController(
            IEqmAFortyCompareService service,
            IExcelService excelService,
            IWebHostEnvironment env
            )
        {
            _service = service;
            _excelService = excelService;
            _env = env;
            _fileFolder = Path.Combine($@"{_env.WebRootPath}", @"Content\Upload");
            _env = env;
        }

        /// <summary>
        /// 檔案上傳
        /// </summary>
        /// <param name="postFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<string>> Upload([FromForm]IFormFile postFile)
        {
            var result = new Result<string>() { Success = false };
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
                    result.Success = true;
                }
                else
                {
                    result.Message = "請選擇上傳檔案.";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        [HttpPost]
        public async Task<Result<string>> DataCheck([FromBody] ReqGetDiffList data)
        {
            var result = new Result<string>() { Success = false };

            if (string.IsNullOrEmpty(data.FilePath))
            {
                result.Content = "請先上傳A40檔案.";
                return result;
            }

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

            var limitEndDate = data.StartDate.AddDays(30);
            if (data.EndDate > limitEndDate)
            {
                result.Content = "日期查詢區間不能超過一個月.";
                return result;
            }

            try
            {             
                result.Content = "";
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        [HttpPost]
        public async Task<FileStreamResult> ExportPK([FromBody]ReqGetDiffList data)
        {
            FileStreamResult fs;

            try
            {
                //讀取A40 Excel，取得A40庫存表
                var A40Stock = new List<StockInfo>();
                var dataA40 = await _excelService.ReadExcel(data.FilePath, null, 3);
                if (dataA40 != null && dataA40.Rows.Count > 0) 
                {
                    A40Stock = _service.GetA40Stock(dataA40).Result;
                }

                /*取得EQP庫存表
                 *1.指定A40工號 => 未歸還數量 + (可借用庫存[idle] + Fail庫存[junk]))
                 *2.不指定A40工號 => 未歸還數量
                 */
                var EqpStock = _service.GetEqpStock(data.StartDate, data.EndDate, data.PNs).Result;

                //PK(EqpStock left join A40Stock)
                var pkResult = EqpStock
                    .GroupJoin(A40Stock, o => new { o.PartNumber, o.EmployeeID }, o => new { o.PartNumber, o.EmployeeID }, (o, c) => new { o.PartNumber, o.Description, o.EmployeeID, o.EqpQty, c })
                    .SelectMany(o => o.c.DefaultIfEmpty(), (o, c) => new StockInfo
                    {
                        PartNumber = o.PartNumber,
                        Description = o.Description,
                        EmployeeID = o.EmployeeID,
                        EqpQty = o.EqpQty,
                        A40Qty = c == null || !Int32.TryParse(c.A40Qty.ToString(), out var qty) ? -1 : c.A40Qty//.ToString()
                    }).ToList();

                fs = await _excelService.ExportExcel(pkResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return fs;
        }

        /// <summary>
        /// 從EQM取得異動PN清單
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<List<DiffPnInfo>>> GetDiffDatas([FromBody] ReqGetDiffList data)
        {
            var result = new Result<List<DiffPnInfo>>() { Success = false };

            try
            {
                var listPNs = new List<string>();
                if (!string.IsNullOrEmpty(data.PNs))
                {
                    listPNs = data.PNs.IndexOf(";") > 0 ? data.PNs.Split(';').ToList() : new List<string> { data.PNs };
                }

                var res = _service.GetDiffPartNo(DateOnly.FromDateTime(data.StartDate), DateOnly.FromDateTime(data.EndDate), listPNs).Result;
                result.Content = res;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
