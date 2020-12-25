using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using AS_Library.Link;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using DevExpress.Data.Browsing;
using Opc.UaFx;
using Opc.UaFx.Client;


namespace _2048_Rbu.Elements.Control
{
    public partial class ElZamesDozing : IElementsUpdater
    {
        public ViewModelDozing _viewModelDozing;

        public ElZamesDozing()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _viewModelDozing = new ViewModelDozing(opcName);
            DataContext = _viewModelDozing;

        }

        public void Subscribe()
        {
            _viewModelDozing.Subscribe();
        }

        public void Unsubscribe()
        {
        }
    }

    public sealed class ViewModelDozing : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private RecipesReader RecipesReader { get; set; } = new RecipesReader();
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private long _id, _currentId;

        public ViewModelDozing(OpcServer.OpcList opcName)
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
            var idItem = new OpcMonitoredItem(_opc.cl.GetNode("TaskID"), OpcAttribute.Value);
            idItem.DataChangeReceived += HandleIdChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(idItem);

            var orderActItem = new OpcMonitoredItem(_opc.cl.GetNode("Current_batchNum"), OpcAttribute.Value);
            orderActItem.DataChangeReceived += HandleOrderActChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(orderActItem);

            var orderItem = new OpcMonitoredItem(_opc.cl.GetNode("PAR_BatchesQuantity"), OpcAttribute.Value);
            orderItem.DataChangeReceived += HandleOrderChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(orderItem);

            var dozingItem = new OpcMonitoredItem(_opc.cl.GetNode(""), OpcAttribute.Value);
            dozingItem.DataChangeReceived += HandleDozingChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(dozingItem);
        }

        private void HandleIdChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _id = long.Parse(e.Item.Value.ToString());
                GetTable();
                OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
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
                GetTable();
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
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleDozingChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                DozingProcess = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
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

        private int? _dozingProcess;
        public int? DozingProcess
        {
            get { return _dozingProcess; }
            set
            {
                _dozingProcess = value;
                OnPropertyChanged(nameof(DozingProcess));
            }
        }

        public void GetTable()
        {
            if (_id != 0)
            {
                if (_id != _currentId)
                {
                    try
                    {
                        GetTask(_id);
                        _currentId = _id;
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.WriteAllText(@"Log\log.txt", DateTime.Now + " - " + ex.Message + "->" + _id);
                    }
                }
            }
            else
            {
                Recipes = null;
                SelRecipes = null;
                OrderActCycle = null;
                OrderCycle = null;
                DozingProcess = null;
                _currentId = _id;
            }
        }

        private void GetTask(long id)
        {
            Recipes = new ObservableCollection<ApiRecipe>(RecipesReader.ListRecipes());
            SelRecipes = Recipes.FirstOrDefault(x => x.Id == id);
        }
    }
}