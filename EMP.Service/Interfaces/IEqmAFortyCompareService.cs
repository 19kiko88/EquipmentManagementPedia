using DocumentFormat.OpenXml.Drawing.Charts;
using EMP.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Service.Interfaces
{
    public interface IEqmAFortyCompareService
    {
        public Task<List<DiffPnInfo>> GetDiffPartNo(DateOnly startDate, DateOnly endDate, List<string> pns);

        public List<EQPIdleInfo> FetchEqpIdleInfo(List<string> partnumbers);

        public List<EQPLendInfo> FetchEqpLendInfo(List<string> partnumbers);

        /// <summary>
        /*取得EQP庫存表
         *1. 未歸還數量 + (可借用庫存[idle] + Fail庫存[junk])) => 指定A40工號
         *2. 未歸還數量 => 不指定A40工號
         */
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pns"></param>
        /// <returns></returns>
        public Task<List<StockInfo>> GetEqpStock(DateTime startDate, DateTime endDate, List<EmployeeIdMapping> idMapping, string pns = null);

        public Task<List<StockInfo>> GetA40Stock(System.Data.DataTable dt);

        /// <summary>
        /// PK(EqpStock left join A40Stock)
        /// </summary>
        /// <param name="eqpStock"></param>
        /// <param name="a40Stock"></param>
        /// <returns></returns>
        public Task<List<StockInfo>> GetStockResult(List<StockInfo> eqpStock, List<StockInfo> a40Stock);
    }
}
