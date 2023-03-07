using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using EMP.Service.Dtos;

namespace EMP.Service.Interfaces
{
    public interface IExcelService
    {
        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭不變動，可以與DB欄位直接對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public Task<DataTable> ReadExcel(string filePath, int? lastCell, int headerRow = 1);

        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭由Model重新取得，才能與DB欄位對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public Task<DataTable> ReadExcel<T>(string filePath, int? lastCell, int headerRow = 1);

        /// <summary>
        /// 匯出PK報表
        /// </summary>
        /// <param name="data">報表內容</param>
        /// <param name="filePath">廠商提供報表的路徑，要把PK結果加到Sheet</param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportExcel(List<StockInfo> data);
    }
}
