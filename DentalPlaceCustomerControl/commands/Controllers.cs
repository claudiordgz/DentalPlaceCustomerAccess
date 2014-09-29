using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalPlaceAccessControl.commands
{
    public class OpenFileCommand
    {
        public string filename;
        public void openDialog()
        {
            var dlg = new OpenFileDialog();
            dlg.FileName = "Clientes"; // Default file name 
            dlg.DefaultExt = ".xlsx"; // Default file extension 
            dlg.Filter = "Excel document |*.xlsx"; // Filter files by extension 
            Nullable<bool> result = dlg.ShowDialog();
            // Process open file dialog box results 
            filename = null;
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
            }
        }
    }
}
