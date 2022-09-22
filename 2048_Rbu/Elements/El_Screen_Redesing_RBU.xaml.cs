using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Classes;
using AS_Library.Link;
using _2048_Rbu.Interfaces;
using Opc.UaFx;
using Opc.UaFx.Client;
using System.Windows.Threading;
using _2048_Rbu.Classes.ViewModel;
using _2048_Rbu.Elements.Control;
using _2048_Rbu.Elements.Indicators;
using _2048_Rbu.Windows;
using AsuBetonLibrary.Services;
using AsuBetonLibrary.Windows;
using NLog;
using _2048_Rbu.Windows.Reports;
using System.Threading.Tasks;
using ArchiverLibCore.Elements;
using AS_Library.Events.Classes;
using ServiceLibCore.Classes;
using RelayCommand = AS_Library.Classes.RelayCommand;

namespace _2048_Rbu.Elements
{
    public partial class El_Screen_Redesing_RBU : UserControl
    {

        OpcServer.OpcList _opcName;
        public static DispatcherTimer LinkTimer;
        private readonly List<IElementsUpdater> _elementList = new List<IElementsUpdater>();
        private List<ElContainer> _containerList = new List<ElContainer>();
        public ViewModelScreenRbu ViewModelScreenRbu;
        public delegate void EventHandlerLogin();
        public event EventHandlerLogin LoginClick;
        public delegate void EventHandlerUser();
        public event EventHandlerUser UserClick;
        private int _cycle;
        public ElArchiversViewModel ElArchiversViewModel { get; private set; }
        private event EventHandler<EventArgs> Closed;


        public El_Screen_Redesing_RBU()
        {
            InitializeComponent();
        }


        public void Initialize(Logger logger)
        {
            _opcName = OpcServer.OpcList.Rbu;

            ViewModelScreenRbu = new ViewModelScreenRbu(_opcName);
            DataContext = ViewModelScreenRbu;

            //ElArchiversViewModel = new ElArchiversViewModel();
            //Closed += ElArchiversViewModel.OnClosed;

            ViewModelScreenRbu.IsUpdating = true;

            #region Init

            foreach (var item in ElGrid.Children)
            {
                if (item.GetType() == typeof(Mechs.ElConveyor))
                {
                    var conveyor = (Mechs.ElConveyor)item;
                    conveyor.Initialize(_opcName);
                    _elementList.Add(conveyor);
                }

                if (item.GetType() == typeof(Mechs.ElGate))
                {
                    var gate = (Mechs.ElGate)item;
                    gate.Initialize(_opcName);
                    _elementList.Add(gate);
                }

                if (item.GetType() == typeof(Mechs.ElMixer))
                {
                    var mixer = (Mechs.ElMixer)item;
                    mixer.Initialize(_opcName);
                    _elementList.Add(mixer);
                }

                if (item.GetType() == typeof(Mechs.ElMotor))
                {
                    var motor = (Mechs.ElMotor)item;
                    motor.Initialize(_opcName);
                    _elementList.Add(motor);
                }

                if (item.GetType() == typeof(Mechs.ElPump))
                {
                    var pump = (Mechs.ElPump)item;
                    pump.Initialize(_opcName);
                    _elementList.Add(pump);
                }

                if (item.GetType() == typeof(Indicators.ElValueBox))
                {
                    var valuebox = (Indicators.ElValueBox)item;
                    valuebox.Initialize(_opcName);
                    _elementList.Add(valuebox);
                }

                if (item.GetType() == typeof(Indicators.ElValueBar))
                {
                    var valuebar = (Indicators.ElValueBar)item;
                    valuebar.Initialize(_opcName);
                    _elementList.Add(valuebar);
                }

                if (item.GetType() == typeof(Indicators.ElDone))
                {
                    var done = (Indicators.ElDone)item;
                    done.Initialize(_opcName);
                    _elementList.Add(done);
                }

                if (item.GetType() == typeof(Indicators.ElAnim))
                {
                    var anim = (Indicators.ElAnim)item;
                    anim.Initialize(_opcName);
                    _elementList.Add(anim);
                }

                if (item.GetType() == typeof(Indicators.ElWarning))
                {
                    var warning = (Indicators.ElWarning)item;
                    warning.Initialize(_opcName);
                    _elementList.Add(warning);
                }

                if (item.GetType() == typeof(Control.ElDosingWait))
                {
                    var dosingWait = (Control.ElDosingWait)item;
                    dosingWait.Initialize(_opcName);
                    _elementList.Add(dosingWait);
                }

                if (item.GetType() == typeof(Control.ElManualDosing))
                {
                    var manualDosing = (Control.ElManualDosing)item;
                    manualDosing.Initialize(_opcName);
                    _elementList.Add(manualDosing);
                }

                if (item.GetType() == typeof(Control.ElEvent))
                {
                    var elEvent = (Control.ElEvent)item;
                    elEvent.Initialize(_opcName);
                    _elementList.Add(elEvent);
                }

                if (item.GetType() == typeof(Indicators.ElContainer))
                {
                    var container = (Indicators.ElContainer)item;
                    container.Initialize(_opcName);
                    _elementList.Add(container);
                    _containerList.Add(container);
                }
            }

            ElControlTabl.Initialize(_opcName);
            _elementList.Add(ElControlTabl);

            ElControlControl.Initialize(_opcName);
            _elementList.Add(ElControlControl);

            ElControlZamesDosing.Initialize(_opcName);
            _elementList.Add(ElControlZamesDosing);

            ElControlZamesMixer.Initialize(_opcName);
            _elementList.Add(ElControlZamesMixer);

            ElControlStatus.Initialize(_opcName);
            _elementList.Add(ElControlStatus);

            var taskQueueItemsService = new TaskQueueItemsService();
            var recipeQueueViewModel = new ElQueueViewModel(taskQueueItemsService, logger);
            ElControlQueue.DataContext = recipeQueueViewModel;

            #endregion

            #region Timer

            LinkTimer = new DispatcherTimer();
            LinkTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            LinkTimer.Tick += new EventHandler(TimerTick500Ms);

            #endregion

            ViewModelScreenRbu.IsUpdating = false;

            Subscribe();

        }


