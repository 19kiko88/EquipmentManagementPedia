using EMP.Service.Implements;
using EMP.Service.Interfaces;
using EMP.Service.Models;
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
        private IEqmAFortyCompareService _service;

        public EqmAFortyCompareController(IEqmAFortyCompareService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<Result<List<DiffPnInfo>>> GetDiffList([FromBody]ReqGetDiffList data)
        {
            var result = new Result<List<DiffPnInfo>>() { Success = false };

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
                result.Content = await _service.GetDiffPartNo(DateOnly.FromDateTime(data.StartDate), DateOnly.FromDateTime(data.EndDate), null);
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
