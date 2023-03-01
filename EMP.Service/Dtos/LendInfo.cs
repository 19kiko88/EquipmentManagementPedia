using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Service.Dtos
{
    public class LendInfo
    {
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string EmployeeID { get; set; }
        public int LendQty { get; set; }
    }
}
