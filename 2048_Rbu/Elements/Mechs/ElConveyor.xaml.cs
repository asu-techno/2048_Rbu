using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using Lib_2048.Classes;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Mechs
{
    /// <summary>
    /// Interaction logic for ElFan.xaml
    /// </summary>
    public partial class ElConveyor : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private bool _err;

        public enum Mech { Shnek, Conveyor, Skip };
        public enum Position { LeftUp, Up, RightUp, Left, Right, LeftDown, Down, RightDown };

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

        private bool _feedback;
        public bool Feedback
        {
            get
            {
                return _feedback;
            }
            set
            {
                _feedback = value;
                OnPropertyChanged(nameof(Feedback));
            }
        }

        private bool _dks;
        public bool DKS
        {
            get
            {
                return _dks;
            }
            set
            {
                _dks = value;
                OnPropertyChanged(nameof(DKS));
            }
        }

        private bool _podpor;
        public bool Podpor
        {
            get
            {
                return _podpor;
            }
            set
            {
                _podpor = value;
                OnPropertyChanged(nameof(Podpor));
            }
        }

        private bool _external;
        public bool External
        {
            get
            {
                return _external;
            }
            set
            {
                _external = value;
                OnPropertyChanged(nameof(External));
            }
        }

        private bool _power;
        public bool Power
        {
            get
            {
                return _power;
            }
            set
            {
                _power = value;
                OnPropertyChanged(nameof(Power));
            }
        }

        private bool _stop;
        public bool Stop
        {
            get
            {
                return _stop;
            }
            set
            {
                _stop = value;
                OnPropertyChanged(nameof(Stop));
            }
        }

        private bool _parkingMode;
        public bool ParkingMode
        {
            get
            {
                return _parkingMode;
            }
            set
            {
                _parkingMode = value;
                OnPropertyChanged(nameof(ParkingMode));
            }
        }

        private bool _parkingTime;
        public bool ParkingTime
        {
            get
            {
                return _parkingTime;
            }
            set
            {
                _parkingTime = value;
                OnPropertyChanged(nameof(ParkingTime));
            }
        }

        private bool _parkingSensor;
        public bool ParkingSensor
        {
            get
            {
                return _parkingSensor;
            }
            set
            {
                _parkingSensor = value;
                OnPropertyChanged(nameof(ParkingSensor));
            }
        }

        private Visibility _visAnalog;
        public Visibility VisAnalog
        {
            get
            {
                return _visAnalog;
            }
            set
            {
                _visAnalog = value;
                OnPropertyChanged(nameof(VisAnalog));
            }
        }

        private bool _visWheel;
        public bool VisWheel
        {
            get
            {
                return _visWheel;
            }
            set
            {
                _visWheel = value;
                OnPropertyChanged(nameof(VisWheel));
            }
        }

        private bool _visDop;
        public bool VisDop
        {
            get
            {
                return _visDop;
            }
            set
            {
                _visDop = value;
                OnPropertyChanged(nameof(VisDop));
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

        private string _freqAnalog;
        public string FreqAnalog
        {
            get
            {
                return _freqAnalog;
            }
            set
            {
                _freqAnalog = value;
                OnPropertyChanged(nameof(FreqAnalog));
            }
        }

        private string _setfreq;
        public string SetFreq
        {
            get
            {
                return _setfreq;
            }
            set
            {
                _setfreq = value;
                OnPropertyChanged(nameof(SetFreq));
            }
        }

        private string _apfreq;
        public string ApFreq
        {
            get
            {
                return _apfreq;
            }
            set
            {
                _apfreq = value;
                OnPropertyChanged(nameof(ApFreq));
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

        private string _startTime;
        public string StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }

        private string _stopTime;
        public string StopTime
        {
            get
            {
                return _stopTime;
            }
            set
            {
                _stopTime = value;
                OnPropertyChanged(nameof(StopTime));
            }
        }

        #region MyRegion

        private bool _alarmStatus;
        private bool _kmStatus;
        private bool _manualMode;
        private bool _automatMode;

        public string Prefix { get; set; }
        public string ModePcy { get; set; }
        public string ManualPcy { get; set; }
        public string KmPcy { get; set; }
        public string AlarmPcy { get; set; }
        public bool IsWheel { get; set; }
        public int ValueNumMech { get; set; }

        private Mech _typeMech;
        public Mech TypeMech
        {
            get { return _typeMech; }
            set
            {
                if (value == Mech.Shnek)
                {
                    ImgKm.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Shnek_On.png", UriKind.Relative));
                    ImgAlarm.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Shnek_Alarm.png", UriKind.Relative));
                    ImgKm.Width = ImgAlarm.Width = 75;
                    ImgKm.Height = ImgAlarm.Height = 17;
                }
                if (value == Mech.Conveyor)
                {
                    ImgKm.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Conv_On.png", UriKind.Relative));
                    ImgAlarm.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Conv_Alarm.png", UriKind.Relative));
                    ImgKm.Width = ImgAlarm.Width = 673;
                    ImgKm.Height = ImgAlarm.Height = 13;
                }
                RectObject.Width = ImgKm.Width + 6;
                RectObject.Height = ImgKm.Height + 4;

                if (value == Mech.Skip)
                {
                    ImgKm.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Skip_On.png", UriKind.Relative));
                    ImgAlarm.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Skip_Alarm.png", UriKind.Relative));
                    ImgKm.Width = ImgAlarm.Width = 258;
                    ImgKm.Height = ImgAlarm.Height = 152;
                    RectObject.RenderTransform = new RotateTransform(30, 17.5, 17.5);
                    RectObject.Width = ImgKm.Width + 40;
                    RectObject.Height = 20;
                    RectObject.VerticalAlignment = VerticalAlignment.Top;
                    RectObject.HorizontalAlignment = HorizontalAlignment.Left;
                    RectObject.Margin = new Thickness(0, 0, 0, 0);
                }

                MainGrid.Width = RectObject.Width + 40;
                MainGrid.Width = RectObject.Width + 10;

                _typeMech = value;
            }
        }

        private Position _modepos;
        public Position ModePos
        {
            get { return _modepos; }
            set
            {
                if (value == Position.LeftUp)
                    LblMode.Margin = new Thickness(-40, -30, 0, 0);
                if (value == Position.Up)
                    LblMode.Margin = new Thickness(0, -40, 0, 0);
                if (value == Position.RightUp)
                    LblMode.Margin = new Thickness(90, 10, 0, 0);
                if (value == Position.Left)
                    LblMode.Margin = new Thickness(2, 35, 0, 0);
                if (value == Position.Right)
                    LblMode.Margin = new Thickness(0, 17, 0, 0);
                if (value == Position.LeftDown)
                    LblMode.Margin = new Thickness(75, 30, 0, 0);
                if (value == Position.Down)
                    LblMode.Margin = new Thickness(0, 44, 0, 0);
                if (value == Position.RightDown)
                    LblMode.Margin = new Thickness(40, 25, 0, 0);

                _modepos = value;
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
                    TbcName.Margin = new Thickness(25, 30, 0, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Left;
                    TbcName.TextAlignment = TextAlignment.Left;
                }
                if (value == Position.Up)
                {
                    TbcName.Margin = new Thickness(0, 5, 140, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Center;
                    TbcName.TextAlignment = TextAlignment.Center;
                }
                if (value == Position.RightUp)
                {
                    TbcName.Margin = new Thickness(0, 0, 16, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Right;
                    TbcName.TextAlignment = TextAlignment.Right;
                }
                if (value == Position.Left)
                {
                    TbcName.Margin = new Thickness(38, 26, 0, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Left;
                    TbcName.TextAlignment = TextAlignment.Left;
                }
                if (value == Position.Right)
                {
                    TbcName.Margin = new Thickness(0, 0, 12, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Right;
                    TbcName.TextAlignment = TextAlignment.Right;
                }
                if (value == Position.LeftDown)
                {
                    TbcName.Margin = new Thickness(45, 25, 0, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Left;
                    TbcName.TextAlignment = TextAlignment.Left;
                }
                if (value == Position.Down)
                {
                    TbcName.Margin = new Thickness(0, 55, 0, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Center;
                    TbcName.TextAlignment = TextAlignment.Center;
                }
                if (value == Position.RightDown)
                {
                    TbcName.Margin = new Thickness(0, 25, 45, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Right;
                    TbcName.TextAlignment = TextAlignment.Right;
                }
                _namepos = value;
            }
        }


        private string _nameObject;
        public string NameObject
        {
            get { return _nameObject; }
            set
            {
                TbcName.Text = value;
                if (_typeMech != Mech.Shnek)
                    TxtPopupName.Text = "Транспортер " + value;
                else
                    TxtPopupName.Text = "Шнек " + value;
                _nameObject = value;
            }
        }

        public string StartPcx { get; set; }
        public string StopPcx { get; set; }

        private Position _popupposition;
        public Position PopupPosition
        {
            get { return _popupposition; }
            set
            {
                if (value == Position.Left)
                    PopupObject.Placement = PlacementMode.Left;
                if (value == Position.Right)
                    PopupObject.Placement = PlacementMode.Right;
                if (value == Position.Up)
                    PopupObject.Placement = PlacementMode.Top;
                if (value == Position.Down)
                    PopupObject.Placement = PlacementMode.Bottom;

                _popupposition = value;
            }
        }

        #endregion

        public ElConveyor()
        {
            InitializeComponent();

            HideImages();
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
            IndexMethod();
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var alarmItem = new OpcMonitoredItem(_opc.cl.GetNode(AlarmPcy), OpcAttribute.Value);
            alarmItem.DataChangeReceived += HandleAlarmStatusChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(alarmItem);
            var kmItem = new OpcMonitoredItem(_opc.cl.GetNode(KmPcy), OpcAttribute.Value);
            kmItem.DataChangeReceived += HandleKmStatusChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(kmItem);
            var modeAutomat = new OpcMonitoredItem(_opc.cl.GetNode(ModePcy), OpcAttribute.Value);
            modeAutomat.DataChangeReceived += HandleAutomatChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeAutomat);
            var modeManual = new OpcMonitoredItem(_opc.cl.GetNode(ManualPcy), OpcAttribute.Value);
            modeManual.DataChangeReceived += HandleManualChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeManual);
        }

        void HideImages()
        {
            VisAlarm = Visibility.Hidden;
            VisOn = Visibility.Hidden;
            ModeAutomat = true;
            Status = "- - - - -";
            Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC0C0C0"));
        }

        private void HandleManualChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _manualMode = bool.Parse(e.Item.Value.ToString());
            VisMode();
        }

        private void HandleAutomatChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _automatMode = bool.Parse(e.Item.Value.ToString());
            VisMode();
        }

        private void HandleKmStatusChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _kmStatus = bool.Parse(e.Item.Value.ToString());
            VisStatus();
        }

        private void HandleAlarmStatusChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _alarmStatus = bool.Parse(e.Item.Value.ToString());
            VisStatus();
        }

        //void StatePopup()
        //{
        //    Feedback = _opc.cl.ReadBool("gb_Mech_Alarm_Feedback", out _err);
        //    DKS = _opc.cl.ReadBool("gb_Mech_Alarm_DKS", out _err);
        //    Podpor = _opc.cl.ReadBool("gb_Mech_Alarm_Podpor", out _err);
        //    External = _opc.cl.ReadBool("gb_Mech_Alarm_External", out _err);
        //    Power = _opc.cl.ReadBool("gb_Mech_Alarm_Power", out _err);
        //    Stop = _opc.cl.ReadBool("gb_Mech_Alarm_Stop", out _err);
        //    ParkingMode = _opc.cl.ReadBool("gb_Mech_Parking", out _err);
        //    ParkingSensor = _opc.cl.ReadBool("gb_Mech_ParkingSensor", out _err);
        //    ParkingTime = _opc.cl.ReadBool("gb_Mech_Alarm_ParkingTime", out _err);

        //    FreqAnalog = _opc.cl.ReadReal("gr_Mech_Percent", out _err).ToString("F1");
        //    Freq = _opc.cl.ReadReal("gr_Mech_DKS_Freq", out _err).ToString("F2");
        //    SetFreq = _opc.cl.ReadReal("gr_Mech_DKS_Freq_Nominal", out _err).ToString("F2");
        //    ApFreq = _opc.cl.ReadReal("gr_Mech_DKS_Freq_Deadband", out _err).ToString("F0");
        //    StartTime = _opc.cl.ReadReal(_prefix + ".StartTime", out _err).ToString("F0");
        //    StopTime = _opc.cl.ReadReal(_prefix + ".StopTime", out _err).ToString("F0");
        //}

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
                if (_kmStatus)
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
            }
            else
            {
                ModeAutomat = false;
                ModeManual = _manualMode;
            }
        }
        void IndexMethod()
        {
            if (Prefix != null)
            {
                if (KmPcy == null)
                    KmPcy = Prefix + ".DI_ON";
                if (ModePcy == null)
                    ModePcy = Prefix + ".DI_Service";
                if (ManualPcy == null)
                    ManualPcy = ".gMode_Manual";
                if (AlarmPcy == null)
                    AlarmPcy = Prefix + ".gb_ALARM";
            }

            StartPcx = "btn_Mech_Start";
            StopPcx = "btn_Mech_Stop";
        }

        private void Rect_OnMouseEnter(object sender, MouseEventArgs e)
        {
            RectObject.Opacity = 1;
        }

        private void Rect_OnMouseLeave(object sender, MouseEventArgs e)
        {
            RectObject.Opacity = 0;
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

        //private void BtnManual_Click(object sender, RoutedEventArgs e)
        //{
        //    object btn = e.Source;
        //    Methods.ButtonClick(btn, BtnManual, _manualPcy, true);
        //    Methods.ButtonClick(btn, BtnManual, _modePcy, false);
        //}

        //private void BtnAutomat_Click(object sender, RoutedEventArgs e)
        //{
        //    object btn = e.Source;
        //    Methods.ButtonClick(btn, BtnAutomat, _modePcy, true);
        //    Methods.ButtonClick(btn, BtnAutomat, _manualPcy, false);
        //}

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

        //private void BtnAway_Click(object sender, RoutedEventArgs e)
        //{
        //    object btn = e.Source;
        //    var temp = _opc.cl.ReadReal("gr_Mech_Percent", out _err) >= 0.5 ? _opc.cl.ReadReal("gr_Mech_Percent", out _err) - 0.5 : 0;
        //    Methods.ButtonClick(btn, BtnAway, "gr_Mech_Percent", temp, TxtPopupName.Text + ". Уменьшение производительности до " + temp + " %");
        //}

        //private void BtnAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    object btn = e.Source;
        //    var temp = _opc.cl.ReadReal("gr_Mech_Percent", out _err) <= 99.5 ? _opc.cl.ReadReal("gr_Mech_Percent", out _err) + 0.5 : 100;
        //    Methods.ButtonClick(btn, BtnAdd, "gr_Mech_Percent", temp, TxtPopupName.Text + ". Увеличение производительности до " + temp + " %");
        //}

        //private void BtnParking_Click(object sender, RoutedEventArgs e)
        //{
        //    object btn = e.Source;

        //    if (!ParkingMode)
        //        Methods.ButtonClick(btn, BtnParking, "gb_Mech_Parking", true, TxtPopupName.Text + ". Парковка");
        //    else
        //        Methods.ButtonClick(btn, BtnParking, "gb_Mech_Parking", false, TxtPopupName.Text + ". Отмена парковки");
        //}

        private void BtnVisDop_Click(object sender, RoutedEventArgs e)
        {
            if (!_visDop)
                VisDop = true;
            else
                VisDop = false;
        }

        private void PopupObject_OnClosed(object sender, EventArgs e)
        {
            VisDop = false;
        }

        //private void LblFreqAnalog_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    object btn = e.Source;

        //    Methods.SetParameter(LblFreqAnalog, btn, _opcName, TxtPopupName.Text + ". Производительность, %", 0, 100, "gr_Mech_Percent", "Real", PopupObject, 0, 1);
        //}

        //private void LblSetFreq_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    object btn = e.Source;

        //    Methods.SetParameter(LblSetFreq, btn, _opcName, TxtPopupName.Text + ". Номинальная частота, Гц", 0, 100, "gr_Mech_DKS_Freq_Nominal", "Real", PopupObject, 0, 2);
        //}

        //private void LblApFreq_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    object btn = e.Source;

        //    Methods.SetParameter(LblApFreq, btn, _opcName, TxtPopupName.Text + ". Допуск по оборотам, %", 0, 100, "gr_Mech_DKS_Freq_Deadband", "Real", PopupObject, 0, 0);
        //}

        //private void LblStartTime_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    object btn = e.Source;

        //    Methods.SetParameter(LblStart, btn, _opcName, TxtPopupName.Text + ". Время запуска, с", 0, 500, _prefix + ".StartTime", "Real", PopupObject, 0, 0);
        //}

        //private void LblStopTime_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    object btn = e.Source;

        //    Methods.SetParameter(LblStop, btn, _opcName, TxtPopupName.Text + ". Время останова, с", 0, 500, _prefix + ".StopTime", "Real", PopupObject, 0, 0);
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
