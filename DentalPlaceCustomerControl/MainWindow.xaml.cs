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
using System.IO;


namespace DentalPlaceAccessControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        CustomersViewModel _customersData;
        DentalPlaceConfigHandler cfgHandler;

        public MainWindow()
        {
            InitializeComponent();
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
            {
                DefaultValue = FindResource(typeof(Window))
            });
            
            int error_t = CheckConfig();
            if (error_t == 1)
            {
                var appSettings = ConfigurationManager.AppSettings;
                cfgHandler = new DentalPlaceConfigHandler();
                _customersData = new CustomersViewModel();
                String[] Headers = { appSettings["CustomersID"], appSettings["CustomersNames"], appSettings["CustomersMembershipDate"], appSettings["CustomersMembershipStatus"] };
                AccessControlMenu.openFile = cfgHandler.openFile.openDialog;
                AccessControlMenu.loadRules = cfgHandler.LoadedConfigurationFile;
                cfgHandler.Headers = Headers;
                cfgHandler.invalidMembership = appSettings["CustomersMembershipInvalid"];
                cfgHandler.validMsg = appSettings["ValidCustomerMessage"];
                cfgHandler.invalidMsg = appSettings["InvalidCustomerMessage"];
                cfgHandler.controlReference = actbIds;
                cfgHandler.viewModelReference = _customersData;

                string sourceFile = appSettings["CustomersSourceFile"];
                if (File.Exists(sourceFile))
                {
                    cfgHandler.fileName = sourceFile;
                    cfgHandler.LoadedConfigurationFile();
                }
                else
                {
                    cfgHandler.fileName = null;
                }
                this.DataContext = _customersData;
            }
        }

        public int CheckConfig()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings.Count == 0) {
                    return appSettings.Count;
                } else {
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
