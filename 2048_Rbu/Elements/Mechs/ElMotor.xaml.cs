using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using Lib_2048.Classes;
using AS_Library.Events.Classes;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;
using ServiceLib.Classes;

namespace _2048_Rbu.Elements.Mechs
{
    /// <summary>
    /// Interaction logic for el_SimpleValve.xaml
    /// </summary>
    public partial class ElMotor : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private bool _err;
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
        private bool _manualMode;
        private bool _automatMode;

        public string Prefix { get; set; }
        public string FreqPcay { get; set; }
        public string ModePcy { get; set; }
        public string ManualPcy { get; set; }
        public string KmPcy { get; set; }
        public string AlarmPcy { get; set; }
        public string StartPcx { get; set; }
        public string StopPcx { get; set; }
        public int ValueNumMech { get; set; }

        private int _angle;
        public int Angle
        {
            get { return _angle; }
            set
            {
                ObjectGrid.RenderTransform = new RotateTransform(value, 17.5, 17.5);
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
                    lbl_mode.Margin = new Thickness(0, 0, 62, 23);
                if (value == Position.Up)
                    lbl_mode.Margin = new Thickness(0, -50, 0, 0);
                if (value == Position.RightUp)
                    lbl_mode.Margin = new Thickness(62, 0, 0, 23);
                if (value == Position.Left)
                    lbl_mode.Margin = new Thickness(0, 0, 70, 0);
                if (value == Position.Right)
                    lbl_mode.Margin = new Thickness(70, 0, 0, 0);
                if (value == Position.LeftDown)
                    lbl_mode.Margin = new Thickness(0, 25, 62, 0);
                if (value == Position.Down)
                    lbl_mode.Margin = new Thickness(0, 30, 0, 0);
                if (value == Position.RightDown)
                    lbl_mode.Margin = new Thickness(62, 25, 0, 0);

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
                    tbc_name.FontSize = 13;
                    tbc_name.Margin = new Thickness(0, -3, 21, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Right;
                    tbc_name.TextAlignment = TextAlignment.Right;
                }
                if (value == Position.Up)
                {
                    tbc_name.Margin = new Thickness(-3, -3, 0, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Center;
                    tbc_name.TextAlignment = TextAlignment.Center;
                }
                if (value == Position.RightUp)
                {
                    tbc_name.FontSize = 13;
                    tbc_name.Margin = new Thickness(23, -3, 0, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Left;
                    tbc_name.TextAlignment = TextAlignment.Left;
                }
                if (value == Position.Left)
                {
                    tbc_name.Margin = new Thickness(0, 14, 65, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Right;
                    tbc_name.TextAlignment = TextAlignment.Right;
                }
                if (value == Position.Right)
                {
                    tbc_name.Margin = new Thickness(65, 14, 0, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Left;
                    tbc_name.TextAlignment = TextAlignment.Left;
                }
                if (value == Position.LeftDown)
                {
                    tbc_name.Margin = new Thickness(0, 25, 62, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Right;
                    tbc_name.TextAlignment = TextAlignment.Right;
                }
                if (value == Position.Down)
                {
                    tbc_name.Margin = new Thickness(0, 17, 5, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Center;
                    tbc_name.TextAlignment = TextAlignment.Center;
                }
                if (value == Position.RightDown)
                {
                    tbc_name.Margin = new Thickness(62, 25, 0, 0);
                    tbc_name.HorizontalAlignment = HorizontalAlignment.Left;
                    tbc_name.TextAlignment = TextAlignment.Left;
                }
                _namepos = value;
            }
        }

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

        private bool _rotate;
        public bool Rotate
        {
            get { return _rotate; }
            set
            {
                if (value)
                {
                    ObjectGrid.RenderTransformOrigin = new Point(0.5, 0.5);
                    ScaleTransform rotate = new ScaleTransform();
                    rotate.ScaleX = -1;
                    ObjectGrid.RenderTransform = rotate;
                }
                else
                {
                    ObjectGrid.RenderTransformOrigin = new Point(0.5, 0.5);
                    ScaleTransform rotate = new ScaleTransform();
                    rotate.ScaleX = 1;
                    ObjectGrid.RenderTransform = rotate;
                }
                _rotate = value;
            }
        }

        #endregion

        public ElMotor()
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

        void HideImages()
        {
            VisAlarm = Visibility.Hidden;
            VisKM = Visibility.Hidden;
            ModeAutomat = true;
            //Status = "- - - - -";
            //Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC0C0C0"));
        }

        void IndexMethod()
        {
            if (KmPcy == null)
                KmPcy = Prefix + ".DI_ON";
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

        private void HandleFreqChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            Freq = Math.Round(double.Parse(FreqPcay), 2).ToString(CultureInfo.InvariantCulture);
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

        private void btnManual_Click(object sender, RoutedEventArgs e)
        {
            _opc.cl.WriteBool(ManualPcy, true, out _err);
            _opc.cl.WriteBool(ModePcy, false, out _err);
            EventsBase.GetInstance().GetControlEvents(_opcName).AddEvent("Мешалка " + _nameObject + " - ручной режим", SystemEventType.UserDoing);
        }

        private void btnAutomat_Click(object sender, RoutedEventArgs e)
        {
            _opc.cl.WriteBool(ModePcy, true, out _err);
            _opc.cl.WriteBool(ManualPcy, false, out _err);
            EventsBase.GetInstance().GetControlEvents(_opcName).AddEvent("Мешалка " + _nameObject + " - режим автомат", SystemEventType.UserDoing);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _opc.cl.WriteBool(StartPcx, true, out _err);
            EventsBase.GetInstance().GetControlEvents(_opcName).AddEvent("Мешалка " + _nameObject + " - пуск", SystemEventType.UserDoing);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _opc.cl.WriteBool(StopPcx, true, out _err);
            EventsBase.GetInstance().GetControlEvents(_opcName).AddEvent("Мешалка " + _nameObject + " - стоп", SystemEventType.UserDoing);
        }

        private void Lbl_param_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;

            Methods.SetParameter(lbl_param, btn, _opcName, "Частота мешалки " + _nameObject + ", Гц", 0, 50, FreqPcay, "Real", PopupObject, 0, 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
