using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using _2048_Rbu.Windows;
using AsuBetonLibrary.Services;
using AsuBetonLibrary.Windows;
using NLog;

namespace _2048_Rbu.Elements
{
    /// <summary>
    /// Interaction logic for ElScreenRbu.xaml
    /// </summary>
    public partial class ElScreenRbu : INotifyPropertyChanged
    {
        public static DispatcherTimer LinkTimer;

        OpcServer.OpcList _opcName;
        private OPC_client _opc;

        private int _currentLinkValue, _linkValue, _cycle;

        private readonly List<IElementsUpdater> _elementList = new List<IElementsUpdater>();
        
        private WindowMode _mode;

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

        public ElScreenRbu()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            IsUpdating = true;

            _opcName = OpcServer.OpcList.Rbu;
            #region Timers
            LinkTimer = new DispatcherTimer();
            LinkTimer.Interval = new TimeSpan(0, 0, 0, 1);
            LinkTimer.Tick += new EventHandler(TimerTick1S);
            LinkTimer.Start();
            #endregion

            DataContext = this;

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

            //var elRecipeQueue = new ElTaskQueue();
            //ElGrid.Children.Add(elRecipeQueue);
            //Grid.SetColumnSpan(elRecipeQueue, 1);
            //elRecipeQueue.Margin = new Thickness(10,200,0,0);
            //ElRecipeQueue.Initialize(_opcName);
            //var taskQueueItemsService = new TaskQueueItemsService();
            //var recipeQueueViewModel = new ElTaskQueueViewModel(taskQueueItemsService, logger);
            //ElRecipeQueue.DataContext = recipeQueueViewModel;

            #endregion

            IsUpdating = false;

            Subscribe();
        }

        public void Subscribe()
        {
            IsUpdating = true;

            foreach (var item in _elementList)
            {
                item.Subscribe();
            }
            CreateSubscription();
            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
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

        private void TimerTick1S(object sender, EventArgs e)
        {
            GetLink();
        }

        private void GetLink()
        {
            LblDateTime.Content = DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy");

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
            Methods.ButtonClick(btn, BtnAck, "cmd_Ack", true, "Сброс звонка");
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
