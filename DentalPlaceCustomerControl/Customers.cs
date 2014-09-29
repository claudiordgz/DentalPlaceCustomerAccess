using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalPlaceAccessControl
{
    public class CustomersModel
    {

        public String[] Ids { get; set; }
        public String[] Names { get; set; }
        public String[] MembershipEndDates { get; set; }
        public String[] Status { get; set; }
        public Dictionary<int, string> Months { get; set; }


        public CustomersModel(Dictionary<string, String[]> customerData, String[] headers)
        {
            Months = new Dictionary<int, string>()
            {
                {1, "Enero"},
                {2, "Febrero"},
                {3, "Marzo"},
                {4, "Abril"},
                {5, "Mayo"},
                {6, "Junio"},
                {7, "Julio"},
                {8, "Agosto"},
                {9, "Septiembre"},
                {10, "Octubre"},
                {11, "Noviembre"},
                {12, "Diciembre"}
            };

            bool isValid = customerData[headers[0]].Length == customerData[headers[1]].Length &&
                customerData[headers[2]].Length == customerData[headers[3]].Length && customerData[headers[0]].Length == customerData[headers[3]].Length;
            if (isValid)
            {
                Ids = customerData[headers[0]];
                Names = customerData[headers[1]];
                MembershipEndDates = customerData[headers[2]];
                Status = customerData[headers[3]];
            }
        }

    }
}