        public void Subscribe()
        {
            ViewModelScreenRbu.IsUpdating = true;

            foreach (var item in _elementList)
            {
                item.Subscribe();
            }

            ViewModelScreenRbu.Subscribe();

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();

            LinkTimer.Start();
        }

        private void TimerTick500Ms(object sender, EventArgs e)
        {
            ViewModelScreenRbu.GetLink();

            _cycle++;
            if (_cycle > 7)
            {
                GetContainerMaterial();
                _cycle = 0;
            }
        }

        private async void GetContainerMaterial()
        {
            await Task.Run(() =>
            {
                ElContainer.GetContainerMaterial();
                foreach (var item in _containerList)
                {
                    item.GetMaterialName();
                }
            });
        }

        private void BtnBell_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModelScreenRbu.DriverBell = true;
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnBell, "btn_Operator_Bell", true, "Звонок водителю");
        }

        private void BtnBell_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewModelScreenRbu.DriverBell = false;
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnBell, "btn_Operator_Bell", false);
        }

        private void BtnReset_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnReset, "cmd_Reset", true, "Сброс аварий");
        }

        private void BtnReset_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnReset, "cmd_Reset", false);
        }

        private void BtnAck_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnAck, "cmd_Ack_Alarm", true, "Сброс звонка");
        }

        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnStop, "btn_StopALL", true, "Общий СТОП");
        }

        #region Menu

        private void Program_Closed(object sender, EventArgs e)
        {
            El_Screen_Redesing_RBU.LinkTimer?.Stop();
            foreach (Window w in App.Current.Windows)
            {
                w.Close();
            }
        }

        private void Login_OnClick(object sender, RoutedEventArgs e)
        {
            LoginClick?.Invoke();
        }

        private void User_OnClick(object sender, RoutedEventArgs e)
        {
            UserClick?.Invoke();
        }

        private void Mode_OnClick(object sender, RoutedEventArgs e)
        {
            WindowMode window = new WindowMode();
            window.Show();
        }

        private void Materials_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonListWithGroups window = new WindowCommonListWithGroups(new MaterialsService());
            window.Show();
        }

        private void MaterialTypes_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonList window = new WindowCommonList(new MaterialTypesService());
            window.Show();
        }

        private void Containers_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonList window = new WindowCommonList(new ContainersService());
            window.Show();
        }

        private void ContainerTypes_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonList window = new WindowCommonList(new ContainerTypesService());
            window.Show();
        }

        private void RecipeTypes_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonList window = new WindowCommonList(new RecipeTypesService());
            window.Show();
        }

        private void RecipeGroups_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonList window = new WindowCommonList(new RecipeGroupsService());
            window.Show();
        }

        private void Recipes_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonListWithGroups window = new WindowCommonListWithGroups(new RecipesService());
            window.Show();
        }

        private void MatchingMaterials_OnClick(object sender, RoutedEventArgs e)
        {
            Windows.WindowMatchingMaterials window = new Windows.WindowMatchingMaterials();
            window.Show();
        }

        private void CommonSettings_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonOpcParameters window = new WindowCommonOpcParameters(new CommonOpcParametersService());
            window.Show();
        }

        private void Batchers_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonList window = new WindowCommonList(new BatchersService());
            window.Show();
        }

        private void DosingSources_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonList window = new WindowCommonList(new DosingSourcesService());
            window.Show();
        }
        private void Archiver_OnClick(object sender, RoutedEventArgs e)
        {
            WindowArchiver window = new WindowArchiver(ElArchiversViewModel);
            window.ShowDialog();
        }
        private void Archive_OnClick(object sender, RoutedEventArgs e)
        {
            Commands.Archive_OnClick(_opcName);
        }

        private void ArchiveNew_OnClick(object sender, RoutedEventArgs e)
        {
            Commands.Archive_OnClick();
        }

        private void EventsArchive_OnClick(object sender, RoutedEventArgs e)
        {
            Commands.EventsArchive_OnClick(_opcName);
        }

        private void RecipeReport_Click(object sender, RoutedEventArgs e)
        {
            RecipeReportWindow window = new RecipeReportWindow();
            window.Show();
        }

        #region DosingSettings

        private void DosingBunker1_Click(object sender, RoutedEventArgs e)
        {
            var dosingSettingsViewModel = new DosingSettingsViewModel(_opcName, DosingSettingsViewModel.DosingSettingType.Inert, Static.ContainerItem.Bunker1);
            WindowDosingSettings window = new WindowDosingSettings(_opcName, dosingSettingsViewModel);
            window.Show();
        }
        private void DosingBunker2_Click(object sender, RoutedEventArgs e)
        {
            var dosingSettingsViewModel = new DosingSettingsViewModel(_opcName, DosingSettingsViewModel.DosingSettingType.Inert, Static.ContainerItem.Bunker2);
            WindowDosingSettings window = new WindowDosingSettings(_opcName, dosingSettingsViewModel);
            window.Show();
        }
        private void DosingBunker3_Click(object sender, RoutedEventArgs e)
        {
            var dosingSettingsViewModel = new DosingSettingsViewModel(_opcName, DosingSettingsViewModel.DosingSettingType.Inert, Static.ContainerItem.Bunker3);
            WindowDosingSettings window = new WindowDosingSettings(_opcName, dosingSettingsViewModel);
            window.Show();
        }
        private void DosingBunker4_Click(object sender, RoutedEventArgs e)
        {
            var dosingSettingsViewModel = new DosingSettingsViewModel(_opcName, DosingSettingsViewModel.DosingSettingType.Inert, Static.ContainerItem.Bunker4);
            WindowDosingSettings window = new WindowDosingSettings(_opcName, dosingSettingsViewModel);
            window.Show();
        }

        private void DosingSilo1_Click(object sender, RoutedEventArgs e)
        {
            var dosingSettingsViewModel = new DosingSettingsViewModel(_opcName, DosingSettingsViewModel.DosingSettingType.Cement, Static.ContainerItem.Silo1);
            WindowDosingSettings window = new WindowDosingSettings(_opcName, dosingSettingsViewModel);
            window.Show();
        }
        private void DosingSilo2_Click(object sender, RoutedEventArgs e)
        {
            var dosingSettingsViewModel = new DosingSettingsViewModel(_opcName, DosingSettingsViewModel.DosingSettingType.Cement, Static.ContainerItem.Silo2);
            WindowDosingSettings window = new WindowDosingSettings(_opcName, dosingSettingsViewModel);
            window.Show();
        }
        private void DosingWater_Click(object sender, RoutedEventArgs e)
        {
            var dosingSettingsViewModel = new DosingSettingsViewModel(_opcName, DosingSettingsViewModel.DosingSettingType.Water, Static.ContainerItem.Water);
            WindowDosingSettings window = new WindowDosingSettings(_opcName, dosingSettingsViewModel);
            window.Show();
        }
        private void DosingAdditive1_Click(object sender, RoutedEventArgs e)
        {
            var dosingSettingsViewModel = new DosingSettingsViewModel(_opcName, DosingSettingsViewModel.DosingSettingType.Additive, Static.ContainerItem.Additive1);
            WindowDosingSettings window = new WindowDosingSettings(_opcName, dosingSettingsViewModel);
            window.Show();
        }
        private void DosingAdditive2_Click(object sender, RoutedEventArgs e)
        {
            var dosingSettingsViewModel = new DosingSettingsViewModel(_opcName, DosingSettingsViewModel.DosingSettingType.Additive, Static.ContainerItem.Additive2);
            WindowDosingSettings window = new WindowDosingSettings(_opcName, dosingSettingsViewModel);
            window.Show();
        }

        #endregion

        private void MaterialSpeed_OnClick(object sender, RoutedEventArgs e)
        {
            WindowMaterialSpeed window = new WindowMaterialSpeed(_opcName);
            window.Show();
        }

        private void Spalsh_OnClick(object sender, RoutedEventArgs e)
        {
            WindowSplash window = new WindowSplash();
            window.ShowDialog();
        }

        private void VibratorSettings_OnClick(object sender, RoutedEventArgs e)
        {
            WindowVibratorSettings window = new WindowVibratorSettings(_opcName);
            window.Show();
        }

        private void AerationSettings_OnClick(object sender, RoutedEventArgs e)
        {
            WindowVibratorSettings window = new WindowVibratorSettings(_opcName, true);
            window.Show();
        }

        private void MainSettings_OnClick(object sender, RoutedEventArgs e)
        {
            WindowParam window = new WindowParam(_opcName);
            window.Show();
        }

        private void WeightMaterials_OnClick(object sender, RoutedEventArgs e)
        {
            WindowContainerSettings window = new WindowContainerSettings();
            window.Show();
        }

        #endregion

        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ViewModelScreenRbu.Cherry = true;
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ViewModelScreenRbu.Cherry = false;
        }

        public void OnClose(object sender, EventArgs e)
        {
            Closed?.Invoke(sender, e);
        }
    }

    public sealed class ViewModelScreenRbu : UserControl, INotifyPropertyChanged, IElementsUpdater
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private int _linkValue, _currentLinkValue, _cycle;
        private bool _link, _firstLink, _firstCheckLink;

        public bool Cherry { get; set; }

        public ViewModelScreenRbu(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            Brush = Brushes.LightGray;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _userPermit;
        public bool UserPermit
        {
            get { return _userPermit; }
            set
            {
                _userPermit = value;
                OnPropertyChanged(nameof(UserPermit));
            }
        }

        private bool _settingPermit;
        public bool SettingPermit
        {
            get { return _settingPermit; }
            set
            {
                _settingPermit = value;
                OnPropertyChanged(nameof(SettingPermit));
            }
        }

        private bool _isUpdating;
        public bool IsUpdating
        {
            get { return _isUpdating; }
            set
            {
                _isUpdating = value;
                OnPropertyChanged(nameof(IsUpdating));
            }
        }

        private bool _linkOk;
        public bool LinkOk
        {
            get { return _linkOk; }
            set
            {
                _linkOk = value;
                OnPropertyChanged(nameof(LinkOk));
            }
        }

        private string _currentDateTime;
        public string CurrentDateTime
        {
            get { return _currentDateTime; }
            set
            {
                _currentDateTime = value;
                OnPropertyChanged(nameof(CurrentDateTime));
            }
        }

        private double _current;
        public double Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnPropertyChanged(nameof(Current));
            }
        }

        private double _minCurrent;
        public double MinCurrent
        {
            get { return _minCurrent; }
            set
            {
                _minCurrent = value;
                OnPropertyChanged(nameof(MinCurrent));
            }
        }

        private double _maxCurrent;
        public double MaxCurrent
        {
            get { return _maxCurrent; }
            set
            {
                _maxCurrent = value;
                OnPropertyChanged(nameof(MaxCurrent));
            }
        }

        private double _normalMinCurrent;
        public double NormalMinCurrent
        {
            get { return _normalMinCurrent; }
            set
            {
                _normalMinCurrent = value;
                OnPropertyChanged(nameof(NormalMinCurrent));
            }
        }

        private double _normalMaxCurrent;
        public double NormalMaxCurrent
        {
            get { return _normalMaxCurrent; }
            set
            {
                _normalMaxCurrent = value;
                OnPropertyChanged(nameof(NormalMaxCurrent));
            }
        }

        private SolidColorBrush _brush;
        public SolidColorBrush Brush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                OnPropertyChanged(nameof(Brush));
            }
        }

        private bool _driverBell;
        public bool DriverBell
        {
            get { return _driverBell; }
            set
            {
                _driverBell = value;
                OnPropertyChanged(nameof(DriverBell));
            }
        }

        private bool _blockUnload;
        public bool BlockUnload
        {
            get { return _blockUnload; }
            set
            {
                _blockUnload = value;
                OnPropertyChanged(nameof(BlockUnload));
            }
        }

        private double _waterCorrect;
        public double WaterCorrect
        {
            get { return _waterCorrect; }
            set
            {
                _waterCorrect = value;
                OnPropertyChanged(nameof(WaterCorrect));
            }
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
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var visItem = new OpcMonitoredItem(_opc.cl.GetNode("gi_lifeWord"), OpcAttribute.Value);
            visItem.DataChangeReceived += HandleVisChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visItem);

            var currentItem = new OpcMonitoredItem(_opc.cl.GetNode("gr_Current"), OpcAttribute.Value);
            currentItem.DataChangeReceived += HandleCurrentChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(currentItem);
            var minCurrentItem = new OpcMonitoredItem(_opc.cl.GetNode("Current_Min"), OpcAttribute.Value);
            minCurrentItem.DataChangeReceived += HandleMinCurrentChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(minCurrentItem);
            var maxCurrentItem = new OpcMonitoredItem(_opc.cl.GetNode("Current_Max"), OpcAttribute.Value);
            maxCurrentItem.DataChangeReceived += HandleMaxCurrentChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(maxCurrentItem);
            var normalMinCurrentItem = new OpcMonitoredItem(_opc.cl.GetNode("Current_NormalMin"), OpcAttribute.Value);
            normalMinCurrentItem.DataChangeReceived += HandleNormalMinCurrentChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(normalMinCurrentItem);
            var normalMaxCurrentItem = new OpcMonitoredItem(_opc.cl.GetNode("Current_NormalMax"), OpcAttribute.Value);
            normalMaxCurrentItem.DataChangeReceived += HandleNormalMaxCurrentChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(normalMaxCurrentItem);
            var blockUnloadItem = new OpcMonitoredItem(_opc.cl.GetNode("gb_BlockUnload"), OpcAttribute.Value);
            blockUnloadItem.DataChangeReceived += HandleBlockUnloadChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(blockUnloadItem);
            var waterCorrectItem = new OpcMonitoredItem(_opc.cl.GetNode("gr_WaterCorrect"), OpcAttribute.Value);
            waterCorrectItem.DataChangeReceived += HandleWaterCorrectChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(waterCorrectItem);
        }

        private void HandleVisChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _linkValue = int.Parse(e.Item.Value.ToString());
        }

        private void HandleCurrentChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            Current = double.Parse(e.Item.Value.ToString());
            GetBrush();
        }

        private void HandleMinCurrentChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            MinCurrent = double.Parse(e.Item.Value.ToString());
        }

        private void HandleMaxCurrentChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            MaxCurrent = double.Parse(e.Item.Value.ToString());
        }

        private void HandleNormalMinCurrentChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            NormalMinCurrent = double.Parse(e.Item.Value.ToString());
            GetBrush();
        }

        private void HandleNormalMaxCurrentChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            NormalMaxCurrent = double.Parse(e.Item.Value.ToString());
            GetBrush();
        }

        private void HandleBlockUnloadChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            BlockUnload = bool.Parse(e.Item.Value.ToString());
        }

        private void HandleWaterCorrectChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            WaterCorrect = double.Parse(e.Item.Value.ToString());
        }

        public void GetUserPermit()
        {
            UserPermit = SelUser.GetInstance().GetSelUser().PermitUser;
            SettingPermit = SelUser.GetInstance().GetSelUser().PermitSetting;
        }

        private void GetBrush()
        {
            if (Current >= NormalMinCurrent && Current <= NormalMaxCurrent)
                Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF85FC84"));
            else
            {
                if (Current < NormalMinCurrent)
                    Brush = Brushes.Yellow;
                if (Current > NormalMaxCurrent)
                    Brush = Brushes.Salmon;
            }
        }

        public void GetLink()
        {
            CurrentDateTime = !Cherry ? DateTime.Now.ToString(" 🕔 HH:mm:ss      📅 dd.MM.yyyy") : _linkValue.ToString();

            _link = _linkValue != _currentLinkValue;
            _currentLinkValue = _linkValue;

            if (_link)
            {
                if (!Static.Link)
                {
                    EventsBase.GetInstance().GetControlEvents(_opcName).AddEvent("Связь со шкафом управления установлена", SystemEventType.Message);
                }
                if (!_firstLink)
                {
                    CreateAutoEvent();
                    _firstLink = true;
                }
                _cycle = 0;
                LinkOk = Static.Link = true;
            }
            else
            {
                _cycle++;
            }

            if (_cycle > 12)
            {
                if (Static.Link || !_firstCheckLink)
                {
                    EventsBase.GetInstance().GetControlEvents(_opcName).AddEvent("Нет связи со шкафом управления", SystemEventType.Alarm);
                    _firstCheckLink = true;
                }
                LinkOk = Static.Link = false;
                _cycle = 15;
            }
            LinkOk = Static.Link = true;
            IsUpdating = false;
        }

        private void CreateAutoEvent()
        {
            #region AutoEvent

            var autoEvent = new AutoEvent[150];
            autoEvent[0] = new AutoEvent(_opcName, "M_15.gb_AL_Feedback", SystemEventType.Alarm, "Насос M-15 - авария ОС", true);
            autoEvent[1] = new AutoEvent(_opcName, "M_15.gb_AL_External", SystemEventType.Alarm, "Насос M-15 - внешняя авария", true);
            autoEvent[2] = new AutoEvent(_opcName, "M_15.gb_AL_DKS", SystemEventType.Alarm, "Насос M-15 - авария ДКС", true);
            autoEvent[3] = new AutoEvent(_opcName, "M_16.gb_AL_Feedback", SystemEventType.Alarm, "Насос M-16 - авария ОС", true);
            autoEvent[4] = new AutoEvent(_opcName, "M_16.gb_AL_External", SystemEventType.Alarm, "Насос M-16 - внешняя авария", true);
            autoEvent[5] = new AutoEvent(_opcName, "M_16.gb_AL_DKS", SystemEventType.Alarm, "Насос M-16 - авария ДКС", true);
            autoEvent[6] = new AutoEvent(_opcName, "M_17.gb_AL_Feedback", SystemEventType.Alarm, "Насос M-17 - авария ОС", true);
            autoEvent[7] = new AutoEvent(_opcName, "M_17.gb_AL_External", SystemEventType.Alarm, "Насос M-17 - внешняя авария", true);
            autoEvent[8] = new AutoEvent(_opcName, "M_17.gb_AL_DKS", SystemEventType.Alarm, "Насос M-17 - авария ДКС", true);
            autoEvent[9] = new AutoEvent(_opcName, "M_18.gb_AL_Feedback", SystemEventType.Alarm, "Бетоносмеситель M-18 - авария ОС", true);
            autoEvent[10] = new AutoEvent(_opcName, "M_18.gb_AL_External", SystemEventType.Alarm, "Бетоносмеситель M-18 - внешняя авария", true);
            autoEvent[11] = new AutoEvent(_opcName, "M_18.gb_AL_Oil", SystemEventType.Alarm, "Бетоносмеситель M-18 - авария системы смазки", true);
            autoEvent[12] = new AutoEvent(_opcName, "M_9.gb_AL_Feedback", SystemEventType.Alarm, "Конвейер M-9 - авария ОС", true);
            autoEvent[13] = new AutoEvent(_opcName, "M_9.gb_AL_External", SystemEventType.Alarm, "Конвейер M-9 - внешняя авария", true);
            autoEvent[14] = new AutoEvent(_opcName, "M_9.gb_AL_DKS", SystemEventType.Alarm, "Конвейер M-9 - авария ДКС", true);
            autoEvent[15] = new AutoEvent(_opcName, "M_1.gb_AL_Feedback", SystemEventType.Alarm, "Конвейер M-1 - авария ОС", true);
            autoEvent[16] = new AutoEvent(_opcName, "M_1.gb_AL_External", SystemEventType.Alarm, "Конвейер M-1 - внешняя авария", true);
            autoEvent[17] = new AutoEvent(_opcName, "M_1.gb_AL_DKS", SystemEventType.Alarm, "Конвейер M-1 - авария ДКС", true);
            autoEvent[18] = new AutoEvent(_opcName, "M_11.gb_AL_Feedback", SystemEventType.Alarm, "Шнек M-11 - авария ОС", true);
            autoEvent[19] = new AutoEvent(_opcName, "M_11.gb_AL_External", SystemEventType.Alarm, "Шнек M-11 - внешняя авария", true);
            autoEvent[20] = new AutoEvent(_opcName, "M_11.gb_AL_DKS", SystemEventType.Alarm, "Шнек M-11 - авария ДКС", true);
            autoEvent[21] = new AutoEvent(_opcName, "М_12.gb_AL_Feedback", SystemEventType.Alarm, "Шнек M-12 - авария ОС", true);
            autoEvent[22] = new AutoEvent(_opcName, "М_12.gb_AL_External", SystemEventType.Alarm, "Шнек M-12 - внешняя авария", true);
            autoEvent[23] = new AutoEvent(_opcName, "М_12.gb_AL_DKS", SystemEventType.Alarm, "Шнек M-12 - авария ДКС", true);
            autoEvent[24] = new AutoEvent(_opcName, "M_13.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-13 - авария ОС", true);
            autoEvent[25] = new AutoEvent(_opcName, "M_13.gb_AL_External", SystemEventType.Alarm, "Вибратор M-13 - внешняя авария", true);
            autoEvent[26] = new AutoEvent(_opcName, "M_13.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-13 - авария ДКС", true);
            autoEvent[27] = new AutoEvent(_opcName, "M_14.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-14 - авария ОС", true);
            autoEvent[28] = new AutoEvent(_opcName, "M_14.gb_AL_External", SystemEventType.Alarm, "Вибратор M-14 - внешняя авария", true);
            autoEvent[29] = new AutoEvent(_opcName, "M_14.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-14 - авария ДКС", true);
            autoEvent[30] = new AutoEvent(_opcName, "M_10.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-10 - авария ОС", true);
            autoEvent[31] = new AutoEvent(_opcName, "M_10.gb_AL_External", SystemEventType.Alarm, "Вибратор M-10 - внешняя авария", true);
            autoEvent[32] = new AutoEvent(_opcName, "M_10.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-10 - авария ДКС", true);
            autoEvent[33] = new AutoEvent(_opcName, "M_2.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-2 - авария ОС", true);
            autoEvent[34] = new AutoEvent(_opcName, "M_2.gb_AL_External", SystemEventType.Alarm, "Вибратор M-2 - внешняя авария", true);
            autoEvent[35] = new AutoEvent(_opcName, "M_2.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-2 - авария ДКС", true);
            autoEvent[36] = new AutoEvent(_opcName, "M_3.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-3 - авария ОС", true);
            autoEvent[37] = new AutoEvent(_opcName, "M_3.gb_AL_External", SystemEventType.Alarm, "Вибратор M-3 - внешняя авария", true);
            autoEvent[38] = new AutoEvent(_opcName, "M_3.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-3 - авария ДКС", true);
            autoEvent[39] = new AutoEvent(_opcName, "M_4.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-4 - авария ОС", true);
            autoEvent[40] = new AutoEvent(_opcName, "M_4.gb_AL_External", SystemEventType.Alarm, "Вибратор M-4 - внешняя авария", true);
            autoEvent[41] = new AutoEvent(_opcName, "M_4.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-4 - авария ДКС", true);
            autoEvent[42] = new AutoEvent(_opcName, "M_5.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-5 - авария ОС", true);
            autoEvent[43] = new AutoEvent(_opcName, "M_5.gb_AL_External", SystemEventType.Alarm, "Вибратор M-5 - внешняя авария", true);
            autoEvent[44] = new AutoEvent(_opcName, "M_5.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-5 - авария ДКС", true);
            autoEvent[45] = new AutoEvent(_opcName, "M_6.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-6 - авария ОС", true);
            autoEvent[46] = new AutoEvent(_opcName, "M_6.gb_AL_External", SystemEventType.Alarm, "Вибратор M-6 - внешняя авария", true);
            autoEvent[47] = new AutoEvent(_opcName, "M_6.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-6 - авария ДКС", true);
            autoEvent[48] = new AutoEvent(_opcName, "M_7.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-7 - авария ОС", true);
            autoEvent[49] = new AutoEvent(_opcName, "M_7.gb_AL_External", SystemEventType.Alarm, "Вибратор M-7 - внешняя авария", true);
            autoEvent[50] = new AutoEvent(_opcName, "M_7.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-7 - авария ДКС", true);
            autoEvent[51] = new AutoEvent(_opcName, "V_1.gb_AL_Feedback_Open", SystemEventType.Alarm, "Задвижка V-1 - авария ОС-Open", true);
            autoEvent[52] = new AutoEvent(_opcName, "V_1.gb_AL_Feedback_Close", SystemEventType.Alarm, "Задвижка V-1 - авария ОС-Close", true);
            autoEvent[53] = new AutoEvent(_opcName, "V_1.gb_AL_BothSensor", SystemEventType.Alarm, "Задвижка V-1 - авария датчиков", true);
            autoEvent[54] = new AutoEvent(_opcName, "V_1.gb_AL_External", SystemEventType.Alarm, "Задвижка V-1 - внешняя авария", true);
            autoEvent[55] = new AutoEvent(_opcName, "V_2.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-2 - авария открытия", true);
            autoEvent[56] = new AutoEvent(_opcName, "V_2.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-2 - авария закрытия", true);
            autoEvent[57] = new AutoEvent(_opcName, "V_3.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-3 - авария открытия", true);
            autoEvent[58] = new AutoEvent(_opcName, "V_3.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-3 - авария закрытия", true);
            autoEvent[59] = new AutoEvent(_opcName, "V_4.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-4 - авария открытия", true);
            autoEvent[60] = new AutoEvent(_opcName, "V_4.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-4 - авария закрытия", true);
            autoEvent[61] = new AutoEvent(_opcName, "V_5.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-5 - авария открытия", true);
            autoEvent[62] = new AutoEvent(_opcName, "V_5.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-5 - авария закрытия", true);
            autoEvent[63] = new AutoEvent(_opcName, "V_6.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-6 - авария открытия", true);
            autoEvent[64] = new AutoEvent(_opcName, "V_6.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-6 - авария закрытия", true);
            autoEvent[65] = new AutoEvent(_opcName, "V_7.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-7 - авария открытия", true);
            autoEvent[66] = new AutoEvent(_opcName, "V_7.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-7 - авария закрытия", true);
            autoEvent[67] = new AutoEvent(_opcName, "V_9_1.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-9-1 - авария открытия", true);
            autoEvent[68] = new AutoEvent(_opcName, "V_9_1.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-9-1 - авария закрытия", true);
            autoEvent[69] = new AutoEvent(_opcName, "V_9_2.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-9-2 - авария открытия", true);
            autoEvent[70] = new AutoEvent(_opcName, "V_9_2.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-9-2 - авария закрытия", true);
            autoEvent[71] = new AutoEvent(_opcName, "V_10_1.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-10-1 - авария открытия", true);
            autoEvent[72] = new AutoEvent(_opcName, "V_10_1.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-10-1 - авария закрытия", true);
            autoEvent[73] = new AutoEvent(_opcName, "V_10_2.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-10-2 - авария открытия", true);
            autoEvent[74] = new AutoEvent(_opcName, "V_10_2.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-10-2 - авария закрытия", true);
            autoEvent[75] = new AutoEvent(_opcName, "V_11_1.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-11-1 - авария открытия", true);
            autoEvent[76] = new AutoEvent(_opcName, "V_11_1.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-11-1 - авария закрытия", true);
            autoEvent[77] = new AutoEvent(_opcName, "V_11_2.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-11-2 - авария открытия", true);
            autoEvent[78] = new AutoEvent(_opcName, "V_11_2.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-11-2 - авария закрытия", true);
            autoEvent[79] = new AutoEvent(_opcName, "V_12_1.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-12-1 - авария открытия", true);
            autoEvent[80] = new AutoEvent(_opcName, "V_12_1.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-12-1 - авария закрытия", true);
            autoEvent[81] = new AutoEvent(_opcName, "V_12_2.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-12-2 - авария открытия", true);
            autoEvent[82] = new AutoEvent(_opcName, "V_12_2.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-12-2 - авария закрытия", true);
            autoEvent[83] = new AutoEvent(_opcName, "M_19.gb_AL_Feedback", SystemEventType.Alarm, "Насос M-19 - Вода - авария ОС", true);
            autoEvent[84] = new AutoEvent(_opcName, "M_19.gb_AL_External", SystemEventType.Alarm, "Насос M-19 - Вода - внешняя авария", true);
            autoEvent[85] = new AutoEvent(_opcName, "M_19.gb_AL_DKS", SystemEventType.Alarm, "Насос M-19 - Вода - авария ДКС", true);
            autoEvent[86] = new AutoEvent(_opcName, "Air_Cement1.gb_AL_Open", SystemEventType.Alarm, "Аэратор силоса цемента 1 - авария открытия", true);
            autoEvent[87] = new AutoEvent(_opcName, "Air_Cement1.gb_AL_Close", SystemEventType.Alarm, "Аэратор силоса цемента 1 - авария закрытия", true);
            autoEvent[88] = new AutoEvent(_opcName, "Air_Cement2.gb_AL_Open", SystemEventType.Alarm, "Аэратор силоса цемента 2 - авария открытия", true);
            autoEvent[89] = new AutoEvent(_opcName, "Air_Cement2.gb_AL_Close", SystemEventType.Alarm, "Аэратор силоса цемента 2 - авария закрытия", true);
            autoEvent[90] = new AutoEvent(_opcName, "gb_AL_Hydro_Feedback", SystemEventType.Alarm, "Насос гидростанции -не сработал пускатель", true);
            autoEvent[91] = new AutoEvent(_opcName, "DI_NotStop_BTN_PLC", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления контроллера", false);
            autoEvent[92] = new AutoEvent(_opcName, "DI_NotStop_BTN_Inert", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления инертными фракциями", false);
            autoEvent[93] = new AutoEvent(_opcName, "DI_NotStop_BTN_Doz", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления дозированием", false);
            autoEvent[94] = new AutoEvent(_opcName, "DI_NotStop_BTN_Cem", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления цементом", false);
            autoEvent[95] = new AutoEvent(_opcName, "DI_NotStop_BTN_Water", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления жидкими компонентами", false);
            autoEvent[96] = new AutoEvent(_opcName, "DI_NotStop_BTN_BS", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления бетоносмесителем", false);
            autoEvent[97] = new AutoEvent(_opcName, "cmd_Stop", SystemEventType.Alarm, "Общий стоп", true);
            autoEvent[98] = new AutoEvent(_opcName, "gb_LinkERR_WeightCement", SystemEventType.Alarm, "Нет связи с весовым индикатором цемента", true);
            autoEvent[99] = new AutoEvent(_opcName, "gb_LinkERR_WeightWater", SystemEventType.Alarm, "Нет связи с весовым индикатором воды", true);
            autoEvent[100] = new AutoEvent(_opcName, "gb_LinkERR_WeightInert", SystemEventType.Alarm, "Нет связи с весовым индикатором инертных фракций", true);
            autoEvent[101] = new AutoEvent(_opcName, "gb_LinkERR_WeightAdditive", SystemEventType.Alarm, "Нет связи с весовым индикатором химических добавок", true);

            autoEvent[102] = new AutoEvent(_opcName, "M_18.gb_Warning_Oil", SystemEventType.Warning, "Бетоносмеситель M-18 - нет давления в системе смазки", true);
            autoEvent[103] = new AutoEvent(_opcName, "DI_Gates_M18_Closed", SystemEventType.Warning, "Крышка открыта", false);
            autoEvent[104] = new AutoEvent(_opcName, "DI_M9_sw_AutomatMode", SystemEventType.Warning, "Конвейер M-9 - автоматический режим", true);
            autoEvent[105] = new AutoEvent(_opcName, "DI_M9_sw_AutomatMode", SystemEventType.Warning, "Конвейер M-9 - ручной режим", false);
            autoEvent[106] = new AutoEvent(_opcName, "DI_M18_sw_AutomatMode", SystemEventType.Warning, "Бетоносмеситель M-18 - автоматический режим", true);
            autoEvent[107] = new AutoEvent(_opcName, "DI_M18_sw_AutomatMode", SystemEventType.Warning, "Бетоносмеситель M-18 - ручной режим", false);
            autoEvent[108] = new AutoEvent(_opcName, "gb_AL_Hydro_Pressure", SystemEventType.Warning, "Низкое давление в гидравлической магистрали", true);
            autoEvent[109] = new AutoEvent(_opcName, "gb_ArchiverERROR", SystemEventType.Warning, "Нет связи с программой архивации", true);
            autoEvent[110] = new AutoEvent(_opcName, "DI_Cement1_LSH", SystemEventType.Warning, "Сработал датчик верхнего уровня цемента в силосе №1", true);
            autoEvent[111] = new AutoEvent(_opcName, "DI_Cement2_LSH", SystemEventType.Warning, "Сработал датчик верхнего уровня цемента в силосе №2", true);

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
            #endregion
        }

        private RelayCommand _setWaterCorrect;
        public RelayCommand SetWaterCorrect
        {
            get
            {
                return _setWaterCorrect ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, "Коррекция воды, кг", -100.0, 100.0, "gr_WaterCorrect", WindowSetParameter.ValueType.Real, null, 1, 10.0, 20.0, 50.0, 100.0, 10.0);
                });
            }
        }

        private RelayCommand _setBlockUnload;
        public RelayCommand SetBlockUnload
        {
            get
            {
                return _setBlockUnload ??= new RelayCommand((o) =>
                {
                    if (BlockUnload)
                        Methods.ButtonClick("gb_BlockUnload", false, "Разрешение выгрузки из бетоносмесителя");
                    else
                        Methods.ButtonClick("gb_BlockUnload", true, "Запрет выгрузки из бетоносмесителя");
                });
            }
        }
    }

}

