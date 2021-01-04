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

        private TasksReader TasksReader { get; set; } = new TasksReader();
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private long _id, _currentId;

        private int _tempOrderCycle;

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

            var dozingItem = new OpcMonitoredItem(_opc.cl.GetNode("CommonProgress"), OpcAttribute.Value);
            dozingItem.DataChangeReceived += HandleDozingChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(dozingItem);
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
                _tempOrderCycle = int.Parse(e.Item.Value.ToString());
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
                DozingProcess = double.Parse(e.Item.Value.ToString());
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

        private ObservableCollection<ApiTask> _tasks;
        public ObservableCollection<ApiTask> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        private ApiTask _selTask;
        public ApiTask SelTask
        {
            get { return _selTask; }
            set
            {
                _selTask = value;
                OnPropertyChanged(nameof(SelTask));
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

        private double? _dozingProcess;
        public double? DozingProcess
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
                OrderCycle = _tempOrderCycle;
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
                Tasks = null;
                SelTask = null;
                OrderActCycle = null;
                OrderCycle = null;
                DozingProcess = null;
                _currentId = _id;
            }
        }

        private void GetTask(long id)
        {
            Tasks = new ObservableCollection<ApiTask>(TasksReader.ListTasks());
            SelTask = Tasks.FirstOrDefault(x => x.Id == id);
        }
    }
}