using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using AS_Library.Link;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using Opc.UaFx;
using Opc.UaFx.Client;
using System.Linq;

namespace _2048_Rbu.Elements.Control
{
    public partial class ElZamesMixer : IElementsUpdater
    {
        public ViewModelMixer _viewModelMixer;
        private OpcServer.OpcList _opcName;

        public ElZamesMixer()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;
            _viewModelMixer = new ViewModelMixer(opcName);
            DataContext = _viewModelMixer;

        }

        public void Subscribe()
        {
            _viewModelMixer.Subscribe();
        }

        public void Unsubscribe()
        {
        }

        private void lbl_t_razgr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;

            Methods.SetParameter(t_razgr, btn, _opcName, "Длительность разгрузки, c", 0, 100, "PAR_TimeFullUnload", "Real", null, 0, 0);
        }
    }

    public class ViewModelMixer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private RecipesReader RecipesReader { get; set; } = new RecipesReader();
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private long _id;

        private int _partialUnload, _fullUnload;

        public ViewModelMixer(OpcServer.OpcList opcName)
        {
            _opcName = opcName;
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
            var idItem = new OpcMonitoredItem(_opc.cl.GetNode("TaskID_mixer"), OpcAttribute.Value);
            idItem.DataChangeReceived += HandleIdChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(idItem);

            var orderActItem = new OpcMonitoredItem(_opc.cl.GetNode("CurrentMixing_batchNum"), OpcAttribute.Value);
            orderActItem.DataChangeReceived += HandleOrderActChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(orderActItem);

            var orderItem = new OpcMonitoredItem(_opc.cl.GetNode("PAR_BatchesQuantity"), OpcAttribute.Value);
            orderItem.DataChangeReceived += HandleOrderChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(orderItem);

            var mixingItem = new OpcMonitoredItem(_opc.cl.GetNode("Current_MixingTime"), OpcAttribute.Value);
            mixingItem.DataChangeReceived += HandleMixingChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(mixingItem);

            var timePrItem = new OpcMonitoredItem(_opc.cl.GetNode("PAR_MixingTime"), OpcAttribute.Value);
            timePrItem.DataChangeReceived += HandleTimeProcessChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(timePrItem); 

            var razgruzkaItem = new OpcMonitoredItem(_opc.cl.GetNode("Current_UnloadTime"), OpcAttribute.Value);
            razgruzkaItem.DataChangeReceived += HandleRazgruzkaChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(razgruzkaItem);

            var partialUnloadItem = new OpcMonitoredItem(_opc.cl.GetNode("PAR_TimePartialUnload"), OpcAttribute.Value);
            partialUnloadItem.DataChangeReceived += HandlePartialUnloadChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(partialUnloadItem);

            var fullUnloadItem = new OpcMonitoredItem(_opc.cl.GetNode("PAR_TimeFullUnload"), OpcAttribute.Value);
            fullUnloadItem.DataChangeReceived += HandleFullUnloadChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(fullUnloadItem);
        }

        private void HandleIdChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _id = long.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleOrderActChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                OrderActCycle = int.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleOrderChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                OrderCycle = int.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleMixingChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                MixingProcess = int.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleTimeProcessChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                TimeProcess = int.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleRazgruzkaChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                RazgruzkaProcess = int.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void HandlePartialUnloadChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _partialUnload = int.Parse(e.Item.Value.ToString());
                TimeUnload();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleFullUnloadChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _fullUnload = int.Parse(e.Item.Value.ToString());
                TimeUnload();
            }
            catch (Exception exception)
            {
            }
        }

        private void TimeUnload()
        {
            TimeRazgruzka = _partialUnload + _fullUnload;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<ApiRecipe> _recipes;
        public ObservableCollection<ApiRecipe> Recipes
        {
            get { return _recipes; }
            set
            {
                _recipes = value;
                OnPropertyChanged(nameof(Recipes));
            }
        }

        private ApiRecipe _selRecipes;
        public ApiRecipe SelRecipes
        {
            get { return _selRecipes; }
            set
            {
                _selRecipes = value;
                OnPropertyChanged(nameof(SelRecipes));
            }
        }

        private int? _orderActCycle;
        public int? OrderActCycle
        {
            get { return _orderActCycle; }
            set
            {
                _orderActCycle = value;
                OnPropertyChanged(nameof(OrderActCycle));
            }
        }

        private int? _orderCycle;
        public int? OrderCycle
        {
            get { return _orderCycle; }
            set
            {
                _orderCycle = value;
                OnPropertyChanged(nameof(OrderCycle));
            }
        }

        private int? _mixingProcess;
        public int? MixingProcess
        {
            get { return _mixingProcess; }
            set
            {
                _mixingProcess = value;
                OnPropertyChanged(nameof(MixingProcess));
            }
        }

        private int? _timeProcess;
        public int? TimeProcess
        {
            get { return _timeProcess; }
            set
            {
                _timeProcess = value;
                OnPropertyChanged(nameof(TimeProcess));
            }
        }

        private int? _razgruzkaProcess;
        public int? RazgruzkaProcess
        {
            get { return _razgruzkaProcess; }
            set
            {
                _razgruzkaProcess = value;
                OnPropertyChanged(nameof(RazgruzkaProcess));
            }
        }
        private int? _timeRazgruzka;
        public int? TimeRazgruzka
        {
            get { return _timeRazgruzka; }
            set
            {
                _timeRazgruzka = value;
                OnPropertyChanged(nameof(TimeRazgruzka));
            }
        }


        public void GetTable()
        {
            if (_id != 0)
            {
                try
                {
                    GetTask(_id);
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText(@"Log\log.txt", DateTime.Now + " - " + ex.Message + "->" + _id);
                }
            }
            else
            {
                Recipes = null;
                SelRecipes = null;
                OrderActCycle = null;
                OrderCycle = null;
                MixingProcess = null;
                TimeProcess = null;
                RazgruzkaProcess = null;
                TimeRazgruzka = null;
            }
        }

        private void GetTask(long id)
        {
            UpdateTasks();
            UpdateSelTask(id);
        }

        private void UpdateTasks()
        {
            Recipes = new ObservableCollection<ApiRecipe>(RecipesReader.ListRecipes());
        }
        private void UpdateSelTask(long id)
        {
            SelRecipes = Recipes.FirstOrDefault(x => x.Id == id);
        }
    }
}
