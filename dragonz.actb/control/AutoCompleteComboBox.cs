using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dragonz.actb.core;
using System.Text.RegularExpressions;

namespace dragonz.actb.control
{
    public class AutoCompleteComboBox : ComboBox
    {
        private AutoCompleteManager _acm;
        private TextBox _textBox;
        private int _oldSelStart;
        private int _oldSelLength;
        private int _maxLength;
        private string _oldText;

        public AutoCompleteManager AutoCompleteManager
        {
            get { return _acm; }
        }

        public AutoCompleteComboBox()
        {
            this.IsEditable = true;
            this.IsTextSearchEnabled = false;
            this.GotMouseCapture += AutoCompleteComboBox_GotMouseCapture;
            
            _acm = new AutoCompleteManager();
        }

        public int MaxLength
        {
            set
            {
                if (_maxLength != value)
                {
                    _maxLength = value;
                }
            }
        }

        private void AutoCompleteComboBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            _oldSelStart = _textBox.SelectionStart;
            _oldSelLength = _textBox.SelectionLength;
            _oldText = _textBox.Text;
        }

        public void AutoCompleteComboBox_TextPasted(object sender, DataObjectPastingEventArgs e)
        {
            if (e.SourceDataObject.GetDataPresent(DataFormats.Text, true) == false)
            {
                return;
            }
            string args = e.DataObject.GetData(typeof(string)) as string;
            args = Regex.Replace(args, @"\t|\n|\r", "");
            if (_maxLength != -1)
            {
                if (args.Length == _maxLength)
                {
                    this.IsTextSearchEnabled = true;
                }
                else
                {
                    this.IsTextSearchEnabled = false;
                }
            }
            if (_acm.AutoCompleting)
            {
                return;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.IsTextSearchEnabled = true;
            }
            else if (e.Key == Key.Back)
            {
                _textBox.Text = "";
            }
            else
            {
                if (_maxLength != -1)
                {
                    if (_textBox.SelectionStart == _maxLength - 1)
                    {
                        this.IsTextSearchEnabled = true;
                    }
                    else
                    {
                        this.IsTextSearchEnabled = false;
                    }
                }
            }
            _oldSelStart = _textBox.SelectionStart;
            _oldSelLength = _textBox.SelectionLength;
            _oldText = _textBox.Text;
            if (_acm.AutoCompleting)
            {
                return;
            }
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                this.SelectedValue = this.Text;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            _acm.Disabled = true;
            this.IsTextSearchEnabled = true;
            this.SelectedValue = Text;

            base.OnDropDownOpened(e);

            if (this.SelectedValue == null)
            {
                this.Text = _oldText;
                _textBox.SelectionStart = _oldSelStart;
                _textBox.SelectionLength = _oldSelLength;
            }
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            _acm.Disabled = false;
            this.IsTextSearchEnabled = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            _acm.AttachTextBox(_textBox);
        }
    }
}