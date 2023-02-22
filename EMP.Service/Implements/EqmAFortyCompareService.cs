using EMP.Service.Interfaces;
using EMP.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Service.Implements
{
    public class EqmAFortyCompareService: IEqmAFortyCompareService
    {
        /// <summary>
        /// 取得EQM與A40有差異的PN資廖
        /// REF：
        /// 1.呼叫外部API：https://dotblogs.com.tw/lapland/2015/08/06/153065
        /// </summary>
        public async Task<List<DiffPnInfo>> GetDiffPartNo(DateOnly startDate, DateOnly endDate, List<string>? pns = null)
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

                    token = JsonConvert.DeserializeObject<APIResult<String>>(resString)?.Content;
                    var paras = new DiffPNQueryParams() { dailyToken = token, start_Date = startDate.ToString(), end_Date = endDate.ToString(), pn = pns };
                    var response2 = await client.PostAsJsonAsync("https://tp-caevm-vt02.corpnet.asus/OsvcMiddle/api/eqm/QtyStockData", paras);
                    //處理呼叫結果-Response2
                    if (response2.IsSuccessStatusCode)
                    {
                        var resString2 = await response2.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<APIResult<List<DiffPnInfo>>>(resString2)?.Content;
                    }
                }
            }

            return res;
        }
    }
}
