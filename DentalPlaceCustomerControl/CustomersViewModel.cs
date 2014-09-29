using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DentalPlaceAccessControl
{
    public class CustomersViewModel : ViewModelBase
    {
        #region "private members"

        CustomersModel _customerData;

        string _invalidMembership;
        string _invalidMsg;
        string _storedInvalidMsg;
        string _validMsg;
        int _currentIdx;
        string _currentName;
        string _currentStatus;
        string _welcomeLbl;
        string _borderBrush;
        int _columnLength;
        
        #endregion

        #region "Public Properties"

        public void setup(Dictionary<string, String[]> customerData, String[] headers, string MembershipInvalid, string validMsg, string invalidMsg)
        {
            _customerData = new CustomersModel(customerData, headers); 
            _invalidMembership = MembershipInvalid;
            _storedInvalidMsg = invalidMsg;
            _validMsg = validMsg;
            _columnLength = customerData[headers[0]].Length;
            _welcomeLbl = "";
            _borderBrush = "White";
            _currentIdx = -1;
        }

        public string[] ids
        {
            get
            {
                return _customerData.Ids;
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
                if (_currentIdx != value)
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
                    _currentName = _customerData.Names[idx];
                    OnPropertyChanged("CustomerName");
                    string membershipTimeStamp = _customerData.MembershipEndDates[idx];
                    string status = _customerData.Status[idx];
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
                _currentStatus = "Su membresía expiró el día " + dt.Day + " de " + _customerData.Months[dt.Month] + " de " + dt.Year;
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

    }
}
