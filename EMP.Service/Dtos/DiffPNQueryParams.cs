using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Service.Dtos
{
    public class DiffPNQueryParams
    {
        public string dailyToken { get; set; }
        public string start_Date { get; set; }
        public string end_Date { get; set; }
        public List<string> pn { get; set; }
    }
}
