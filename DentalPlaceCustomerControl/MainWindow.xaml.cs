using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using dragonz.actb.core;
using dragonz.actb.provider;


namespace DentalPlaceCustomerControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        Customers _customersData;

        public MainWindow()
        {
            InitializeComponent();
            _customersData = new Customers();
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
            {
                DefaultValue = FindResource(typeof(Window))
            });

            int error_t = CheckConfig();
            if (error_t == 1)
            {
                var appSettings = ConfigurationManager.AppSettings;
                String[] Headers = { appSettings["CustomersID"], appSettings["CustomersNames"], appSettings["CustomersMembershipDate"], appSettings["CustomersMembershipStatus"] };
                string directoryToCustomersFile;
                if (String.IsNullOrEmpty(appSettings["CustomersSourceDirectory"]))
                {
                    directoryToCustomersFile = Environment.CurrentDirectory;
                }
                else
                {
                    directoryToCustomersFile = appSettings["CustomersSourceDirectory"];
                }
                string sourceFile = appSettings["CustomersSourceFile"];
                actbIds.MaxLength = setCustomersData(Headers, directoryToCustomersFile, sourceFile, Headers[0], 
                    appSettings["CustomersMembershipInvalid"],
                    appSettings["ValidCustomerMessage"],
                    appSettings["InvalidCustomerMessage"]);
                actbIds.ItemsSource = _customersData.ids;
                actbIds.AutoCompleteManager.DataProvider = new SimpleStaticDataProvider(_customersData.ids);
                actbIds.AutoCompleteManager.AutoAppend = true;
                this.DataContext = _customersData;
                DataObject.AddPastingHandler(actbIds, new DataObjectPastingEventHandler(actbIds.AutoCompleteComboBox_TextPasted));
            }
        }

        public int setCustomersData(String[] headers, string pathToConfig, string configFile, string headerColName, string invalidMembership, string validMsg, string invalidMsg)
        {
            ExcelConfiguration e = new ExcelConfiguration(pathToConfig, configFile, headers[2]);
            DataTable customersExcel = e.Data;
            Dictionary<string, String[]> customerData = new Dictionary<string,string[]>();
            for(int i = 0; i != headers.Length; ++i) {
                String[] data = customersExcel.AsEnumerable().Select(r => r.Field<string>(headers[i])).ToArray();
                customerData[headers[i]] = data;
            }
            int maxLength = -1;
            for (int i = 0; i != customerData[headerColName].Length; ++i)
            {
                int currentLength = customerData[headerColName][i].Length;
                if (currentLength > maxLength)
                {
                    maxLength = currentLength;
                }
            }
            _customersData.setup(customerData, headers, invalidMembership, validMsg, invalidMsg);
            return maxLength;
        }

        public int CheckConfig()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    return appSettings.Count;
                }
                else
                {
                    return 1;
                }
            }
            catch (ConfigurationErrorsException)
            {
                return -1;
            }
        }

    }
}
