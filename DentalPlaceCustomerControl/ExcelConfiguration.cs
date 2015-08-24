using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Mantin.Controls.Wpf.Notification;
using System.Windows.Media;


namespace DentalPlaceAccessControl
{
    

    public static class ExcelUtilities
    {
        public static void DisplayError(string errorMessage, string customerName)
        {
            new ToastPopUp(errorMessage, customerName, NotificationType.Error)
            {
                Background = new SolidColorBrush(Color.FromArgb(255, 68, 68, 68)),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 68, 68, 68)),
                FontColor = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))
            }.Show();
        }

        public static object GetDate(string personName, string possibleDate) 
        {
            DateTime? conv = null;
            if (possibleDate == null) return conv;
            try
            {
                double d = double.Parse(possibleDate);
                conv = DateTime.FromOADate(d);
            }
            catch (System.FormatException)
            {
                DisplayError("Fecha Incorrecta, no se utilizará la fila completa de su tabla", personName);
            }
            return conv;
        }
    }


    public class ExcelConfiguration
    {
        public string configurationFile;
        private string _membershipDateColName;
        private string _customerNameColName;

        public ExcelConfiguration(string customerNameColName, string membershipDateColName)
        {
            _membershipDateColName = membershipDateColName;
            _customerNameColName = customerNameColName;
        }

        public class ConfigurationControl
        {
            public List<int> colIdx;
            public int membershipColumnIndex;
            public int patientNameColumnIndex;

            public ConfigurationControl()
            {
                colIdx = new List<int>();
            }
        }

        private ConfigurationControl GetHeaderNames(DataTable userInformationTable, Excel.Range range)
        {
            ConfigurationControl config = new ConfigurationControl();
            int row = 1,
                end = range.Columns.Count + 1;
            for (int column = 1; column != end; ++column)
            {
                string columnHeaderName = (range.Cells[row, column] as Excel.Range).Value2 as string;
                if (columnHeaderName != null && columnHeaderName.Length != 0)
                {
                    userInformationTable.Columns.Add(columnHeaderName);
                    config.colIdx.Add(column);
                    config.membershipColumnIndex = (columnHeaderName == _membershipDateColName && config.membershipColumnIndex == 0) ? column : config.membershipColumnIndex;
                    config.patientNameColumnIndex = (columnHeaderName == _customerNameColName && config.patientNameColumnIndex == 0) ? column : config.patientNameColumnIndex;
                }
            }
            return config;
        }

        delegate object ExtractMethod(string customerName, string currentValue);

        private object processValue(string customerName, string value, ExtractMethod subProcess = null)
        {
            if (subProcess != null)
            {
                return subProcess(customerName, value);
            }
            else
            {
                return value;
            }
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
                DataTable userInformationTable = new DataTable();
                if (range != null) {
                    ConfigurationControl configuration = this.GetHeaderNames(userInformationTable, range);
                    int end = range.Rows.Count + 1;
                    for (int row = 2; row != end; ++row) {
                        DataRow dr = userInformationTable.NewRow();
                        bool unsafeValueSkipAllRow = false;
                        string customerName = "";
                        for (int col = 0; col != configuration.colIdx.Count; ++col)
                        {
                            object retVal = null;
                            string value = GetValue(range, row, configuration.colIdx[col]);
                            if (configuration.colIdx[col] != configuration.membershipColumnIndex)
                            {
                                if (configuration.colIdx[col] == configuration.patientNameColumnIndex) 
                                {
                                    retVal = processValue(value, value);
                                    customerName = (string)retVal ?? customerName;
                                    if (retVal == null)
                                    {
                                        unsafeValueSkipAllRow = true;
                                        ExcelUtilities.DisplayError("Nombre de Cliente Incorrecto, no se utilizará la fila completa de su tabla.", customerName);
                                    }
                                }
                                else
                                {
                                    retVal = processValue(customerName, value);
                                    if (retVal == null)
                                    {
                                        unsafeValueSkipAllRow = true;
                                        ExcelUtilities.DisplayError("Campo Incorrecto, posiblemente ID, no se utilizará la fila completa de su tabla", customerName);
                                    }
                                }
                            } 
                            else 
                            {
                                ExtractMethod getDate = ExcelUtilities.GetDate;
                                retVal = processValue(customerName, value, getDate);
                                if (retVal == null) { unsafeValueSkipAllRow = true; }
                            }
                            if (unsafeValueSkipAllRow) break;
                            dr[col] = retVal;
                        }
                        if (unsafeValueSkipAllRow) continue;
                        userInformationTable.Rows.Add(dr);
                        userInformationTable.AcceptChanges();
                    }
                }
                workbook.Close(false, Missing.Value, Missing.Value);
                Marshal.ReleaseComObject(workbook);
                excelApp.Quit();
                Marshal.ReleaseComObject(excelApp);
                return userInformationTable;
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
