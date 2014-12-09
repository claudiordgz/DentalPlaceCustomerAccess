using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace DentalPlaceAccessControl
{

    public class ExcelConfiguration
    {
        public string configurationFile;
        private string _membershipDateColName;

        public ExcelConfiguration(string membershipDateColName)
        {
            _membershipDateColName = membershipDateColName;
        }

        public DataTable Data
        {
            get
            {
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook;
                Excel.Worksheet worksheet;
                Excel.Range range;
                workbook = excelApp.Workbooks.Open(configurationFile);
                worksheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);
                range = worksheet.UsedRange;
                DataTable dt = new DataTable();
                if (range != null) {
                    List<int> colIdx = new List<int>();
                    int row = 1, 
                        end = range.Columns.Count + 1, 
                        vigenciaIdx = 0;
                    for (int column=1; column != end; ++column) {
                        string value = (range.Cells[row, column] as Excel.Range).Value2 as string;
                        if (value != null) {
                            if (value.Length != 0) {
                                dt.Columns.Add(value);
                                colIdx.Add(column);
                                if (value == _membershipDateColName)
                                {
                                    vigenciaIdx = column;
                                }
                            }
                        }
                    }
                    end = range.Rows.Count + 1;
                    for (row = 2; row != end; ++row) {
                        DataRow dr = dt.NewRow();
                        bool skip = false;
                        for (int col = 0; col != colIdx.Count; ++col) {
                            if(colIdx[col] != vigenciaIdx) {
                                string value = GetValue(range, row, colIdx[col]);
                                if (value != null)
                                {
                                    dr[col] = value;
                                }
                                else
                                {
                                    skip = true;
                                    break;
                                }
                            } else {
                                string value = GetValue(range, row, colIdx[col]);
                                if (value != null)
                                {
                                    double d = double.Parse(value);
                                    DateTime conv = DateTime.FromOADate(d);
                                    dr[col] = conv;
                                }
                                else
                                {
                                    skip = true;
                                    break;
                                }
                            }
                        }
                        if (skip) continue;
                        dt.Rows.Add(dr);
                        dt.AcceptChanges();
                    }
                }

                workbook.Close(false, Missing.Value, Missing.Value);
                Marshal.ReleaseComObject(workbook);
                excelApp.Quit();
                Marshal.ReleaseComObject(excelApp);
                return dt;
            }
        }

        private string GetValue(Excel.Range range, int row, int col)
        {
            string value;
            try
            {
                value = (range.Cells[row, col] as Excel.Range).Value2.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
            return value;
        }
    }
}
