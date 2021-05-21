using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Control
{
    /// <summary>
    /// Interaction logic for ElManualDosing.xaml
    /// </summary>
    public partial class ElManualDosing : INotifyPropertyChanged, IElementsUpdater 
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        private bool _leftManual, _rightManual;

        public string RoughtPcy { get; set; }
        public string PrecisePcy { get; set; }
        public string LeftManualPcy { get; set; }
        public string RightManualPcy { get; set; }
        public string EventText { get; set; }

        private bool _vis;
        public bool Vis
        {
            get
            {
                return _vis;
            }
            set
            {
                _vis = value;
                OnPropertyChanged(nameof(Vis));
            }
        }

        public ElManualDosing()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            DataContext = this;
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
            var leftManualItem = new OpcMonitoredItem(_opc.cl.GetNode(LeftManualPcy), OpcAttribute.Value);
            leftManualItem.DataChangeReceived += HandleLeftManualChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(leftManualItem);
            var rightManualItem = new OpcMonitoredItem(_opc.cl.GetNode(RightManualPcy), OpcAttribute.Value);
            rightManualItem.DataChangeReceived += HandleRightManualChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(rightManualItem);
        }

        private void HandleLeftManualChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _leftManual = bool.Parse(e.Item.Value.ToString());
            VisStatus();
        }

        private void HandleRightManualChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _rightManual = bool.Parse(e.Item.Value.ToString());
            VisStatus();
        }

        private void VisStatus()
        {
            Vis = _leftManual || _rightManual;
        }

        private void BtnRought_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnRought, RoughtPcy, true, EventText == null? EventText:"" + ". Грубое дозирование");
        }

        private void BtnRought_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnRought, RoughtPcy, false);
        }

        private void BtnPrecise_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnPrecise, PrecisePcy, true, EventText == null ? EventText : "" + ". Точное дозирование");
        }

        private void BtnPrecise_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnPrecise, PrecisePcy, false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
