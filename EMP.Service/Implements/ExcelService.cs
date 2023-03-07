using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using EMP.Service.Dtos;
using EMP.Service.helper;
using EMP.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EMP.Service.Implements
{
    public class ExcelService : IExcelService
    {
        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭不變動，可以與DB欄位直接對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public async Task<DataTable> ReadExcel(string filePath, int? lastCell, int headerRow = 1)
        {
            var dt = new DataTable();
            if (!Directory.Exists(filePath))
            {
                dt = await ExcelHelper.ReadExcel(filePath, lastCell, headerRow);
            }
            return dt;
        }

        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭由Model重新取得，才能與DB欄位對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public async Task<DataTable> ReadExcel<T>(string filePath, int? lastCell, int headerRow = 1)
        {
            var dt = new DataTable();
            if (!Directory.Exists(filePath))
            {
                dt = await ExcelHelper.ReadExcel<T>(filePath, lastCell, headerRow);
            }
            return dt;
        }

        /// <summary>
        /// PK報表。Excel匯出
        /// </summary>
        /// <param name="data">報表內容</param>
        /// <param name="filePath">廠商提供報表的路徑，要把PK結果加到Sheet</param>
        /// <returns></returns>
        public async Task<FileStreamResult> ExportExcel(List<StockInfo> data)
        {
            XLWorkbook wb = new XLWorkbook(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"Content\PK_Report_Template.xlsx"));
            IXLWorksheet wsClosed = wb.Worksheet("Result");
            int NumberOfLastRow = 1;// Worksheet.LastRowUsed().RowNumber();

            //Append List_Closed Data
            IXLCell CellForNewData_Closed = wsClosed.Cell(NumberOfLastRow + 1, 1);
            CellForNewData_Closed.InsertData(data);
            //Set Style
            ExcelHelper.SettingCellStyle(wsClosed);

            //輸出Excel報表
            var filePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), @$"Content\Download\pk_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            wb.SaveAs(filePath);

            var ms = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(ms);
            }
            ms.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
