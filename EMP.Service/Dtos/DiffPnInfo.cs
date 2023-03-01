using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Service.Dtos
{
    public class DiffPnInfo
    {
        public string pn { get; set; }
        public string stock_Type { get; set; }
        public int qty { get; set; }
        public string memo { get; set; }
        public DateTime create_Time { get; set; }
        public string creator { get; set; }

    }
}
