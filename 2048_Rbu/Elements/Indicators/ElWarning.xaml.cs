using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
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
    public partial class ElWarning : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private string _startName = "";

        private Visibility _vis;
        public Visibility Vis
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

        #region MyRegion

        public string ValuePcy { get; set; }
        public string ValueTime { get; set; }
        public bool Logic { get; set; }

        private string _nameObject;
        public string NameObject
        {
            get { return _nameObject; }
            set
            {
                TxtObject.Text = value;
                _nameObject = value;
            }
        }

        private string _valueBtn;
        public string ValueBtn
        {
            get { return _valueBtn; }
            set
            {
                BtnRepeat.Visibility = value != null ? Visibility.Visible : Visibility.Collapsed;
                _valueBtn = value;
            }
        }

        private int _myWidth;
        public int MyWidth
        {
            get { return _myWidth; }
            set
            {
                MainGrid.Width = value;
                _myWidth = value;
            }
        }

        private int _myHeight;
        public int MyHeight
        {
            get { return _myHeight; }
            set
            {
                MainGrid.Height = value;
                _myHeight = value;
            }
        }

        private Brush _color;
        public Brush Color
        {
            set
            {
                MainGrid.Background = value;
                _color = value;
            }
            get
            {
                return _color;
            }
        }

        private Thickness _myPadding;
        public Thickness MyPadding
        {
            get { return _myPadding; }
            set
            {
                LblObject.Padding = value;
                _myPadding = value;
            }
        }

        #endregion

        public ElWarning()
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

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            _startName = NameObject;

            DataContext = this;
        }
        private void CreateSubscription()
        {
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var visItem = new OpcMonitoredItem(_opc.cl.GetNode(ValuePcy), OpcAttribute.Value);
            visItem.DataChangeReceived += HandleVisChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visItem);

            if (ValueTime != null)
            {
                var valueTimeItem = new OpcMonitoredItem(_opc.cl.GetNode(ValueTime), OpcAttribute.Value);
                valueTimeItem.DataChangeReceived += HandleValueTimeChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(valueTimeItem);
            }
        }

        private void HandleVisChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                Vis = bool.Parse(e.Item.Value.ToString()) == Logic ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (System.Exception)
            {
            }
        }

        private void HandleValueTimeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                NameObject = _startName != null ? _startName + " " + int.Parse(e.Item.Value.ToString()) + " с" : "";
            }
            catch (System.Exception)
            {
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BtnRepeat_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;

            Methods.ButtonClick(btn, BtnRepeat, ValueBtn, true, NameObject + ". Повторить");
        }
    }
}
