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
using ServiceLibCore.Classes;
using AS_Library.Classes;
using AS_Library.Events;
using AS_Library.Events.Classes;
using AS_Library.Graphics;
using AS_Library.Readers;
using AsLibraryCore.Events.Classes;
using RelayCommand = AS_Library.Classes.RelayCommand;

namespace _2048_Rbu.Elements
{
    public partial class ElScreenRbu : UserControl
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

        public ElScreenRbu()
        {
            InitializeComponent();
        }

        public void Initialize(Logger logger)
        {
            _opcName = OpcServer.OpcList.Rbu;

            ViewModelScreenRbu = new ViewModelScreenRbu(_opcName);
            DataContext = ViewModelScreenRbu;
            
            ViewModelScreenRbu.IsUpdating = true;

            OpcServer.GetInstance().InitOpc(_opcName, Service.GetInstance().GetOpcDict()["OpcServerAddress"]);
            OpcServer.GetInstance().ConnectOpc(_opcName);

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
            LinkTimer.Interval = new TimeSpan(0, 0, 0, 0,500);
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
            Methods.ButtonClick(btn, BtnStop, "cmd_Stop", true, "Общий СТОП");
        }

        #region Menu

        private void Program_Closed(object sender, EventArgs e)
        {
            ElScreenRbu.LinkTimer?.Stop();
            foreach (Window w in App.Current.Windows)
                w.Close();
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
            window.ShowDialog();
        }

        private void Materials_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCommonList window = new WindowCommonList(new MaterialsService());
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
            WindowCommonList window = new WindowCommonList(new RecipesService());
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

        private void Archive_OnClick(object sender, RoutedEventArgs e)
        {
            Commands.Archive_OnClick(_opcName);
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
            WindowVibratorSettings window = new WindowVibratorSettings(_opcName,true);
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
    }

    public sealed class ViewModelScreenRbu : UserControl, INotifyPropertyChanged, IElementsUpdater
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private int _linkValue, _currentLinkValue, _cycle;
        private bool _link;

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
            CurrentDateTime = !Cherry ? DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy") : _linkValue.ToString();

            _link = _linkValue != _currentLinkValue;
            _currentLinkValue = _linkValue;

            if (_link)
            {
                _cycle = 0;
                LinkOk = Static.Link = true;
            }
            else
            {
                _cycle++;
            }

            if (_cycle > 5)
            {
                LinkOk = Static.Link = false;
                _cycle = 11;
            }

            IsUpdating = false;
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
                        Methods.ButtonClick("gb_BlockUnload", false, "Разрешить выгрузку из бетоносмесителя");
                    else
                        Methods.ButtonClick("gb_BlockUnload", true, "Запретить выгрузку из бетоносмесителя");
                });
            }
        }
    }
}
