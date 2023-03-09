using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using EMP.Service.Dtos;
using EMP.Service.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EMP.Service.Implements
{
    public class EqmAFortyCompareService: IEqmAFortyCompareService
    {
        IWCFService _wcfService;

        public EqmAFortyCompareService()
        {
            _wcfService = new WCFService("http://cae.corpnet.asus/caeservices/caeservice.svc");
        }

        public EqmAFortyCompareService(IWCFService wcfService)
        {//for unit。傳入Mock IWCFService.
            _wcfService = wcfService;
        }

        /// <summary>
        /// 取得EQM與A40有差異的PN資廖
        /// REF：
        /// 1.呼叫外部API：https://dotblogs.com.tw/lapland/2015/08/06/153065
        /// </summary>
        public async Task<List<DiffPnInfo>> GetDiffPartNo(DateOnly startDate, DateOnly endDate, List<string> pns)
        {
            pns = pns ?? new List<string>();

            var res = new List<DiffPnInfo>();
            var httpClientHandler = new HttpClientHandler()
            {
                PreAuthenticate = true,
                //Credentials = new NetworkCredential("homer_chen", "pw", "ASUS"), // real password instead of "pw"
                UseDefaultCredentials = true,
                AllowAutoRedirect = true
            };
            
            var token = string.Empty;

            using (var client = new HttpClient(httpClientHandler))
            {
                //設定Header - Accept的資料型別
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //設定呼叫網址
                var response = await client.GetAsync("https://tp-caevm-vt02.corpnet.asus/OsvcMiddle/api/eqm/dailytoken");

                //處理呼叫結果-Response
                if (response.IsSuccessStatusCode)
                {
                    var resString = await response.Content.ReadAsStringAsync();

                    token = JsonConvert.DeserializeObject<APIResult<string>>(resString)?.Content;
                    var paras = new DiffPNQueryParams() { dailyToken = token, start_Date = startDate.ToString(), end_Date = endDate.ToString(), pn = pns };
                    var response2 = await client.PostAsJsonAsync("https://tp-caevm-vt02.corpnet.asus/OsvcMiddle/api/eqm/QtyStockData", paras);
                    //處理呼叫結果-Response2
                    if (response2.IsSuccessStatusCode)
                    {
                        var resString2 = await response2.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<APIResult<List<DiffPnInfo>>>(resString2)?.Content;

                        //只取90開頭
                        res = res?.Where(c => c.pn.StartsWith("90")).ToList();
                    }
                }
            }

            return res;
        }

        public List<EQPIdleInfo> FetchEqpIdleInfo(List<string> partnumbers)
        {
            var result = new List<EQPIdleInfo>();

            var ebsCmd = "SELECT LOC, EQ_NAME, PN, SPEC_NOTE, IDLE_QTY, LEND_QTY, JUNK_QTY, CDT " +
                    "FROM EQP.EQP_QTY_NB_IDLE_INFO_V " +
                    $"WHERE PN in ('{string.Join("','", partnumbers.Select(a => a.ToUpper()))}')";

            var xmlResult = _wcfService.GetEBSProDataAsync(ebsCmd);

            var parsedResult = Regex.Matches(xmlResult.Nodes[1].ToString().Replace("\r\n", "").Replace("\r", "").Replace("\n", ""),
                @"<LOC>(?<Loc>[^\<]+)<\/LOC>[\s]{0,}<EQ_NAME>(?<EQ_Name>[^\<]+)<\/EQ_NAME>[\s]{0,}<PN>(?<PN>[^\<]+)<\/PN>[\s]{0,}<SPEC_NOTE>(?<Desc>[^\<]+)<\/SPEC_NOTE>[\s]{0,}<IDLE_QTY>(?<IdleQ>[^\<]+)<\/IDLE_QTY>[\s]{0,}<LEND_QTY>(?<LendQ>[^\<]+)<\/LEND_QTY>[\s]{0,}<JUNK_QTY>(?<JunkQ>[^\<]+)<\/JUNK_QTY>[\s]{0,}<CDT>(?<Time>[^\<]+)<\/CDT>");

            foreach (Match match in parsedResult)
            {
                result.Add(new EQPIdleInfo
                {
                    Location = match.Groups["Loc"].Value,
                    EQName = match.Groups["EQ_Name"].Value,
                    PartNumber = match.Groups["PN"].Value,
                    Description = match.Groups["Desc"].Value,
                    IdleQuantity = Convert.ToInt32(match.Groups["IdleQ"].Value),
                    LendQuantity = Convert.ToInt32(match.Groups["LendQ"].Value),
                    JunkQuantity = Convert.ToInt32(match.Groups["JunkQ"].Value),
                    InStockTime = Convert.ToDateTime(match.Groups["Time"].Value)
                });
            }

            return result;
        }

        public List<EQPLendInfo> FetchEqpLendInfo(List<string> partnumbers)
        {
            var result = new List<EQPLendInfo>();

            var ebsCmd = "SELECT LOC, EQ_NAME, PN, SPEC_NOTE, BORROWER_ID, NQTY, LDT " +
                    "FROM EQP.VW_EQP_QTY_LEND_INFO " +
                    $"WHERE PN in ('{string.Join("','", partnumbers.Select(a => a.ToUpper()))}')";

            var xmlResult = _wcfService.GetEBSProDataAsync(ebsCmd);

            var parsedResult = Regex.Matches(xmlResult.Nodes[1].ToString().Replace("\r\n", "").Replace("\r", "").Replace("\n", ""),
                @"<LOC>(?<Loc>[^\<]+)<\/LOC>[\s]{0,}<EQ_NAME>(?<EQ_Name>[^\<]+)<\/EQ_NAME>[\s]{0,}<PN>(?<PN>[^\<]+)<\/PN>[\s]{0,}<SPEC_NOTE>(?<Desc>[^\<]+)<\/SPEC_NOTE>[\s]{0,}<BORROWER_ID>(?<WorkID>[^\<]+)<\/BORROWER_ID>[\s]{0,}<NQTY>(?<Quantity>[^\<]+)<\/NQTY>[\s]{0,}<LDT>(?<Time>[^\<]+)<\/LDT>");

            foreach (Match match in parsedResult)
            {
                result.Add(new EQPLendInfo
                {
                    Location = match.Groups["Loc"].Value,
                    EQName = match.Groups["EQ_Name"].Value,
                    PartNumber = match.Groups["PN"].Value,
                    Description = match.Groups["Desc"].Value,
                    EmployeeID = match.Groups["WorkID"].Value,
                    Quantity = Convert.ToInt32(match.Groups["Quantity"].Value),
                    LendTime = Convert.ToDateTime(match.Groups["Time"].Value)
                });
            }

            return result;
        }

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
        public async Task<List<StockInfo>> GetEqpStock(DateTime startDate, DateTime endDate, string pns = null)
        {
            var res = new List<StockInfo>();

            //取得有庫存數量有異動紀錄的機台
            var listPNs = new List<string>();
            if (!string.IsNullOrEmpty(pns))
            {
                listPNs = pns.IndexOf(";") > 0 ? pns.Split(';').ToList() : new List<string> { pns };
            }

            var diffPNs = await this.GetDiffPartNo(DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(endDate), listPNs);

            //Distinct
            var distinctDiffPns = diffPNs?.Select(c => c.pn).Distinct().ToList();

            if (distinctDiffPns != null && distinctDiffPns.Count > 0) 
            {
                
                var lendData = this.FetchEqpLendInfo(distinctDiffPns);//EQP借出機台
                var idleData = this.FetchEqpIdleInfo(distinctDiffPns);//EQP未借出機台

                //Oracle 需指定A40掛帳工號 的工號
                var tranToSpecifyWordID = new string[] { "AA2100659", "AA0900217", "AA2100539", "P7145" };
                var specifyWorkID = "P7145";//指定A40掛帳工號

                //未借出機台總數量 by PN
                var idleDataGroup =
                    idleData
                    .Select(c => new { c.PartNumber, IdleQty = c.IdleQuantity + c.JunkQuantity })
                    .GroupBy(g => new { g.PartNumber })
                    .Select(c => new
                    {
                        c.Key.PartNumber,
                        IdleQty = c.Sum(s => s.IdleQty )
                    }).ToList();

                //借出機台總數量 by PN + 人
                var lendDataGroup =
                    lendData
                    .Select(c => new { c.PartNumber, c.Description, EmployeeID = tranToSpecifyWordID.Contains(c.EmployeeID) ? specifyWorkID : c.EmployeeID, Qty = c.Quantity})
                    .GroupBy(g => new { g.PartNumber, g.Description, g.EmployeeID })
                    .Select(c => new StockInfo
                    {
                        PartNumber = c.Key.PartNumber,
                        Description = c.Key.Description,
                        EmployeeID = c.Key.EmployeeID,
                        EqpQty = c.Sum(s => s.Qty)
                    }).ToList();

                //指定工號必須再加上未借出的機台數量
                foreach (var lends in lendDataGroup)
                {
                    var idles = idleDataGroup.Where(c => c.PartNumber == lends.PartNumber);

                    if (lends.EmployeeID == specifyWorkID && idles.Any())
                    {
                        lends.EqpQty += idles.FirstOrDefault().IdleQty;
                    }
                }

                res = lendDataGroup;
            }

            return res;
        }

        /// <summary>
        /// A40 Excel內容轉List
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public async Task<List<StockInfo>> GetA40Stock(System.Data.DataTable dt)
        {
            var res = new List<StockInfo>();

            foreach (var dr in dt.AsEnumerable())
            {
                Int32.TryParse(dr["Qty"].ToString(), out var qty);

                var drData = new StockInfo()
                {
                    PartNumber = dr["Item"].ToString(),
                    EmployeeID = dr["Locator"].ToString(),
                    A40Qty = qty
                };

                res.Add(drData);
            }

            return res;
        }

        /// <summary>
        /// PK(EqpStock left join A40Stock)
        /// </summary>
        /// <param name="eqpStock"></param>
        /// <param name="a40Stock"></param>
        /// <returns></returns>
        public async Task<List<StockInfo>> GetStockResult(List<StockInfo> eqpStock, List<StockInfo> a40Stock)
        {
            var res = new List<StockInfo>();

            res = eqpStock
                .GroupJoin(a40Stock, o => new { o.PartNumber, o.EmployeeID }, o => new { o.PartNumber, o.EmployeeID }, (o, c) => new { o.PartNumber, o.Description, o.EmployeeID, o.EqpQty, c })
                .SelectMany(o => o.c.DefaultIfEmpty(), (o, c) => new StockInfo
                {
                    PartNumber = o.PartNumber,
                    Description = o.Description,
                    EmployeeID = o.EmployeeID,
                    EqpQty = o.EqpQty,
                    A40Qty = c == null || !Int32.TryParse(c.A40Qty.ToString(), out var qty) ? -1 : c.A40Qty//.ToString()
                }).ToList();

            return res;
        }
    }
}
