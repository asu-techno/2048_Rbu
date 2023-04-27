using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using _2048_Rbu.Classes;
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

        #region Property
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

        private int _openState;
        public int OpenState
        {
            get
            {
                return _openState;
            }
            set
            {
                _openState = value;
                OnPropertyChanged(nameof(OpenState));
            }
        }

        private bool _halfOpenState;
        public bool HalfOpenState
        {
            get
            {
                return _halfOpenState;
            }
            set
            {
                _halfOpenState = value;
                OnPropertyChanged(nameof(HalfOpenState));
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

        private bool _isUnload;
        public bool IsUnload
        {
            get { return _isUnload; }
            set
            {
                if (value)
                {
                    ImgClose.Source = (DrawingImage)Resources["im_Big_Zadv_Closed"];
                    ImgOpen.Source = (DrawingImage)Resources["im_Big_Zadv_Opened_100"];
                    ImgAlarm.Source = (DrawingImage)Resources["im_Big_Zadv_Alarm"];
                    //ImgClose.Width = ImgAlarm.Width = ImgOpen.Width = 126;
                    //ImgClose.Height = ImgAlarm.Height = ImgOpen.Height = 17;
                    //RectObject.Width = ImgOpen.Width;
                }

                _isUnload = value;
                OnPropertyChanged(nameof(IsUnload));
            }
        }

        #endregion

        #region MyRegion

        private bool _alarmStatus;
        private bool _openStatus;
        private bool _closeStatus;
        private bool _manualMode;
        private bool _manualDosingMode;
        private bool _automatMode;
        private double _percentOpen;

        public string Prefix { get; set; }
        public string ModePcy { get; set; }
        public string ManualPcy { get; set; }
        public string ManualDosingPcy { get; set; }
        public string OpenPcy { get; set; }
        public string ClosePcy { get; set; }
        public string AlarmPcy { get; set; }
        public string OpenPcx { get; set; }
        public string ClosePcx { get; set; }
        public int ValueNumMech { get; set; }
        public string AerationName { get; set; }

        private bool _isAeration;
        public bool IsAeration
        {
            get { return _isAeration; }
            set
            {
                if (value)
                {
                    //ImgClose.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs_Redesign/img_Aerator_Off.png", UriKind.Relative));
                    //ImgOpen.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs_Redesign/img_Aerator_ON.png", UriKind.Relative));
                    //ImgAlarm.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Mechs_Redesign/img_Aerator_AL.png", UriKind.Relative));
                    ImgClose.Source = (DrawingImage)Resources["im_Zadv_Aero_Closed"];
                    ImgOpen.Source = (DrawingImage)Resources["im_Zadv_Aero_Opened"];
                    ImgAlarm.Source = (DrawingImage)Resources["im_Zadv_Aero_Alarm"];
                    ImgOpen.Width = ImgClose.Width = ImgAlarm.Width = 126;
                    ImgOpen.Height = ImgClose.Height = ImgAlarm.Height = 17;
                    RectObject.Width = ImgOpen.Width;
                    RectObject.Margin = new Thickness(0, 0, 0, 0);
                    RectObject.ToolTip = "Удерживайте ПКМ для активации во время работы узла";
                }

                _isAeration = value;
            }
        }

        private Position _modepos;
        public Position ModePos
        {
            get { return _modepos; }
            set
            {
                if (value == Position.LeftUp)
                    LblMode.Margin = new Thickness(97, -25, 0, 0);
                if (value == Position.Up)
                    LblMode.Margin = new Thickness(0, -40, 0, 0);
                if (value == Position.RightUp)
                    LblMode.Margin = new Thickness(70, -25, 0, 0);
                if (value == Position.Left)
                    LblMode.Margin = new Thickness(44, 25, 0, 0);
                if (value == Position.Right)
                    LblMode.Margin = new Thickness(45, 0, 0, 0);
                if (value == Position.LeftDown)
                    LblMode.Margin = new Thickness(-32, 25, 0, 0);
                if (value == Position.Down)
                    LblMode.Margin = new Thickness(0, 35, 0, 0);
                if (value == Position.RightDown)
                    LblMode.Margin = new Thickness(60, 25, 0, 0);

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
                    LblModeDosing.Margin = new Thickness(95, -23, 0, 0);
                if (value == Position.Up)
                    LblModeDosing.Margin = new Thickness(0, -38, 0, 0);
                if (value == Position.RightUp)
                    LblModeDosing.Margin = new Thickness(68, -23, 0, 0);
                if (value == Position.Left)
                    LblModeDosing.Margin = new Thickness(43, 23, 0, 0);
                if (value == Position.Right)
                    LblModeDosing.Margin = new Thickness(43, 0, 0, 0);
                if (value == Position.LeftDown)
                    LblModeDosing.Margin = new Thickness(-30, 23, 0, 0);
                if (value == Position.Down)
                    LblModeDosing.Margin = new Thickness(0, 33, 0, 0);
                if (value == Position.RightDown)
                    LblModeDosing.Margin = new Thickness(58, 23, 0, 0);

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
                    TbcName.Margin = new Thickness(0, 0, 140, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Right;
                    TbcName.TextAlignment = TextAlignment.Right;
                }

                if (value == Position.Right)
                {
                    TbcName.Margin = new Thickness(118, 24, 0, 0);
                    TbcName.HorizontalAlignment = HorizontalAlignment.Left;
                    TbcName.TextAlignment = TextAlignment.Left;
                }

                if (value == Position.LeftDown)
                {
                    TbcName.Margin = new Thickness(0, 22, 69, 0);
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
                    TbcName.Margin = new Thickness(70, 22, 0, 0);
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
                TxtPopupName.Text = AerationName == null ? "Задвижка " + value : AerationName;

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
                    RectObject.HorizontalAlignment = HorizontalAlignment.Right;
                    RectObject.Margin = new Thickness(0, 0, 39, 0);
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

        private double _rotateAngle = 0.0;
        public double RotateAngle
        {
            get { return _rotateAngle; }
            set
            {
                _rotateAngle = value;
                RotateTransform rotateTransform = new RotateTransform(_rotateAngle, 25.5, 8.5);
                ObjectGrid.RenderTransform = rotateTransform;
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
            if (IsDosing)
            {
                var modeManualDosingItem = new OpcMonitoredItem(_opc.cl.GetNode(ManualDosingPcy), OpcAttribute.Value);
                modeManualDosingItem.DataChangeReceived += HandleManualDosingChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeManualDosingItem);
            }
            if (IsUnload)
            {
                var percentOpenItem = new OpcMonitoredItem(_opc.cl.GetNode("gr_Valve_PercentOpen"), OpcAttribute.Value);
                percentOpenItem.DataChangeReceived += HandlePercentChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(percentOpenItem);
            }
        }

        void HideImages()
        {
            VisAlarm = Visibility.Collapsed;
            VisOpen = Visibility.Collapsed;
            VisClose = Visibility.Collapsed;
            OpenState = 0;
            HalfOpenState = false;
            ModeAutomat = true;
            Status = "- - - - -";
            Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC0C0C0"));
        }

        #region HandleMethod
        private void HandleManualChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _manualMode = bool.Parse(e.Item.Value.ToString());
                VisMode();
            }
            catch
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
            catch
            {

            }
        }

        private void HandleCloseStatusChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _closeStatus = bool.Parse(e.Item.Value.ToString());
                VisStatus();
            }
            catch
            {

            }
        }

        private void HandleOpenStatusChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _openStatus = bool.Parse(e.Item.Value.ToString());
                VisStatus();
            }
            catch
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
            catch
            {

            }
        }

        private void HandlePercentChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _percentOpen = double.Parse(e.Item.Value.ToString());
                VisStatus();
            }
            catch
            {

            }
        }

        #endregion

        void VisStatus()
        {
            if (_alarmStatus)
            {
                VisAlarm = Visibility.Visible;
                VisOpen = Visibility.Collapsed;
                VisClose = Visibility.Collapsed;
                HalfOpenState = false;
                OpenState = 0;
                Status = "Авария";
                Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFEB1B22"));
            }
            else
            {
                VisAlarm = Visibility.Collapsed;
                if (!_openStatus && !_closeStatus)
                {
                    VisOpen = Visibility.Collapsed;
                    VisClose = Visibility.Collapsed;

                    if (IsUnload)
                    {
                        if (_percentOpen <= 0)
                            OpenState = 0;
                        if (_percentOpen > 0 && _percentOpen < 20)
                            OpenState = 1;
                        if (_percentOpen >= 20 && _percentOpen < 40)
                            OpenState = 2;
                        if (_percentOpen >= 40 && _percentOpen < 60)
                            OpenState = 3;
                        if (_percentOpen >= 60 && _percentOpen < 80)
                            OpenState = 4;
                        if (_percentOpen >= 80)
                            OpenState = 5;
                    }
                    if (OpenState == 0)
                    {
                        HalfOpenState = false;
                        Status = "- - - - -";
                        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF00"));
                    }
                    else
                    {
                        HalfOpenState = true;
                        Status = "Приоткрыта";
                        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF2DE6C3"));
                    }
                }
                else
                {
                    HalfOpenState = false;
                    OpenState = 0;
                    if (_openStatus)
                    {
                        VisOpen = Visibility.Visible;
                        VisClose = Visibility.Collapsed;
                        Status = "Открыта";
                        Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF85fc84"));
                    }
                    if (_closeStatus)
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
                if (ModePcy == null)
                    ModePcy = Prefix + ".gMode_Automat";
                if (ManualPcy == null)
                {
                    if (IsDosing)
                        ManualPcy = Prefix + ".gMode_Naladka";
                    else
                        ManualPcy = Prefix + ".gMode_Manual";
                }
                if (OpenPcy == null)
                    OpenPcy = Prefix + ".DI_Opened";
                if (ClosePcy == null)
                    ClosePcy = Prefix + ".DI_Closed";
                if (AlarmPcy == null)
                    AlarmPcy = Prefix + ".gb_ALARM";
                if (ManualDosingPcy == null)
                {
                    ManualDosingPcy = Prefix + ".gMode_Manual";
                }
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
            if (IsDosing)
                Methods.ButtonClick(btn, BtnManual, "btn_Valve_Naladka", true, TxtPopupName.Text + ". Режим работы - наладка");
            else
                Methods.ButtonClick(btn, BtnManual, "btn_Valve_Manual", true, TxtPopupName.Text + ". Режим работы - ручной");
        }

        private void BtnAutomat_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnAutomat, "btn_Valve_Automat", true, TxtPopupName.Text + ". Режим работы - автомат");
        }

        private void BtnManualDosing_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnManualDosing, "btn_Valve_Manual", true, TxtPopupName.Text + ". Режим работы - ручное дозирование");
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnOpen, OpenPcx, true, TxtPopupName.Text + ". Открыть");
        }

        private void BtnOpen_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnOpenUnloadGate, "btn_V1_Open", true, TxtPopupName.Text + ". Открыть");
        }

        private void BtnOpen_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnOpenUnloadGate, "btn_V1_Open", false);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnClose, ClosePcx, true, TxtPopupName.Text + ". Закрыть");
        }

        private void ValveGrid_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsAeration)
            {
                Methods.ButtonClick($"{Prefix}.Start_ByOperator", true, TxtPopupName.Text + ". Старт");
            }
        }

        private void ValveGrid_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsAeration)
            {
                Methods.ButtonClick($"{Prefix}.Start_ByOperator", false, TxtPopupName.Text + ". Стоп");
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
