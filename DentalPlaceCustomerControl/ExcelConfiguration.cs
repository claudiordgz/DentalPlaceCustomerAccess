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
                    int row = 1;
                    int end = range.Columns.Count + 1;
                    int vigenciaIdx = 0;
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
                        for (int col = 0; col != colIdx.Count; ++col) {
                            if(colIdx[col] != vigenciaIdx) {
                                string value = (range.Cells[row, colIdx[col]] as Excel.Range).Value2.ToString();
                                dr[col] = value;
                            } else {
                                double d = double.Parse((range.Cells[row, colIdx[col]] as Excel.Range).Value2.ToString());
                                DateTime conv = DateTime.FromOADate(d);
                                dr[col] = conv;
                            }
                        }
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
    }
}
