using CAEService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Service.Interfaces;
using EMP.Service.Dtos;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using UniversalLibrary.WCF.CAEService;

namespace EMP.Service.Implements
{
    public class WCFService: IWCFService
    {
        private readonly OraServiceClient _srvClient;

        public WCFService(string config)
        {
            _srvClient = new OraServiceClient(OraServiceClient.EndpointConfiguration.BasicHttpBinding_IOraService, config);
        }

        public ArrayOfXElement GetEBSProDataAsync(string ebsCmd)
        { 
            return _srvClient.GetEBSProDataAsync(ebsCmd).Result.GetEBSProDataResult;
        }

        public Employee GetEmployeeInfo(string username)
        {
            var emps = _srvClient.GetEIPEmployeeDataAsync("Name", username, false, true).Result.GetEIPEmployeeDataResult;
            if (emps.IsMatch && emps.Employees.Any(a => a.Quit == "N"))
            {
                return emps.Employees.Where(a => a.Quit == "N").First();
            }
            return null;
        }
    }
}
