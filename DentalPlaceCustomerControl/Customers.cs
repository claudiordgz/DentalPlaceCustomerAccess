using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalPlaceCustomerControl
{
    public class Customers : INotifyPropertyChanged
    {
        #region "private members"

        String[] _CustomersId;
        String[] _CustomersNames;
        String[] _CustomersMembershipEndDate;
        String[] _CustomersStatus;
        string _invalidMembership;
        string _invalidMsg;
        string _storedInvalidMsg;
        string _validMsg;
        int _currentIdx;
        string _currentName;
        string _currentMembership;
        string _currentStatus;
        string _welcomeLbl;
        string _MembershipLbl;
        string _borderBrush;
        int _columnLength;

        Dictionary<int, string> _Months;
        #endregion

        #region "Public Properties"

        public void setup(Dictionary<string, String[]> customerData, String[] headers, string MembershipInvalid, string validMsg, string invalidMsg)
        {
            _Months = new Dictionary<int, string>()
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
            _invalidMembership = MembershipInvalid;
            _storedInvalidMsg = invalidMsg;
            _validMsg = validMsg;
            bool isValid = customerData[headers[0]].Length == customerData[headers[1]].Length && 
                customerData[headers[2]].Length == customerData[headers[3]].Length && customerData[headers[0]].Length == customerData[headers[3]].Length;
            if (isValid)
            {
                _columnLength = customerData[headers[0]].Length;
                _CustomersId = customerData[headers[0]];
                _CustomersNames = customerData[headers[1]];
                _CustomersMembershipEndDate = customerData[headers[2]];
                _CustomersStatus = customerData[headers[3]];
                _welcomeLbl = "";
                _MembershipLbl = "";
                _borderBrush = "White";
            }
            _currentIdx = -1;
        }

        public string[] ids
        {
            get
            {
                return _CustomersId;
            }
        }

        public int CustomerIndex
        {
            get 
            {
                return _currentIdx;
            }
            set
            {
                if(_currentIdx != value)
                {
                    _currentIdx = value;
                    SetOtherVariables(_currentIdx);
                    OnPropertyChanged("CustomerIndex");
                }
            }
        }

        private void SetOtherVariables(int idx)
        {
            if (idx != -1)
            {
                if (idx < _columnLength)
                {
                    _welcomeLbl = "Bienvenido(a)";
                    OnPropertyChanged("WelcomeLabel");
                    _currentName = _CustomersNames[idx];
                    OnPropertyChanged("CustomerName");
                    string membershipTimeStamp = _CustomersMembershipEndDate[idx];
                    string status = _CustomersStatus[idx];
                    setMembership(membershipTimeStamp, status);
                }
            }
            else
            {
                _currentName = "";
                OnPropertyChanged("CustomerName");
                _currentStatus = "";
                OnPropertyChanged("CustomerStatus");
                _welcomeLbl = "";
                OnPropertyChanged("WelcomeLabel");
                _borderBrush = "White";
                OnPropertyChanged("BorderBrush");
                _invalidMsg = "";
                OnPropertyChanged("InvalidCustomerMessage");
            }
        }

        private void setMembership(string membershipEndDate, string membershipStatus)
        {
            if (membershipStatus == _invalidMembership)
            {
                _borderBrush = "Red";
                OnPropertyChanged("BorderBrush");
                DateTime dt = Convert.ToDateTime(membershipEndDate);
                _currentStatus = "Su membresía expiró el día " + dt.Day + " de " + _Months[dt.Month] + " de " + dt.Year;
                OnPropertyChanged("CustomerStatus");
                _invalidMsg = _storedInvalidMsg;
                OnPropertyChanged("InvalidCustomerMessage");
            }
            else
            {
                _borderBrush = "Green";
                OnPropertyChanged("BorderBrush");
                _currentStatus = _validMsg;
                OnPropertyChanged("CustomerStatus");
                _invalidMsg = "";
                OnPropertyChanged("InvalidCustomerMessage");
            }
        }

        public string CustomerStatus
        {
            get
            {
                return _currentStatus;
            }
        }

        public string InvalidCustomerMessage
        {
            get
            {
                return _invalidMsg;
            }
        }

        public string CustomerName
        {
            get
            {
                return _currentName;
            }
        }

        public string WelcomeLabel
        {
            get
            {
                return _welcomeLbl;
            }
        }

        public string BorderBrush
        {
            get
            {
                return _borderBrush;
            }
        }
        

        #endregion

        #region "INotifyPropertyChanged members"

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
