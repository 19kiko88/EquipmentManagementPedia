using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversalLibrary.Models;

namespace EMP.Website.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class CommonController : ControllerBase
    {
        /// <summary>
        /// 取得UserName
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<Result<string>> GetUserName()
        {
            var result = new Result<string>() { Success = false };
            try
            {
                result.Content = User.Identity.Name.Split('\\')[1];
                result.Success = true;
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
