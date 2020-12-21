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
using _2048_Rbu.Classes;
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
                    RectObject.Margin = new Thickness(25, 0, 0, 0);
                }

                MainGrid.Width = RectObject.Width + 40;

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
                    LblMode.Margin = new Thickness(100, 10, 0, 0);
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
                    ModePcy = Prefix + ".gMode_Automat";
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

        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnManual, ManualPcy, true);
            Methods.ButtonClick(btn, BtnManual, ModePcy, false);
        }

        private void BtnAutomat_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnAutomat, ModePcy, true);
            Methods.ButtonClick(btn, BtnAutomat, ManualPcy, false);
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
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
