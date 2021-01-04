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

        private TasksReader TasksReader { get; set; } = new TasksReader();
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private long _id, _currentId;

        private int _tempTimeProcess, _tempOrderCycle;
        private double _tempPartial, _tempFull;

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
            var idItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.TaskID_mixer"), OpcAttribute.Value);
            idItem.DataChangeReceived += HandleIdChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(idItem);

            var orderActItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.CurrentMixing_batchNum"), OpcAttribute.Value);
            orderActItem.DataChangeReceived += HandleOrderActChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(orderActItem);

            var orderItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.PAR_BatchesQuantity"), OpcAttribute.Value);
            orderItem.DataChangeReceived += HandleOrderChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(orderItem);

            var mixingItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.Current_MixingTime"), OpcAttribute.Value);
            mixingItem.DataChangeReceived += HandleMixingChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(mixingItem);

            var timePrItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.PAR_MixingTime"), OpcAttribute.Value);
            timePrItem.DataChangeReceived += HandleTimeProcessChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(timePrItem);

            var currentPartialtem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.Current_PartUnloadTime"), OpcAttribute.Value);
            currentPartialtem.DataChangeReceived += HandlePartialProcessChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(currentPartialtem);

            var currentFullItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.Current_UnloadTime"), OpcAttribute.Value);
            currentFullItem.DataChangeReceived += HandleFullProcessChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(currentFullItem);

            var partialUnloadItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.PAR_TimePartialUnload"), OpcAttribute.Value);
            partialUnloadItem.DataChangeReceived += HandlePartialUnloadChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(partialUnloadItem);

            var fullUnloadItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.PAR_TimeFullUnload"), OpcAttribute.Value);
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

        private void HandleMixingChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                MixingProcess = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleTimeProcessChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempTimeProcess = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandlePartialProcessChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                PartialProcess = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleFullProcessChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                FullProcess = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandlePartialUnloadChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempPartial = double.Parse(e.Item.Value.ToString());
                PartialUnload = Convert.ToInt32(_tempPartial);
                GetProgressWidth();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleFullUnloadChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempFull = double.Parse(e.Item.Value.ToString());
                FullUnload = Convert.ToInt32(_tempFull);
                GetProgressWidth();
            }
            catch (Exception exception)
            {
            }
        }

        private void GetProgressWidth()
        {
            WidthPartial = _tempPartial == 0.0? 64 : 214 * _tempPartial / (_tempPartial + _tempFull);
            WidthFull = _tempFull == 0.0 ? 150:214 * _tempFull / (_tempPartial + _tempFull) * 214;
            GetTable();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private double _widthPartial;
        public double WidthPartial
        {
            get { return _widthPartial; }
            set
            {
                _widthPartial = value;
                OnPropertyChanged(nameof(WidthPartial));
            }
        }

        private double _widthFull;
        public double WidthFull
        {
            get { return _widthFull; }
            set
            {
                _widthFull = value;
                OnPropertyChanged(nameof(WidthFull));
            }
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

        private int? _partialUnload;
        public int? PartialUnload
        {
            get { return _partialUnload; }
            set
            {
                _partialUnload = value;
                OnPropertyChanged(nameof(PartialUnload));
            }
        }

        private int? _fullUnload;
        public int? FullUnload
        {
            get { return _fullUnload; }
            set
            {
                _fullUnload = value;
                OnPropertyChanged(nameof(FullUnload));
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

        private int? _partialProcess;
        public int? PartialProcess
        {
            get { return _partialProcess; }
            set
            {
                _partialProcess = value;
                OnPropertyChanged(nameof(PartialProcess));
            }
        }

        private int? _fullProcess;
        public int? FullProcess
        {
            get { return _fullProcess; }
            set
            {
                _fullProcess = value;
                OnPropertyChanged(nameof(FullProcess));
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
                OrderCycle = _tempOrderCycle;
                TimeProcess = _tempTimeProcess;
                TimeRazgruzka = PartialUnload + FullUnload;

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
                MixingProcess = null;
                TimeProcess = null;
                PartialProcess = null;
                FullProcess = null;
                PartialUnload = null;
                FullUnload = null;
                TimeRazgruzka = null;
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
