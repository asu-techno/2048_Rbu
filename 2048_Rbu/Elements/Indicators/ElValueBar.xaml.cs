using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Interfaces;
using _2048_Rbu.Classes;
using _2048_Rbu.Classes.ViewModel;
using _2048_Rbu.Windows;
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

        private bool _visCheck;
        public bool VisCheck
        {
            get
            {
                return _visCheck;
            }
            set
            {
                _visCheck = value;
                OnPropertyChanged(nameof(VisCheck));
            }
        }

        private SolidColorBrush _brush;
        public SolidColorBrush Brush
        {
            get
            {
                return _brush;
            }
            set
            {
                _brush = value;
                OnPropertyChanged(nameof(Brush));
            }
        }

        private SolidColorBrush _backgroundBrush;
        public SolidColorBrush BackgroundBrush
        {
            get
            {
                return _backgroundBrush;
            }
            set
            {
                _backgroundBrush = value;
                OnPropertyChanged(nameof(BackgroundBrush));
            }
        }

        private bool _visBox;
        public bool VisBox
        {
            get
            {
                return _visBox;
            }
            set
            {
                _visBox = value;
                OnPropertyChanged(nameof(VisBox));
            }
        }

        #region MyRegion

        public string ValuePcay { get; set; }
        public double NormalMin { get; set; }
        public SolidColorBrush ForegroundBrush { get; set; }
        public int CheckLevelNum { get; set; }

        private string _valueBoxPcay;
        public string ValueBoxPcay
        {
            get { return _valueBoxPcay; }
            set
            {
                ValueBox.ValuePcay = value;
                _valueBoxPcay = value;
            }
        }

        private Static.ContainerItem _containerItem;
        public Static.ContainerItem ContainerItem
        {
            get { return _containerItem;}
            set
            {
                ValueBox.ContainerItem = value;
                _containerItem = value;
                }
        }

        private int _myWidth;
        public int MyWidth
        {
            get { return _myWidth; }
            set
            {
                ProgressBorder.Width = ProgressValue.Width = value;
                _myWidth = value;
            }
        }

        private int _myHeight;
        public int MyHeight
        {
            get { return _myHeight; }
            set
            {
                ProgressBorder.Height = value;
                _myHeight = value;
            }
        }

        private int _max;
        public int Max
        {
            get { return _max; }
            set
            {
                ProgressValue.Maximum = value;
                _max = value;
            }
        }

        #endregion

        public ElValueBar()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            ValueBox.Initialize(_opcName);

            DataContext = this;
        }

        public void Subscribe()
        {
            CreateSubscription();
            ValueBox.Subscribe();
        }

        public void Unsubscribe()
        {
        }

        private void CreateSubscription()
        {
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var valueItem = new OpcMonitoredItem(_opc.cl.GetNode(ValuePcay), OpcAttribute.Value);
            valueItem.DataChangeReceived += HandleValueChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(valueItem);

            if (CheckLevelNum != 0)
            {
                var checkLevelItem = new OpcMonitoredItem(_opc.cl.GetNode("DO_CheckLevel_Cement" + CheckLevelNum), OpcAttribute.Value);
                checkLevelItem.DataChangeReceived += HandleCheckLevelChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(checkLevelItem);
            }
        }

        private void HandleValueChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                Value = double.Parse(e.Item.Value.ToString());
                Brush = GetBrush();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleCheckLevelChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                VisCheck = bool.Parse(e.Item.Value.ToString());
                Brush = GetBrush();
            }
            catch (Exception exception)
            {
            }
        }

        private SolidColorBrush GetBrush()
        {
            if (VisCheck)
            {
                BackgroundBrush = Brushes.Yellow;
                return Brushes.Yellow;
            }
            else
            {
                BackgroundBrush = Brushes.LightGray;
                return NormalMin != 0
                    ? (Value > NormalMin
                        ? (ForegroundBrush != null ? ForegroundBrush : Brushes.DeepSkyBlue)
                        : Brushes.Salmon)
                    : (ForegroundBrush != null ? ForegroundBrush : Brushes.DeepSkyBlue);
            }
        }

        private void Rect_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (CheckLevelNum != 0)
            {
                RectObject.Opacity = 1;
                RectObject.ToolTip = "Контроль уровня";
            }
            VisBox = true;
        }

        private void Rect_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (CheckLevelNum != 0)
             RectObject.Opacity = 0;
            VisBox = false;
        }
        
        private void ElValueBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckLevelNum != 0)
            {
                var window = new WindowCheckLevelSettings(new CheckLevelSettingsViewModel(_opcName, CheckLevelNum));
                window.ShowDialog();
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
