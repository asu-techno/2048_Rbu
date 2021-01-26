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
    }

    public class ViewModelMixer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TasksReader TasksReader { get; set; } = new TasksReader();
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private long _id, _currentId;

        private int _tempCurrentBatchNum, _tempBatchesQuantity, _tempMixingProcess, _tempParMixingProcess,
            _tempPartialUnloadProcess, _tempFullUnloadProcess, _tempParPartialUnload, _tempParFullUnload;
        private double _tempPartialWidth, _tempFullWidth;

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

            var currentBatchNumItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.CurrentMixing_batchNum"), OpcAttribute.Value);
            currentBatchNumItem.DataChangeReceived += HandleCurrentBatchNumChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(currentBatchNumItem);

            var batchesQuantityItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.PAR_BatchesQuantity"), OpcAttribute.Value);
            batchesQuantityItem.DataChangeReceived += HandleBatchesQuantityChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(batchesQuantityItem);

            var mixingProcessItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.Current_MixingTime"), OpcAttribute.Value);
            mixingProcessItem.DataChangeReceived += HandleMixingProcessChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(mixingProcessItem);

            var parMixingProcessItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.PAR_MixingTime"), OpcAttribute.Value);
            parMixingProcessItem.DataChangeReceived += HandleParMixingProcessChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(parMixingProcessItem);

            var currentPartialUnloadItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.Current_PartUnloadTime"), OpcAttribute.Value);
            currentPartialUnloadItem.DataChangeReceived += HandlePartialUnloadProcessChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(currentPartialUnloadItem);

            var currentFullUnloadItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.Current_UnloadTime"), OpcAttribute.Value);
            currentFullUnloadItem.DataChangeReceived += HandleFullUnloadProcessChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(currentFullUnloadItem);

            var parPartialUnloadItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.PAR_TimePartialUnload"), OpcAttribute.Value);
            parPartialUnloadItem.DataChangeReceived += HandleParPartialUnloadChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(parPartialUnloadItem);

            var parFullUnloadItem = new OpcMonitoredItem(_opc.cl.GetNode("Mixer.PAR_TimeFullUnload"), OpcAttribute.Value);
            parFullUnloadItem.DataChangeReceived += HandleParFullUnloadChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(parFullUnloadItem);
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

        private void HandleCurrentBatchNumChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempCurrentBatchNum = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleBatchesQuantityChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempBatchesQuantity = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleMixingProcessChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempMixingProcess = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleParMixingProcessChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempParMixingProcess = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandlePartialUnloadProcessChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempPartialUnloadProcess = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleFullUnloadProcessChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempFullUnloadProcess = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleParPartialUnloadChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempPartialWidth = double.Parse(e.Item.Value.ToString());
                _tempParPartialUnload = Convert.ToInt32(_tempPartialWidth);
                GetProgressWidth();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleParFullUnloadChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempFullWidth = double.Parse(e.Item.Value.ToString());
                _tempParFullUnload = Convert.ToInt32(_tempFullWidth);
                GetProgressWidth();
            }
            catch (Exception exception)
            {
            }
        }

        private void GetProgressWidth()
        {
            WidthPartial = _tempPartialWidth == 0.0 ? 64 : 244 * _tempPartialWidth / (_tempPartialWidth + _tempFullWidth);
            WidthFull = _tempPartialWidth == 0.0 ? 180 : 244 * _tempFullWidth / (_tempPartialWidth + _tempFullWidth);
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

        private int? _parPartialUnload;
        public int? ParPartialUnload
        {
            get { return _parPartialUnload; }
            set
            {
                _parPartialUnload = value;
                OnPropertyChanged(nameof(ParPartialUnload));
            }
        }

        private int? _parFullUnload;
        public int? ParFullUnload
        {
            get { return _parFullUnload; }
            set
            {
                _parFullUnload = value;
                OnPropertyChanged(nameof(ParFullUnload));
            }
        }

        private int? _currentBatchNum;
        public int? CurrentBatchNum
        {
            get { return _currentBatchNum; }
            set
            {
                _currentBatchNum = value;
                OnPropertyChanged(nameof(CurrentBatchNum));
            }
        }

        private int? _batchesQuantity;
        public int? BatchesQuantity
        {
            get { return _batchesQuantity; }
            set
            {
                _batchesQuantity = value;
                OnPropertyChanged(nameof(BatchesQuantity));
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

        private int? _parMixingProcess;
        public int? ParMixingProcess
        {
            get { return _parMixingProcess; }
            set
            {
                _parMixingProcess = value;
                OnPropertyChanged(nameof(ParMixingProcess));
            }
        }

        private int? _partialUnloadProcess;
        public int? PartialUnloadProcess
        {
            get { return _partialUnloadProcess; }
            set
            {
                _partialUnloadProcess = value;
                OnPropertyChanged(nameof(PartialUnloadProcess));
            }
        }

        private int? _fullUnloadProcess;
        public int? FullUnloadProcess
        {
            get { return _fullUnloadProcess; }
            set
            {
                _fullUnloadProcess = value;
                OnPropertyChanged(nameof(FullUnloadProcess));
            }
        }

        private int? _parSumUnload;
        public int? ParSumUnload
        {
            get { return _parSumUnload; }
            set
            {
                _parSumUnload = value;
                OnPropertyChanged(nameof(ParSumUnload));
            }
        }

        private string _selTaskName;
        public string SelTaskName
        {
            get { return _selTaskName; }
            set
            {
                _selTaskName = value;
                OnPropertyChanged(nameof(SelTaskName));
            }
        }

        public void GetTable()
        {
            if (_id != 0)
            {
                CurrentBatchNum = _tempCurrentBatchNum;
                BatchesQuantity = _tempBatchesQuantity;
                MixingProcess = _tempMixingProcess;
                ParMixingProcess = _tempParMixingProcess;
                PartialUnloadProcess = _tempPartialUnloadProcess;
                FullUnloadProcess = _tempFullUnloadProcess;
                ParPartialUnload = _tempParPartialUnload;
                ParFullUnload = _tempParFullUnload;
                ParSumUnload = _tempParPartialUnload + _tempParFullUnload;

                if (_id != _currentId)
                {
                    try
                    {
                        GetTask(_id);
                        _currentId = _id;
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.AppendAllText(@"log.txt", DateTime.Now + " - " + ex.Message + "->" + _id);
                    }
                }
            }
            else
            {
                SelTaskName = null;
                Tasks = null;
                SelTask = null;
                CurrentBatchNum = null;
                BatchesQuantity = null;
                MixingProcess = null;
                ParMixingProcess = null;
                PartialUnloadProcess = null;
                FullUnloadProcess = null;
                ParPartialUnload = null;
                ParFullUnload = null;
                ParSumUnload = null;
                _currentId = _id;
            }
        }

        private void GetTask(long id)
        {
            Tasks = new ObservableCollection<ApiTask>(TasksReader.ListTasks());
            SelTask = Tasks.FirstOrDefault(x => x.Id == id);
            SelTaskName = SelTask.Id + " - " + SelTask.Recipe.Name;
        }
    }
}
