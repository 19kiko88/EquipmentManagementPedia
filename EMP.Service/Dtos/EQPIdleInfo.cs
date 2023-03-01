using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Service.Dtos
{
    public class EQPIdleInfo
    {
        public string Location { get; set; }
        public string EQName { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
        public int IdleQuantity { get; set; }
        public int LendQuantity { get; set; }
        public int JunkQuantity { get; set; }
        public DateTime InStockTime { get; set; }
    }
}
