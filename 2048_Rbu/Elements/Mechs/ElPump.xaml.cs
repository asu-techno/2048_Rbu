using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using _2048_Rbu.Classes;
using Lib_2048.Classes;
using _2048_Rbu.Interfaces;
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

        private Visibility _visKM;
        public Visibility VisKM
        {
            get
            {
                return _visKM;
            }
            set
            {
                _visKM = value;
                OnPropertyChanged(nameof(VisKM));
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

        #region MyRegion

        private bool _alarmStatus;
        private bool _kmStatus;
        private bool _automatMode;
        private bool _manualMode;

        public string Prefix { get; set; }
        public string FreqPCAY { get; set; }
        public string ModePcy { get; set; }
        public string ManualPcy { get; set; }
        public string KMPCY { get; set; }
        public string AlarmPcy { get; set; }
        public string StartPcx { get; set; }
        public string StopPcx { get; set; }
        public string NumMech { get; set; }
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
                    img_KM.Source =
                        new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Pump1_ON.png", UriKind.Relative));
                }
                else
                {
                    img_Alarm.Source =
                    new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Pump_Alarm.png", UriKind.Relative));
                    img_KM.Source =
                        new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_Pump_ON.png", UriKind.Relative));
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
                    tbc_name.Margin = new Thickness(0, 10, 0, 0);
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

        private string _nameObject;
        public string NameObject
        {
            get { return _nameObject; }
            set
            {
                tbc_name.Text = value;
                txt_popName.Text = value;
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
            var kmItem = new OpcMonitoredItem(_opc.cl.GetNode(KMPCY), OpcAttribute.Value);
            kmItem.DataChangeReceived += HandleKmStatusChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(kmItem);
            var modeAutomat = new OpcMonitoredItem(_opc.cl.GetNode(ModePcy), OpcAttribute.Value);
            modeAutomat.DataChangeReceived += HandleAutomatChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeAutomat);
            var modeManual = new OpcMonitoredItem(_opc.cl.GetNode(ManualPcy), OpcAttribute.Value);
            modeManual.DataChangeReceived += HandleManualChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeManual);

            if (FreqPCAY == null)
                VisFreq = Visibility.Collapsed;
            else
            {
                VisFreq = Visibility.Visible;

                var freqItem = new OpcMonitoredItem(_opc.cl.GetNode(FreqPCAY), OpcAttribute.Value);
                freqItem.DataChangeReceived += HandleFreqChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(freqItem);
            }
        }

        private void HandleFreqChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            Freq = Math.Round(double.Parse(e.Item.Value.ToString()), 2).ToString(CultureInfo.InvariantCulture);
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

        void HideImages()
        {
            VisAlarm = Visibility.Collapsed;
            VisKM = Visibility.Collapsed;
            ModeAutomat = true;
        }

        void VisStatus()
        {
            if (_alarmStatus)
            {
                VisAlarm = Visibility.Visible;
            }
            else
            {
                VisAlarm = Visibility.Collapsed;
                if (_kmStatus)
                    VisKM = Visibility.Visible;
                else
                    VisKM = Visibility.Collapsed;
            }
        }
        void VisMode()
        {
            if (_automatMode || (ModePcy == "" && !_manualMode))
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
                if (KMPCY == null)
                    KMPCY = Prefix + ".DI_ON";
                if (ModePcy == null)
                    ModePcy = Prefix + ".gMode_Automat";
                if (ManualPcy == null)
                    ManualPcy = Prefix + ".gMode_Manual";
                if (AlarmPcy == null)
                    AlarmPcy = Prefix + ".gb_ALARM";
                if (StartPcx == null)
                    StartPcx = Prefix + ".btn_Start";
                if (StopPcx == null)
                    StopPcx = Prefix + ".btn_Stop";
            }


            //if (Tag != null)
            //    _startPCX = Tag + ".btn_StartPump";
            //if (Tag != null)
            //    _stopPCX = Tag + ".btn_StopPump";

            if (NumMech != null)
            {
                StartPcx = "btn_Mech_Start";
                StopPcx = "btn_Mech_Stop";
                FreqPCAY = "gr_Mech_Hz";
            }
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
            //if (SelUser.GetInstance().GetSelUser().PermitManual && false)
            PopupObject.IsOpen = true;
            try
            {
                if (PopupObject.IsOpen)
                {
                    _opc.cl.WriteInt16(Static.NumMech, (short)ValueNumMech, out _err);
                    if (_err)
                        MessageBox.Show("Возможно запись не прошла.\nПроверьте OPC-сервер или соответствующий тег", "Предупреждение");
                    //StatePopup();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Нет связи с OPC-сервером!", "Ошибка");
            }
        }

        private void btnManual_Click(object sender, RoutedEventArgs e)
        {
            _opc.cl.WriteBool(ManualPcy, true, out _err);
            _opc.cl.WriteBool(ModePcy, false, out _err);
            //EventsBase.GetInstance().GetControlEvents(_opcName)?.AddEvent("Насос " + _nameObject + ". Режим работы - Ручной", SystemEventType.UserDoing);
        }

        private void btnAutomat_Click(object sender, RoutedEventArgs e)
        {
            _opc.cl.WriteBool(ModePcy, true, out _err);
            _opc.cl.WriteBool(ManualPcy, false, out _err);
            //EventsBase.GetInstance().GetControlEvents(_opcName)?.AddEvent("Насос " + _nameObject + ". Режим работы - Автомат", SystemEventType.UserDoing);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _opc.cl.WriteBool(StartPcx, true, out _err);
            //EventsBase.GetInstance().GetControlEvents(_opcName)?.AddEvent("Насос " + _nameObject + " - Пуск", SystemEventType.UserDoing);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _opc.cl.WriteBool(StopPcx, true, out _err);
            //EventsBase.GetInstance().GetControlEvents(_opcName)?.AddEvent("Насос " + _nameObject + " - Стоп", SystemEventType.UserDoing);
        }

        private void Lbl_param_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;

            Methods.SetParameter(lbl_param, btn, _opcName, "Частота насоса " + _nameObject + ", Гц", 0, 50, FreqPCAY, "Real", PopupObject, 0, 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
