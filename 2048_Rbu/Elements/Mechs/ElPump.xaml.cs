using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using _2048_Rbu.Windows;
using AS_Library.Annotations;
using AS_Library.Events.Classes;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Mechs
{
    /// <summary>
    /// Interaction logic for el_SimpleValve.xaml
    /// </summary>
    public partial class ElPump : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private bool _err;
        private int _numStation;

        public enum Position { LeftUp, Up, RightUp, Left, Right, LeftDown, Down, RightDown };

        private Visibility _visFreq;
        public Visibility VisFreq
        {
            get
            {
                return _visFreq;
            }
            set
            {
                _visFreq = value;
                OnPropertyChanged(nameof(VisFreq));
            }
        }

        private Visibility _visAlarm;
        public Visibility VisAlarm
        {
            get
            {
                return _visAlarm;
            }
            set
            {
                _visAlarm = value;
                OnPropertyChanged(nameof(VisAlarm));
            }
        }

        private Visibility _visOn;
        public Visibility VisOn
        {
            get
            {
                return _visOn;
            }
            set
            {
                _visOn = value;
                OnPropertyChanged(nameof(VisOn));
            }
        }

        private bool _modeAutomat;
        public bool ModeAutomat
        {
            get
            {
                return _modeAutomat;
            }
            set
            {
                _modeAutomat = value;
                OnPropertyChanged(nameof(ModeAutomat));
            }
        }

        private bool _modeManual;
        public bool ModeManual
        {
            get
            {
                return _modeManual;
            }
            set
            {
                _modeManual = value;
                OnPropertyChanged(nameof(ModeManual));
            }
        }

        private bool _modeManualDosing;
        public bool ModeManualDosing
        {
            get
            {
                return _modeManualDosing;
            }
            set
            {
                _modeManualDosing = value;
                OnPropertyChanged(nameof(ModeManualDosing));
            }
        }

        private string _freq;
        public string Freq
        {
            get
            {
                return _freq;
            }
            set
            {
                _freq = value;
                OnPropertyChanged(nameof(Freq));
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

        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private bool _isDosing;
        public bool IsDosing
        {
            get
            {
                return _isDosing;
            }
            set
            {
                _isDosing = value;
                OnPropertyChanged(nameof(IsDosing));
            }
        }

        #region MyRegion

        private bool _alarmStatus;
        private bool _onStatus;
        private bool _automatMode;
        private bool _manualMode;
        private bool _manualDosingMode;

        public string Prefix { get; set; }
        public string FreqPcay { get; set; }
        public string ModePcy { get; set; }
        public string ManualPcy { get; set; }
        public string ManualDosingPcy { get; set; }
        public string OnPcy { get; set; }
        public string AlarmPcy { get; set; }
        public string StartPcx { get; set; }
        public string StopPcx { get; set; }
        public int ValueNumMech { get; set; }

        private bool _rotate;
        public bool Rotate
        {
            get { return _rotate; }
            set
            {
                if (value)
                {
                    img_Alarm.Source =
                        new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Pump1_Alarm.png", UriKind.Relative));
                    img_On.Source =
                        new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Pump1_ON.png", UriKind.Relative));
                }
                else
                {
                    img_Alarm.Source =
                    new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs_Redesign/img_Pump_2_Alarm.png", UriKind.Relative));
                    img_On.Source =
                        new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs_Redesign/img_Pump_2_ON.png", UriKind.Relative));
                }
                _rotate = value;
            }
        }

        private int _angle;
        public int Angle
        {
            get { return _angle; }
            set
            {
                ValveGrid.RenderTransform = new RotateTransform(value, 17.5, 17.5);
                _angle = value;
            }
        }
        private Position _modepos;
        public Position ModePos
        {
            get { return _modepos; }
            set
            {
                if (value == Position.LeftUp)
                    lbl_mode.Margin = new Thickness(-40, -30, 0, 0);
                if (value == Position.Up)
                    lbl_mode.Margin = new Thickness(0, -40, 0, 0);
                if (value == Position.RightUp)
                    lbl_mode.Margin = new Thickness(40, -40, 0, 0);
                if (value == Position.Left)
                    lbl_mode.Margin = new Thickness(-40, 10, 0, 0);
                if (value == Position.Right)
                    lbl_mode.Margin = new Thickness(40, 10, 0, 0);
                if (value == Position.LeftDown)
                    lbl_mode.Margin = new Thickness(-40, 25, 0, 0);
                if (value == Position.Down)
                    lbl_mode.Margin = new Thickness(0, 40, 0, 0);
                if (value == Position.RightDown)
                    lbl_mode.Margin = new Thickness(40, 25, 0, 0);

                _modepos = value;
            }
        }

        private Position _modeDosingPos;
        public Position ModeDosingPos
        {
            get { return _modeDosingPos; }
            set
            {
                if (value == Position.LeftUp)
                    LblModeDosing.Margin = new Thickness(-40, -30, 0, 0);
                if (value == Position.Up)
                    LblModeDosing.Margin = new Thickness(0, -40, 0, 0);
                if (value == Position.RightUp)
                    LblModeDosing.Margin = new Thickness(40, -40, 0, 0);
                if (value == Position.Left)
                    LblModeDosing.Margin = new Thickness(-40, 10, 0, 0);
                if (value == Position.Right)
                    LblModeDosing.Margin = new Thickness(40, 10, 0, 0);
                if (value == Position.LeftDown)
                    LblModeDosing.Margin = new Thickness(-40, 25, 0, 0);
                if (value == Position.Down)
                    LblModeDosing.Margin = new Thickness(0, 40, 0, 0);
                if (value == Position.RightDown)
                    LblModeDosing.Margin = new Thickness(40, 25, 0, 0);

                _modeDosingPos = value;
            }
        }

        private Position _namepos;
        public Position NamePos
        {
            get { return _namepos; }
            set
            {
                if (value == Position.LeftUp)
                {
                    tbc_name.Margin = new Thickness(0, 17, 42, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Right;
                    tbc_name.TextAlignment = TextAlignment.Right;
                }
                if (value == Position.Up)
                {
                    tbc_name.Margin = new Thickness(0, 14, 0, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Center;
                    tbc_name.TextAlignment = TextAlignment.Center;
                }
                if (value == Position.RightUp)
                {
                    tbc_name.Margin = new Thickness(55, 20, 0, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Left;
                    tbc_name.TextAlignment = TextAlignment.Left;
                }
                if (value == Position.Left)
                {
                    tbc_name.Margin = new Thickness(0, 35, 60, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Right;
                    tbc_name.TextAlignment = TextAlignment.Right;
                }
                if (value == Position.Right)
                {
                    tbc_name.Margin = new Thickness(55, 45, 0, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Left;
                    tbc_name.TextAlignment = TextAlignment.Left;
                }
                if (value == Position.LeftDown)
                {
                    tbc_name.Margin = new Thickness(0, 50, 55, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Right;
                    tbc_name.TextAlignment = TextAlignment.Right;
                }
                if (value == Position.Down)
                {
                    tbc_name.Margin = new Thickness(0, 55, 0, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Center;
                    tbc_name.TextAlignment = TextAlignment.Center;
                }
                if (value == Position.RightDown)
                {
                    tbc_name.Margin = new Thickness(55, 55, 0, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Left;
                    tbc_name.TextAlignment = TextAlignment.Left;
                }
                _namepos = value;
            }
        }

        private ElGate.Position _popupposition;
        public ElGate.Position PopupPosition
        {
            get { return _popupposition; }
            set
            {
                if (value == ElGate.Position.Left)
                    PopupObject.Placement = PlacementMode.Left;
                if (value == ElGate.Position.Right)
                    PopupObject.Placement = PlacementMode.Right;
                if (value == ElGate.Position.Up)
                    PopupObject.Placement = PlacementMode.Top;
                if (value == ElGate.Position.Down)
                    PopupObject.Placement = PlacementMode.Bottom;

                _popupposition = value;
            }
        }

        private string _nameObject;
        public string NameObject
        {
            get { return _nameObject; }
            set
            {
                tbc_name.Text = value;
                TxtPopupName.Text = "Насос " + value;
                _nameObject = value;
            }
        }

        #endregion

        public ElPump()
        {
            InitializeComponent();

            HideImages();
        }

        public void Subscribe()
        {
            CreateSubscription();
        }

        public void Unsubscribe()
        {
        }

        public void Initialize(OpcServer.OpcList opcName, int numStation = 0)
        {
            _opcName = opcName;
            _numStation = numStation;

            DataContext = this;
        }
        private void CreateSubscription()
        {
            IndexMethod();
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var alarmItem = new OpcMonitoredItem(_opc.cl.GetNode(AlarmPcy), OpcAttribute.Value);
            alarmItem.DataChangeReceived += HandleAlarmStatusChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(alarmItem);
            var onItem = new OpcMonitoredItem(_opc.cl.GetNode(OnPcy), OpcAttribute.Value);
            onItem.DataChangeReceived += HandleKmStatusChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(onItem);
            var modeAutomat = new OpcMonitoredItem(_opc.cl.GetNode(ModePcy), OpcAttribute.Value);
            modeAutomat.DataChangeReceived += HandleAutomatChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeAutomat);
            var modeManual = new OpcMonitoredItem(_opc.cl.GetNode(ManualPcy), OpcAttribute.Value);
            modeManual.DataChangeReceived += HandleManualChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeManual);
            if (IsDosing)
            {
                var modeManualDosingItem = new OpcMonitoredItem(_opc.cl.GetNode(ManualDosingPcy), OpcAttribute.Value);
                modeManualDosingItem.DataChangeReceived += HandleManualDosingChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeManualDosingItem);
            }
            if (FreqPcay == null)
                VisFreq = Visibility.Collapsed;
            else
            {
                VisFreq = Visibility.Visible;

                var freqItem = new OpcMonitoredItem(_opc.cl.GetNode(FreqPcay), OpcAttribute.Value);
                freqItem.DataChangeReceived += HandleFreqChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(freqItem);
            }
        }

        private void HandleFreqChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                Freq = Math.Round(double.Parse(e.Item.Value.ToString()), 2).ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {

            }
        }

        private void HandleManualChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _manualMode = bool.Parse(e.Item.Value.ToString());
                VisMode();
            }
            catch (Exception)
            {

            }
        }

        private void HandleManualDosingChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _manualDosingMode = bool.Parse(e.Item.Value.ToString());
                VisMode();
            }
            catch
            {

            }
        }

        private void HandleAutomatChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _automatMode = bool.Parse(e.Item.Value.ToString());
                VisMode();
            }
            catch (Exception)
            {

            }
        }

        private void HandleKmStatusChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _onStatus = bool.Parse(e.Item.Value.ToString());
                VisStatus();
            }
            catch (Exception)
            {

            }
        }

        private void HandleAlarmStatusChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _alarmStatus = bool.Parse(e.Item.Value.ToString());
                VisStatus();
            }
            catch (Exception)
            {

            }
        }

        void HideImages()
        {
            VisAlarm = Visibility.Collapsed;
            VisOn = Visibility.Collapsed;
            ModeAutomat = true;
            Status = "- - - - -";
            Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC0C0C0"));
        }

        void VisStatus()
        {
            if (_alarmStatus)
            {
                VisAlarm = Visibility.Visible;
                Status = "Авария";
                Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFEB1B22"));
            }
            else
            {
                VisAlarm = Visibility.Hidden;
                if (_onStatus)
                {
                    VisOn = Visibility.Visible;
                    Status = "Включен";
                    Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF85FC84"));
                }
                else
                {
                    VisOn = Visibility.Hidden;
                    Status = "Остановлен";
                    Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC0C0C0"));
                }
            }
        }

        void VisMode()
        {
            if (_automatMode)
            {
                ModeAutomat = true;
                ModeManual = false;
                ModeManualDosing = false;
            }
            else
            {
                ModeAutomat = false;
                ModeManual = _manualMode;
                ModeManualDosing = _manualDosingMode;
            }
        }

        void IndexMethod()
        {
            if (Prefix != null)
            {
                if (OnPcy == null)
                    OnPcy = Prefix + ".DI_ON";
                if (ModePcy == null)
                    ModePcy = Prefix + ".gMode_Automat";
                if (ManualPcy == null)
                {
                    if (IsDosing)
                        ManualPcy = Prefix + ".gMode_Naladka";
                    else
                        ManualPcy = Prefix + ".gMode_Manual";
                }
                if (AlarmPcy == null)
                    AlarmPcy = Prefix + ".gb_ALARM"; 
                if (ManualDosingPcy == null)
                {
                    ManualDosingPcy = Prefix + ".gMode_Manual";
                }
            }

            StartPcx = "btn_Mech_Start";
            StopPcx = "btn_Mech_Stop";
        }

        private void Rect_OnMouseEnter(object sender, MouseEventArgs e)
        {
            rect_object.Opacity = 1;
        }

        private void Rect_OnMouseLeave(object sender, MouseEventArgs e)
        {
            rect_object.Opacity = 0;
        }

        private void ValveGrid_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopupObject.IsOpen = true;
            try
            {
                if (PopupObject.IsOpen)
                {
                    _opc.cl.WriteInt16(Static.NumMech, (short)ValueNumMech, out _err);
                    if (_err)
                        MessageBox.Show("Возможно запись не прошла.\nПроверьте OPC-сервер или соответствующий тег", "Предупреждение");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Нет связи с OPC-сервером!", "Ошибка");
            }
        }

        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source; 
            if (IsDosing)
                Methods.ButtonClick(btn, BtnManual, "btn_Mech_Naladka", true, TxtPopupName.Text + ". Режим работы - наладка");
            else
                Methods.ButtonClick(btn, BtnManual, "btn_Mech_Manual", true, TxtPopupName.Text + ". Режим работы - ручной");
        }

        private void BtnAutomat_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnAutomat, "btn_Mech_Automat", true, TxtPopupName.Text + ". Режим работы - автомат");
        }

        private void BtnManualDosing_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnManualDosing, "btn_Mech_Manual", true, TxtPopupName.Text + ". Режим работы - ручное дозирование");
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnStart, StartPcx, true, TxtPopupName.Text + ". Старт");
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnStop, StopPcx, true, TxtPopupName.Text + ". Стоп");
        }

        private void Lbl_param_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;

            Methods.SetParameter(LblParam, btn, _opcName, "Частота насоса " + _nameObject + ", Гц", 0, 50, FreqPcay, WindowSetParameter.ValueType.Real, PopupObject, 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
