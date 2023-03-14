using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace EMP.Service.helper
{
    internal class ExcelHelper
    {
        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭不變動，可以與DB欄位直接對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public static Task<DataTable> ReadExcel(string filePath, int? lastCell, int headerRow = 1)
        {
            return ReadExcel(filePath, null, lastCell, headerRow);
        }

        /// <summary>
        /// 讀取Excel內容轉DataTable(DT表頭由Model重新取得，才能與DB欄位對應)
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public static Task<DataTable> ReadExcel<T>(string filePath, int? lastCell, int headerRow = 1)
        {
            var obj = Activator.CreateInstance<T>();
            return ReadExcel(filePath, obj, lastCell, headerRow);
        }

        /// <summary>
        /// Ref：[Read Excel worksheet into DataTable using ClosedXML]
        /// https://stackoverflow.com/questions/48756449/read-excel-worksheet-into-datatable-using-closedxml
        /// 讀取Excel內容轉DataTable
        /// </summary>
        /// <param name="filePath">excel檔路徑</param>
        /// <param name="obj">新header對應物件</param>
        /// <param name="headerRow">表頭所在列數</param>
        /// <returns></returns>
        public async static Task<DataTable> ReadExcel(string filePath, object? obj, int? lastCell, int headerRow = 1)
        {
            //Create a new DataTable.
            DataTable dt = new DataTable();

            using (XLWorkbook workBook = new XLWorkbook(filePath))
            {
                //Read the first Sheet from Excel file.
                IXLWorksheet workSheet = workBook.Worksheet(1);

                //Loop through the Worksheet rows.
                bool columnNameSetDone = false;

                var sn = 1;
                foreach (IXLRow row in workSheet.Rows())
                {
                    //Use the first row to add columns to DataTable.
                    if (sn == headerRow)
                    {
                        if (obj == null)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Columns.Add(cell.Value.ToString()?.Replace("\r\n", " "));
                            }
                        }
                        else
                        {
                            foreach (var item in obj.GetType().GetProperties())
                            {
                                var col = obj.GetType().GetProperty(item.Name);//get class property name
                                //var columnName = obj.GetType().GetProperty(item.Name).GetValue(obj);
                                //var type = obj.GetType().GetProperty(item.Name).PropertyType;
                                dt.Columns.Add(col.GetValue(obj).ToString(), col.PropertyType);
                            }
                        }

                        columnNameSetDone = true;
                    }
                    else if (columnNameSetDone)
                    {
                        int i = 0;

                        if (!row.IsEmpty())
                        {//Add rows to DataTable.
                            dt.Rows.Add();
                            var endCell = lastCell.HasValue ? lastCell.Value : row.LastCellUsed().Address.ColumnNumber;
                            foreach (IXLCell cell in row.Cells(1/*row.FirstCellUsed().Address.ColumnNumber*/, endCell))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value;//.ToString();
                                i++;
                            }
                        }
                    }

                    sn++;
                }
            }

            return dt;
        }

        /// <summary>
        /// 設定Excell Cell Style
        /// </summary>
        /// <param name="ws"></param>
        public static void SettingCellStyle(IXLWorksheet ws)
        {
            var numberOfLastRow = ws.LastRowUsed().RowNumber();

            for (int row = 1; row <= numberOfLastRow; row++)
            {
                var cellEQM = ws.Cell(row, 4);
                var cellA40 = ws.Cell(row, 5);

                if (cellA40.Value.ToString() == "-1")
                {
                    cellA40.Value = "N/A";
                }

                if (cellEQM.Value.ToString() == cellA40.Value.ToString())
                {
                    cellEQM.Style.Font.FontColor = XLColor.BlueGray;
                    cellA40.Style.Font.FontColor = XLColor.BlueGray;
                }
                else
                {
                    cellEQM.Style.Font.FontColor = XLColor.Red;
                    cellA40.Style.Font.FontColor = XLColor.Red;
                    cellEQM.Style.Font.Bold = true;
                    cellA40.Style.Font.Bold = true;
                }
            }
        }
    }
}
