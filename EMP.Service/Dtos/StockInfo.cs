using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Service.Dtos
{
    public class StockInfo
    {
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public string EmployeeID { get; set; }
        public int EqpQty { get; set; }
        public int A40Qty { get; set; }
    }
}
