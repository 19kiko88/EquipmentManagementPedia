using EMP.Service.Implements;
using EMP.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversalLibrary.Models;
using CAEService;

namespace EMP.Website.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class CommonController : ControllerBase
    {
        private IWCFService _wcfService;

        public CommonController(IWCFService wcfService) 
        { 
            _wcfService = wcfService;
        }

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
                var employee = _wcfService.GetEmployeeInfo(User.Identity.Name.Split('\\')[1]);
                result.Content = $"{employee.NameCT}({employee.Name})";
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
