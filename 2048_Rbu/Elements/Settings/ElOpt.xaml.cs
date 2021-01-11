using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Classes;
using System.Runtime.CompilerServices;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Settings
{
    /// <summary>
    /// Логика взаимодействия для el_Screen.xaml
    /// </summary>
    public partial class ElOpt : INotifyPropertyChanged, IElementsUpdater
    {
        private OpcServer.OpcList _opcName;
        private OPC_client _opc;
        string _varName, _paramRead, _paramWrite, _type;
        double _min, _max;
        int _numStation, _digit;
        private bool _err;

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

        public ElOpt()
        {
            InitializeComponent();

            DataContext = this;
        }

        public void Initialize(OpcServer.OpcList opcName, string varName, double min, double max, string paramRead, string type, int numStation = 0, int digit = 0, string paramWrite = "", Brush brush = null)
        {
            _opcName = opcName;

            _varName = varName;
            _type = type;
            _paramRead = paramRead;
            _paramWrite = paramWrite;
            _min = min;
            _max = max;
            _numStation = numStation;
            _digit = digit;

            TxtName.Text = varName;

            if (brush != null)
                lbl_param.Background = brush;
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
            var paramItem = new OpcMonitoredItem(_opc.cl.GetNode(_paramRead), OpcAttribute.Value);
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

            Methods.SetParameter(lbl_param, btn, _opcName, _varName, _min, _max, _paramRead, _type, null, _numStation, _digit);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
