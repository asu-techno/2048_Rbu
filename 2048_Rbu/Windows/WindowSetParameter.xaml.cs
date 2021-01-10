using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using AS_Library.Link;
using AS_Library.Events;
using _2048_Rbu.Classes;
using AS_Library.Events.Classes;


namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Логика взаимодействия для Window_setParameter.xaml
    /// </summary>
    public partial class WindowSetParameter : Window
    {
        readonly OpcServer.OpcList _opcName;
        private readonly OPC_client _opc;
        private readonly Popup _popup;
        readonly string _variableType;
        readonly double _minVal;
        readonly double _maxVal;
        readonly string _parameter;
        readonly string _name;
        readonly double _startValue;
        double _endValue;
        private bool _err;
        private readonly int _numStation;
        private readonly int _digit;

        public WindowSetParameter(OpcServer.OpcList opcName, string name, double minVal, double maxVal, string parameter, string variableType, Popup popup = null, int numStation = 0, int digit = 0)
        {
            InitializeComponent();

            KeyDown += OnKeyDown;

            _opcName = opcName;
            _popup = popup;
            _numStation = numStation;
            _minVal = minVal;
            _maxVal = maxVal;
            _parameter = parameter;
            _name = name;
            TxtName.Text = name;
            lbl_Min.Content = minVal.ToString();
            lbl_Max.Content = maxVal.ToString();
            _variableType = variableType;
            _digit = digit;
            _opc = OpcServer.GetInstance().GetOpc(_opcName);

            if (_variableType == "Byte")
            {
                txt_Val.Text = _opc.cl.ReadByte(_parameter, out _err).ToString();
                _startValue = _opc.cl.ReadByte(_parameter, out _err);
            }
            if (_variableType == "Real")
            {
                txt_Val.Text = Math.Round(_opc.cl.ReadReal(_parameter, out _err), _digit).ToString($"F{_digit}");
                _startValue = Math.Round(_opc.cl.ReadReal(_parameter, out _err), _digit);
            }
            if (_variableType == "Int16")
            {
                txt_Val.Text = _opc.cl.ReadInt16(_parameter, out _err).ToString();
                _startValue = _opc.cl.ReadInt16(_parameter, out _err);
            }
            if (_variableType == "UInt16")
            {
                txt_Val.Text = _opc.cl.ReadUInt16(_parameter, out _err).ToString();
                _startValue = _opc.cl.ReadUInt16(_parameter, out _err);
            }
            if (_variableType == "UInt32")
            {
                txt_Val.Text = (_opc.cl.ReadUInt32(_parameter, out _err) / 1000).ToString();
                _startValue = _opc.cl.ReadUInt32(_parameter, out _err) / 1000;
            }
            if (_variableType == "UInt32m")
            {
                txt_Val.Text = (_opc.cl.ReadUInt32(_parameter, out _err) / 60000).ToString();
                _startValue = _opc.cl.ReadUInt32(_parameter, out _err) / 60000;
            }

            txt_Val.SelectAll();
            txt_Val.Focus();
        }

        void Save()
        {
            try
            {
                string txt = txt_Val.Text;
                //txt = txt.Replace(",", ".");
                txt = txt.Replace(".", ",");
                Double.TryParse(txt, out _endValue);
                if (_endValue <= _maxVal && _endValue >= _minVal)
                {
                    if (_variableType == "Real")
                    {
                        _opc.cl.WriteReal(_parameter, (float)_endValue, out _err);
                    }
                    if (_variableType == "Byte")
                    {
                        _opc.cl.WriteByte(_parameter, Convert.ToByte(_endValue), out _err);
                    }
                    if (_variableType == "Int16")
                    {
                        _opc.cl.WriteInt16(_parameter, Convert.ToInt16(_endValue), out _err);
                    }
                    if (_variableType == "UInt16")
                    {
                        _opc.cl.WriteUInt16(_parameter, Convert.ToUInt16(_endValue), out _err);
                    }
                    if (_variableType == "UInt32")
                    {
                        _opc.cl.WriteUInt32(_parameter, Convert.ToUInt32(_endValue) * 1000, out _err);
                    }
                    if (_variableType == "UInt32m")
                    {
                        _opc.cl.WriteUInt32(_parameter, Convert.ToUInt32(_endValue) * 60000, out _err);
                    }
                    //EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent("Параметр \"" + _name + "\" изменен с " + _startValue + " на " + txt, SystemEventType.UserDoing);
                    if (_err)
                        MessageBox.Show("Возможно запись не прошла.\nПроверьте OPC-сервер или соответствующий тег", "Предупреждение");
                    Close();
                }
                else
                {
                    MessageBox.Show("Введите число в заданном диапазоне. Повторите ввод", "Предупреждение");
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Запись не прошла. Повторите ввод", "Ошибка");
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Save();

            if (_popup != null)
                _popup.IsOpen = true;
        }

        private void txt_Val_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Save();

                if (_popup != null)
                    _popup.IsOpen = true;
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();

            if (_popup != null)
                _popup.IsOpen = true;
        }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Escape)
            {
                Close();

                if (_popup != null)
                    _popup.IsOpen = true;
            }
        }
    }
}
