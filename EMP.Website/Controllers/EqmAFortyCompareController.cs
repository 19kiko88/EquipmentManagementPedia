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

        public EqmAFortyCompareController(
            IEqmAFortyCompareService service, 
            IWebHostEnvironment env
            )
        {
            _service = service;
            _env = env;
            _fileFolder = Path.Combine($@"{_env.WebRootPath}", @"Content\Temp");
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
        public async Task<Result<List<string>>> ExportPK([FromBody]ReqGetDiffList data)
        {
            var result = new Result<List<string>>() { Success = false };

            if (data.StartDate == null || data.EndDate == null)
            {
                result.Message = "日期欄位必須有值.";
                return result;
            }

            if (!DateTime.TryParse(data.StartDate.ToString(), out var sd) || !DateTime.TryParse(data.EndDate.ToString(), out var ed))
            {
                result.Message = "日期欄位格式錯誤.";
                return result;
            }

            var limitEndDate = data.StartDate.AddDays(30);
            if (data.EndDate > limitEndDate)
            {
                result.Message = "日期查詢區間不能超過一個月.";
                return result;
            }

            try
            {
                //Read Excel
                var A40Stock = "";

                /*取得EQP庫存表
                 *1.指定A40工號 => 未歸還數量 + (可借用庫存[idle] + Fail庫存[junk]))
                 *2.不指定A40工號 => 未歸還數量
                 */
                var EqpStock = _service.GetEqpStock(data.StartDate, data.EndDate, data.PNs).Result;

                //PK


                result.Content = EqpStock.Select(c => c.PartNumber).Distinct().ToList();
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
