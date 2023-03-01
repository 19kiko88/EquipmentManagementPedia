using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Service.Dtos
{
    public class APIResult<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Content { get; set; }
    }
}
