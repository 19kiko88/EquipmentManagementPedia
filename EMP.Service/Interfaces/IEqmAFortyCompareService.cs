using EMP.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Service.Interfaces
{
    public interface IEqmAFortyCompareService
    {
        public Task<List<DiffPnInfo>> GetDiffPartNo(DateOnly startDate, DateOnly endDate, List<string>? pns = null);
    }
}
