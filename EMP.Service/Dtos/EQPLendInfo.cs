using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Service.Dtos
{
    public class EQPLendInfo
    {
        public string Location { get; set; }
        public string EQName { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string EmployeeID { get; set; }
        public int Quantity { get; set; }
        public DateTime LendTime { get; set; }
    }
}
