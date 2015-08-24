using dragonz.actb.provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DentalPlaceAccessControl
{
    public class DentalPlaceConfigHandler
    {
        public commands.OpenFileCommand openFile;
        public string fileName;
        public String[] Headers;
        public string invalidMembership;
        public string validMsg;
        public string invalidMsg;
        public dragonz.actb.control.AutoCompleteComboBox controlReference;
        public CustomersViewModel viewModelReference;

        public DentalPlaceConfigHandler()
        {
            openFile = new commands.OpenFileCommand();
        }

        public void LoadedConfigurationFile()
        {
            controlReference.MaxLength = setCustomersData(viewModelReference);
            controlReference.ItemsSource = viewModelReference.ids;
            controlReference.AutoCompleteManager.DataProvider = new SimpleStaticDataProvider(viewModelReference.ids);
            controlReference.AutoCompleteManager.AutoAppend = true;
            DataObject.AddPastingHandler(controlReference, new DataObjectPastingEventHandler(controlReference.AutoCompleteComboBox_TextPasted));
        }

        public int setCustomersData(CustomersViewModel customerData)
        {
            int maxLength = -1; 
            if (openFile.filename != null)
            {
                fileName = openFile.filename; 
            }
            ExcelConfiguration e = new ExcelConfiguration(Headers[1], Headers[2]);
            e.configurationFile = fileName;
            DataTable customersExcel = e.Data;
            Dictionary<string, String[]> lCustomerData = new Dictionary<string, string[]>();
            for (int i = 0; i != Headers.Length; ++i)
            {
                String[] data = customersExcel.AsEnumerable().Select(r => r.Field<string>(Headers[i])).ToArray();
                lCustomerData[Headers[i]] = data;
            }

            for (int i = 0; i != lCustomerData[Headers[0]].Length; ++i)
            {
                int currentLength = lCustomerData[Headers[0]][i].Length;
                if (currentLength > maxLength)
                {
                    maxLength = currentLength;
                }
            }
            customerData.setup(lCustomerData, Headers, invalidMembership, validMsg, invalidMsg);
            
            return maxLength;
        }

    }
}
