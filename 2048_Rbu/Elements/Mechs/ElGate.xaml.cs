using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using _2048_Rbu.Classes;
using Lib_2048.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Mechs
{
    /// <summary>
    /// Interaction logic for ElGate.xaml
    /// </summary>
    public partial class ElGate : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private bool _err;
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

        private Visibility _visOpen;
        public Visibility VisOpen
        {
            get
            {
                return _visOpen;
            }
            set
            {
                _visOpen = value;
                OnPropertyChanged(nameof(VisOpen));
            }
        }

        private Visibility _visClose;
        public Visibility VisClose
        {
            get
            {
                return _visClose;
            }
            set
            {
                _visClose = value;
                OnPropertyChanged(nameof(VisClose));
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
        private bool _openStatus;
        private bool _closeStatus;
        private bool _manualMode;
        private bool _automatMode;

        public string Prefix { get; set; }
        public string ModePcy { get; set; }
        public string ManualPcy { get; set; }
        public string OpenPcy { get; set; }
        public string ClosePcy { get; set; }
        public string AlarmPcy { get; set; }
        public string OpenPcx { get; set; }
        public string ClosePcx { get; set; }
        public int ValueNumMech { get; set; }

        private bool _isUnload;
        public bool IssUnload
        {
            get { return _isUnload; }
            set
            {
                if (value)
                {
                    ImgClose.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_BigGate_Close.png", UriKind.Relative));
                    ImgOpen.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_BigGate_Open.png", UriKind.Relative));
                    ImgAlarm.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs/img_BigGate_Alarm.png", UriKind.Relative));
                    ImgOpen.Width = 130;
                    ImgClose.Margin = ImgAlarm.Margin = new Thickness(0, 0, 11, 0);
                    ImgOpen.Margin = new Thickness(38, 0, 0, 0);
                    ImgClose.Width = ImgAlarm.Width = 81;
                }

                RectObject.Width = ImgOpen.Width + 14;

                _isUnload = value;
            }
        }

        private Position _modepos;
        public Position ModePos
        {
            get { return _modepos; }
            set
            {
                if (value == Position.LeftUp)
                    LblMode.Margin = new Thickness(-25, -25, 0, 0);
                if (value == Position.Up)
                    LblMode.Margin = new Thickness(0, -40, 0, 0);
                if (value == Position.RightUp)
                    LblMode.Margin = new Thickness(25, -25, 0, 0);
                if (value == Position.Left)
                    LblMode.Margin = new Thickness(0, 24, 110, 0);
                if (value == Position.Right)
                    LblMode.Margin = new Thickness(45, 0, 0, 0);
                if (value == Position.LeftDown)
                    LblMode.Margin = new Thickness(-32, 25, 0, 0);
                if (value == Position.Down)
                    LblMode.Margin = new Thickness(0, 35, 0, 0);
                if (value == Position.RightDown)
                    LblMode.Margin = new Thickness(30, 25, 0, 0);

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
                    TbcName.Margin = new Thickness(0, 11, 52, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Right;
                    TbcName.TextAlignment = TextAlignment.Right;
                }

                if (value == Position.Up)
                {
                    TbcName.Margin = new Thickness(0, 20, 0, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Center;
                    TbcName.TextAlignment = TextAlignment.Center;
                }

                if (value == Position.RightUp)
                {
                    TbcName.Margin = new Thickness(55, 25, 0, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Left;
                    TbcName.TextAlignment = TextAlignment.Left;
                }

                if (value == Position.Left)
                {
                    TbcName.Margin = new Thickness(0, -5, 140, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Right;
                    TbcName.TextAlignment = TextAlignment.Right;
                }

                if (value == Position.Right)
                {
                    TbcName.Margin = new Thickness(45, 22, 0, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Left;
                    TbcName.TextAlignment = TextAlignment.Left;
                }

                if (value == Position.LeftDown)
                {
                    TbcName.Margin = new Thickness(0, 20, 100, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Right;
                    TbcName.TextAlignment = TextAlignment.Right;
                }

                if (value == Position.Down)
                {
                    TbcName.Margin = new Thickness(0, 60, 0, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Center;
                    TbcName.TextAlignment = TextAlignment.Center;
                }

                if (value == Position.RightDown)
                {
                    TbcName.Margin = new Thickness(100, 20, 0, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Left;
                    TbcName.TextAlignment = TextAlignment.Left;
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
                TxtPopupName.Text = "Задвижка " + value;

                _nameObject = value;
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

        public ElGate()
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
            var closeItem = new OpcMonitoredItem(_opc.cl.GetNode(ClosePcy), OpcAttribute.Value);
            closeItem.DataChangeReceived += HandleCloseStatusChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(closeItem);
            var openItem = new OpcMonitoredItem(_opc.cl.GetNode(OpenPcy), OpcAttribute.Value);
            openItem.DataChangeReceived += HandleOpenStatusChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(openItem);
            var modeAutomat = new OpcMonitoredItem(_opc.cl.GetNode(ModePcy), OpcAttribute.Value);
            modeAutomat.DataChangeReceived += HandleAutomatChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeAutomat);
            var modeManual = new OpcMonitoredItem(_opc.cl.GetNode(ManualPcy), OpcAttribute.Value);
            modeManual.DataChangeReceived += HandleManualChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeManual);
        }

        void HideImages()
        {
            VisAlarm = Visibility.Collapsed;
            VisOpen = Visibility.Collapsed;
            VisClose = Visibility.Collapsed;
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

        private void HandleCloseStatusChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _closeStatus = bool.Parse(e.Item.Value.ToString());
            VisStatus();
        }

        private void HandleOpenStatusChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _openStatus = bool.Parse(e.Item.Value.ToString());
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
                VisAlarm = Visibility.Collapsed;
                if (!_openStatus && !_closeStatus && OpenPcy != "")
                {
                    VisOpen = Visibility.Collapsed;
                    VisClose = Visibility.Collapsed;
                    Status = "- - - - -";
                    Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF00"));
                }
                else
                {
                    if (_openStatus || (OpenPcy == "" && !_closeStatus))
                    {
                        VisOpen = Visibility.Visible;
                        VisClose = Visibility.Collapsed;
                        Status = "Открыта";
                        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF85fc84"));
                    }
                    if (_closeStatus || (ClosePcy == "" && !_openStatus))
                    {
                        VisOpen = Visibility.Collapsed;
                        VisClose = Visibility.Visible;
                        Status = "Закрыта";
                        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC0C0C0"));
                    }
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
                if (ModePcy == null)
                    ModePcy = Prefix + ".gMode_Automat";
                if (ManualPcy == null)
                    ManualPcy = Prefix+".gMode_Manual";
                if (OpenPcy == null)
                    OpenPcy = Prefix + ".DI_Opened";
                if (ClosePcy == null)
                    ClosePcy = Prefix + ".DI_Closed";
                if (AlarmPcy == null)
                    AlarmPcy = Prefix + ".gb_ALARM";
            }

            OpenPcx = "btn_Valve_Open";
            ClosePcx = "btn_Valve_Close";
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

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnOpen, OpenPcx, true, TxtPopupName.Text + ". Открыть");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnClose, ClosePcx, true, TxtPopupName.Text + ". Закрыть");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
