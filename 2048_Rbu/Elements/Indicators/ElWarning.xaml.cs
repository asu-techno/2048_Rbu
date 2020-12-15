using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Lib_2048.Classes;
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

        private string _nameObject;
        public string NameObject
        {
            get { return _nameObject; }
            set
            {
                txt_object.Text = value;
                _nameObject = value;
            }
        }

        private int _myWidth;
        public int MyWidth
        {
            get { return _myWidth; }
            set
            {
                lbl_object.Width = value;
                _myWidth = value;
            }
        }

        private int _myHeight;
        public int MyHeight
        {
            get { return _myHeight; }
            set
            {
                lbl_object.Height = value;
                _myHeight = value;
            }
        }

        public bool Logic { get; set; }

        private Brush _color;
        public Brush Color
        {
            set
            {
                lbl_object.Background = value;
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
                lbl_object.Padding = value;
                _myPadding = value;
            }
        }

        #endregion

        public ElWarning()
        {
            InitializeComponent();

            HideImages();
        }

        public void Subscribe()
        {
            CreateSubscription();
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
            if (ValuePcy != null)
            {
                var visItem = new OpcMonitoredItem(_opc.cl.GetNode(ValuePcy), OpcAttribute.Value);
                visItem.DataChangeReceived += HandleVisChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visItem);
            }
        }
        void HideImages()
        {
            Vis = Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
