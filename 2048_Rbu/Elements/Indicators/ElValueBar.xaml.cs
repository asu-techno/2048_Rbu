using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Indicators
{
    /// <summary>
    /// Interaction logic for el_SimpleValve.xaml
    /// </summary>
    public partial class ElValueBar : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        private double _value;
        public double Value
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

        #region MyRegion

        private int _myWidth;

        public int MyWidth
        {
            get { return _myWidth; }
            set
            {
                lbl_value.Width = value;
                _myWidth = value;
            }
        }

        public string Prefix { get; set; }
        public string ValuePcay { get; set; }

        private int _max;
        public int Max
        {
            get { return _max; }
            set
            {
                lbl_value.Maximum = value;
                _max = value;
            }
        }

        private Brush _color;
        public Brush Color
        {
            set
            {
                lbl_value.Foreground = value;
                _color = value;
            }
            get
            {
                return _color;
            }
        }

        #endregion

        public ElValueBar()
        {
            InitializeComponent();
        }

        public void Subscribe()
        {
            CreateSubscription();
        }

        private void HandleValueChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                Value = double.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        public void Unsubscribe()
        {
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            DataContext = this;
        }
        private void CreateSubscription()
        {
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var valueItem = new OpcMonitoredItem(_opc.cl.GetNode(Prefix + ValuePcay), OpcAttribute.Value);
            valueItem.DataChangeReceived += HandleValueChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(valueItem);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
