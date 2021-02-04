using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using AS_Library.Link;
using _2048_Rbu.Classes;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using Opc.UaFx;
using Opc.UaFx.Client;
using AS_Library.Annotations;

namespace _2048_Rbu.Elements.Indicators
{
    /// <summary>
    /// Interaction logic for el_SimpleValve.xaml
    /// </summary>
    public partial class ElDone : INotifyPropertyChanged, IElementsUpdater
    {
        private OpcServer.OpcList _opcName;
        private string _readVal;
        private OPC_client _opc;

        private SolidColorBrush _brush;
        public SolidColorBrush Brush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                OnPropertyChanged(nameof(Brush));
            }
        }

        #region MyRegion

        public string Prefix { get; set; }
        public string OnPcy { get; set; }
        public SolidColorBrush OnBrush { get; set; }

        private int _radius;
        public int Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                RectDone.RadiusX = RectDone.RadiusY = value;
                _radius = value;
            }
        }

        #endregion

        public ElDone()
        {
            InitializeComponent();
        }

        public void Subscribe()
        {
            CreateSubscription();
        }

        public void Unsubscribe()
        {
        }

        public void Initialize(OpcServer.OpcList opcName, string readVal = null)
        {
            _opcName = opcName;
            _readVal = readVal;
            Brush = Brushes.White;

            DataContext = this;
        }

        private void CreateSubscription()
        {
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var readVal = _readVal ?? OnPcy;
            var visItem = new OpcMonitoredItem(_opc.cl.GetNode(Prefix + readVal), OpcAttribute.Value);
            visItem.DataChangeReceived += HandleVisChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visItem);
        }

        private void HandleVisChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            Brush = bool.Parse(e.Item.Value.ToString()) ? (OnBrush != null ? OnBrush : (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF85FC84"))) : Brushes.White;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
