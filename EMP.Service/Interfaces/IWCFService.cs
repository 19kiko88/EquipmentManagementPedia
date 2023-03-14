using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAEService;
using EMP.Service.Dtos;

namespace EMP.Service.Interfaces
{
    public interface IWCFService
    {
        public ArrayOfXElement GetEBSProDataAsync(string ebsCmd);
        Employee GetEmployeeInfo(string username);
    }
}
