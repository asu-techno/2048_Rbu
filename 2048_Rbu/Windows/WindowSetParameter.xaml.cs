using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using AS_Library.Link;
using AS_Library.Events;
using _2048_Rbu.Classes;
using AS_Library.Annotations;
using AS_Library.Events.Classes;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowSetParameter.xaml
    /// </summary>
    public partial class WindowSetParameter : INotifyPropertyChanged
    {
        public enum ValueType
        {
            Byte = 1,
            Int16,
            Int32,
            Real,
            UInt16,
            UInt32
        }

        private OpcServer.OpcList _opcName;
        private OPC_client _opc;
        private Popup _popup;
        private string _opcTag, _startValue;
        private ValueType _valueType;
        private double _finishValue;
        private bool _err;
        private int _digit;

        private string _parameterName;
        public string ParameterName
        {
            get
            {
                return _parameterName;
            }
            set
            {
                _parameterName = value;
                OnPropertyChanged(nameof(ParameterName));
            }
        }

        private string _parameterValue;
        public string ParameterValue
        {
            get
            {
                return _parameterValue;
            }
            set
            {
                _parameterValue = value;
                OnPropertyChanged(nameof(ParameterValue));
            }
        }

        private double _minValue;
        public double MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                _minValue = value;
                OnPropertyChanged(nameof(MinValue));
            }
        }

        private double _maxValue;
        public double MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
                OnPropertyChanged(nameof(MaxValue));
            }
        }

        private double _firstPrompt;
        public double FirstPrompt
        {
            get
            {
                return _firstPrompt;
            }
            set
            {
                _firstPrompt = value;
                OnPropertyChanged(nameof(FirstPrompt));
            }
        }

        private double _secondPrompt;
        public double SecondPrompt
        {
            get
            {
                return _secondPrompt;
            }
            set
            {
                _secondPrompt = value;
                OnPropertyChanged(nameof(SecondPrompt));
            }
        }

        private double _thirdPrompt;
        public double ThirdPrompt
        {
            get
            {
                return _thirdPrompt;
            }
            set
            {
                _thirdPrompt = value;
                OnPropertyChanged(nameof(ThirdPrompt));
            }
        }

        private double _fourthPrompt;
        public double FourthPrompt
        {
            get
            {
                return _fourthPrompt;
            }
            set
            {
                _fourthPrompt = value;
                OnPropertyChanged(nameof(FourthPrompt));
            }
        }

        private double _stepFeed;
        public double StepFeed
        {
            get
            {
                return _stepFeed;
            }
            set
            {
                _stepFeed = value;
                OnPropertyChanged(nameof(StepFeed));
            }
        }

        private bool _visPrompt;
        public bool VisPrompt
        {
            get
            {
                return _visPrompt;
            }
            set
            {
                _visPrompt = value;
                OnPropertyChanged(nameof(VisPrompt));
            }
        }

        private bool _visFeed;
        public bool VisFeed
        {
            get
            {
                return _visFeed;
            }
            set
            {
                _visFeed = value;
                OnPropertyChanged(nameof(VisFeed));
            }
        }

        private string _valueStringFormat;
        public string ValueStringFormat
        {
            get
            {
                return _valueStringFormat;
            }
            set
            {
                _valueStringFormat = value;
                OnPropertyChanged(nameof(ValueStringFormat));
            }
        }

        public WindowSetParameter(OpcServer.OpcList opcName, string parameterName, double minValue, double maxValue, string opcTag, ValueType valueType, Popup popup = null, int digit = 0, double? firstPrompt = null, double? secondPrompt = null, double? thirdPrompt = null, double? fourthPrompt = null, double? stepFeed = null)
        {
            InitializeComponent();
            KeyDown += OnKeyDown;

            DataContext = this;

            _opcName = opcName;
            _popup = popup;
            MinValue = minValue;
            MaxValue = maxValue;
            _opcTag = opcTag;
            ParameterName = parameterName;
            _valueType = valueType;
            _digit = digit;
            _opc = OpcServer.GetInstance().GetOpc(_opcName);

            switch (_valueType)
            {
                case ValueType.Byte:
                    ParameterValue = _opc.cl.ReadByte(_opcTag, out _err).ToString();
                    break;
                case ValueType.Int16:
                    ParameterValue = _opc.cl.ReadInt16(_opcTag, out _err).ToString();
                    break;
                case ValueType.Int32:
                    ParameterValue = _opc.cl.ReadInt32(_opcTag, out _err).ToString();
                    break;
                case ValueType.Real:
                    ParameterValue = Math.Round(_opc.cl.ReadReal(_opcTag, out _err), _digit).ToString($"F{_digit}");
                    break;
                case ValueType.UInt16:
                    ParameterValue = _opc.cl.ReadUInt16(_opcTag, out _err).ToString();
                    break;
                case ValueType.UInt32:
                    ParameterValue = (_opc.cl.ReadUInt32(_opcTag, out _err) / 1000).ToString();
                    break;
            }

            _startValue = ParameterValue;

            if (firstPrompt != null && secondPrompt != null && thirdPrompt != null && fourthPrompt != null)
            {
                FirstPrompt = Convert.ToDouble(firstPrompt);
                SecondPrompt = Convert.ToDouble(secondPrompt);
                ThirdPrompt = Convert.ToDouble(thirdPrompt);
                FourthPrompt = Convert.ToDouble(fourthPrompt);

                VisPrompt = true;
            }

            if (stepFeed != null)
            {
                StepFeed = Convert.ToDouble(stepFeed);
                VisFeed = true;
            }

            ValueStringFormat = $"F{_digit}";

            TxtValue.Focus();
        }

        void Save()
        {
            try
            {
                ParameterValue = ParameterValue.Replace(".", ",");
                var result = Double.TryParse(ParameterValue, out _finishValue);
                if (result)
                {
                    if (_finishValue <= MaxValue && _finishValue >= MinValue)
                    {
                        switch (_valueType)
                        {
                            case ValueType.Byte:
                                _opc.cl.WriteByte(_opcTag, Convert.ToByte(_finishValue), out _err);
                                break;
                            case ValueType.Int16:
                                _opc.cl.WriteInt16(_opcTag, Convert.ToInt16(_finishValue), out _err);
                                break;
                            case ValueType.Int32:
                                _opc.cl.WriteInt32(_opcTag, Convert.ToInt32(_finishValue), out _err);
                                break;
                            case ValueType.Real:
                                _opc.cl.WriteReal(_opcTag, (float)_finishValue, out _err);
                                break;
                            case ValueType.UInt16:
                                _opc.cl.WriteUInt16(_opcTag, Convert.ToUInt16(_finishValue), out _err);
                                break;
                            case ValueType.UInt32:
                                _opc.cl.WriteUInt32(_opcTag, Convert.ToUInt32(_finishValue) * 1000, out _err);
                                break;
                        }
                        EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent("Параметр \"" + ParameterName + "\" изменен с " + _startValue+" на " + _finishValue.ToString($"F{_digit}"), SystemEventType.UserDoing);
                        if (_err)
                            MessageBox.Show("Возможно запись не прошла.\nПроверьте OPC-сервер или соответствующий тег", "Предупреждение");
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Введите число в заданном диапазоне. Повторите ввод", "Предупреждение");
                    }
                }
                else
                {
                    MessageBox.Show("В строке не содержится число", "Ошибка!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Запись не прошла. Повторите ввод\n"+ ex.Message, "Ошибка");
            }
        }

        private string GetPrompt(double value)
        {
            return value >= MinValue && value <= MaxValue ? value.ToString($"F{_digit}") : value < MinValue ? MinValue.ToString($"F{_digit}") : MaxValue.ToString($"F{_digit}");
        }

        private void BtnSub_OnClick(object sender, RoutedEventArgs e)
        {
            ParameterValue = ParameterValue.Replace(".", ",");
            var result = Double.TryParse(ParameterValue, out var feedValue);
            if (result)
                ParameterValue = feedValue - StepFeed >= MinValue
                    ? (feedValue - StepFeed).ToString($"F{_digit}")
                    : MinValue.ToString($"F{_digit}");
            else
                MessageBox.Show("В строке не содержится число", "Ошибка!");

            TxtValue.Focus();
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            ParameterValue = ParameterValue.Replace(".", ",");
            var result = Double.TryParse(ParameterValue, out var feedValue);
            if (result)
                ParameterValue = feedValue + StepFeed <= MaxValue ? (feedValue + StepFeed).ToString($"F{_digit}") : MaxValue.ToString($"F{_digit}");
            else
                MessageBox.Show("В строке не содержится число", "Ошибка!");

            TxtValue.Focus();
        }

        private void BtnFirst_OnClick(object sender, RoutedEventArgs e)
        {
            ParameterValue = GetPrompt(FirstPrompt);
            TxtValue.SelectAll();
            TxtValue.Focus();
        }

        private void BtnSecond_OnClick(object sender, RoutedEventArgs e)
        {
            ParameterValue = GetPrompt(SecondPrompt);
            TxtValue.Focus();
        }

        private void BtnThird_OnClick(object sender, RoutedEventArgs e)
        {
            ParameterValue = GetPrompt(ThirdPrompt); 
            TxtValue.Focus();
        }

        private void BtnFourth_OnClick(object sender, RoutedEventArgs e)
        {
            ParameterValue = GetPrompt(FourthPrompt); 
            TxtValue.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();

            if (_popup != null)
                _popup.IsOpen = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();

            if (_popup != null)
                _popup.IsOpen = true;
        }

        private void TxtValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Save();

                if (_popup != null)
                    _popup.IsOpen = true;
            }
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
