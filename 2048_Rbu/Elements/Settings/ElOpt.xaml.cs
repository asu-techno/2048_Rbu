using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Classes;
using System.Runtime.CompilerServices;
using _2048_Rbu.Interfaces;
using _2048_Rbu.Windows;
using AS_Library.Annotations;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Settings
{
    /// <summary>
    /// Логика взаимодействия для ElOpt.xaml
    /// </summary>
    public partial class ElOpt : INotifyPropertyChanged, IElementsUpdater
    {
        private OpcServer.OpcList _opcName;
        private OPC_client _opc;
        private WindowSetParameter.ValueType _valueType;
        private string _opcTag;
        private double _minValue, _maxValue;
        private int _digit;
        private double? _firstPrompt, _secondPrompt, _thirdPrompt, _fourthPrompt, _stepFeed;

        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

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

        public ElOpt()
        {
            InitializeComponent();

            DataContext = this;
        }

        public void Initialize(OpcServer.OpcList opcName, string parameterName, double minValue, double maxValue, string opcTag, WindowSetParameter.ValueType valueType, int digit = 0, double? firstPrompt = null, double? secondPrompt = null, double? thirdPrompt = null, double? fourthPrompt = null, double? stepFeed = null)
        {
            _opcName = opcName;
            ParameterName = parameterName;
            _minValue = minValue;
            _maxValue = maxValue;
            _opcTag = opcTag;
            _valueType = valueType;
            _digit = digit;
            _firstPrompt = firstPrompt;
            _secondPrompt = secondPrompt;
            _thirdPrompt = thirdPrompt;
            _fourthPrompt = fourthPrompt;
            _stepFeed = stepFeed;
        }

        public void Subscribe()
        {
            CreateSubscription();
        }

        public void Unsubscribe()
        {
        }

        private void CreateSubscription()
        {
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var paramItem = new OpcMonitoredItem(_opc.cl.GetNode(_opcTag), OpcAttribute.Value);
            paramItem.DataChangeReceived += HandleIdChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(paramItem);
        }

        private void HandleIdChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                Value = double.Parse(e.Item.Value.ToString()).ToString($"F{_digit}");
            }
            catch (Exception exception)
            {
            }
        }

        private void SetParam_MouseDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;

            Methods.SetParameter(LblParam, btn, _opcName, ParameterName, _minValue, _maxValue, _opcTag, _valueType, null, _digit,_firstPrompt,_secondPrompt,_thirdPrompt,_fourthPrompt,_stepFeed);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
