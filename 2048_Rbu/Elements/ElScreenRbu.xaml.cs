using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using _2048_Rbu.Classes;
using AS_Library.Annotations;
using AS_Library.Link;
using _2048_Rbu.Interfaces;
using Opc.UaFx;
using Opc.UaFx.Client;
using System.Windows.Threading;
using _2048_Rbu.Elements.Control;
using _2048_Rbu.Elements.Indicators;
using _2048_Rbu.Windows;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using AsuBetonLibrary.Services;
using AsuBetonLibrary.Windows;
using NLog;

namespace _2048_Rbu.Elements
{
    public partial class ElScreenRbu : UserControl
    {
        OpcServer.OpcList _opcName;

        public static DispatcherTimer LinkTimer;

        private readonly List<IElementsUpdater> _elementList = new List<IElementsUpdater>();
        private List<ElContainer> _containerList = new List<ElContainer>();
        private ViewModelScreenRbu _viewModelScreenRbu;

        private WindowMode _mode;

        private int _cycle;

        public ElScreenRbu()
        {
            InitializeComponent();
        }

        public void Initialize(Logger logger)
        {
            _opcName = OpcServer.OpcList.Rbu;

            _viewModelScreenRbu = new ViewModelScreenRbu(_opcName);
            DataContext = _viewModelScreenRbu;

            _viewModelScreenRbu.IsUpdating = true;

            OpcServer.GetInstance().InitOpc(_opcName, "opc.tcp://192.168.100.70:49320");
            OpcServer.GetInstance().ConnectOpc(_opcName);
            EventsBase.GetInstance().CreateControlEvents(_opcName);

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

                if (item.GetType() == typeof(Indicators.ElContainer))
                {
                    var container = (Indicators.ElContainer)item;
                    container.Initialize(_opcName);
                    _containerList.Add(container);
                }
            }

            ElControlControl.Initialize(_opcName);
            _elementList.Add(ElControlControl);

            ElControlDosingWait.Initialize(_opcName);
            _elementList.Add(ElControlDosingWait);

            ElControlTabl.Initialize(_opcName);
            _elementList.Add(ElControlTabl);

            ElControlZamesDozing.Initialize(_opcName);
            _elementList.Add(ElControlZamesDozing);

            ElControlZamesMixer.Initialize(_opcName);
            _elementList.Add(ElControlZamesMixer);

            var taskQueueItemsService = new TaskQueueItemsService();
            var recipeQueueViewModel = new ElQueueViewModel(taskQueueItemsService, logger);
            ElControlQueue.DataContext = recipeQueueViewModel;

            #endregion

            _viewModelScreenRbu.IsUpdating = false;

            Subscribe();
        }

        public void Subscribe()
        {
            _viewModelScreenRbu.IsUpdating = true;

            foreach (var item in _elementList)
            {
                item.Subscribe();
            }

            _viewModelScreenRbu.Subscribe();

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();

            #region Timers

            LinkTimer = new DispatcherTimer();
            LinkTimer.Interval = new TimeSpan(0, 0, 0, 1);
            LinkTimer.Tick += new EventHandler(TimerTick1S);
            LinkTimer.Start();

            #endregion
        }

        private void TimerTick1S(object sender, EventArgs e)
        {
            _viewModelScreenRbu.GetLink();

            _cycle++;
            if (_cycle > 5)
            {
                foreach (var item in _containerList)
                {
                    item.GetMaterial();
                }
                ElControlTabl.GetValue();
                _cycle = 0;
            }
        }

        #region Menu
        private void BtnReset_Down(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnReset, "cmd_Reset", true, "Сброс аварий");
        }

        private void BtnReset_Up(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnReset, "cmd_Reset", false);
        }

        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnStop, "cmd_Stop", true, "Общий СТОП");
        }

        private void BtnAck_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnAck, "cmd_Ack_Alarm", true, "Сброс звонка");
        }

        private void Mode_OnClick(object sender, RoutedEventArgs e)
        {
            _mode = new WindowMode(_opcName);
            _mode.Subscribe();
            _mode.ShowDialog();
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
            WindowMatchingMaterials window = new WindowMatchingMaterials();
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
        #endregion
    }

    public sealed class ViewModelScreenRbu : UserControl, INotifyPropertyChanged, IElementsUpdater
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        private int _currentLinkValue, _linkValue, _cycle;


        public ViewModelScreenRbu(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

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

        private bool _linkMessage;
        public bool LinkMessage
        {
            get { return _linkMessage; }
            set
            {
                _linkMessage = value;
                OnPropertyChanged(nameof(LinkMessage));
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
        }

        private void HandleVisChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            _linkValue = int.Parse(e.Item.Value.ToString());
        }

        public void GetLink()
        {
            CurrentDateTime = DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy");

            _cycle++;
            if (_cycle > 2)
            {
                Static.Link = (_linkValue != _currentLinkValue);
                LinkMessage = !Static.Link;
                _currentLinkValue = _linkValue;

                _cycle = 0;

                IsUpdating = false;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
