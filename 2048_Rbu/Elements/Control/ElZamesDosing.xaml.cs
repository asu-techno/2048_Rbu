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
    public partial class ElZamesDosing : IElementsUpdater
    {
        public ViewModelDosing _viewModelDosing;

        public ElZamesDosing()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _viewModelDosing = new ViewModelDosing(opcName);
            DataContext = _viewModelDosing;

        }

        public void Subscribe()
        {
            _viewModelDosing.Subscribe();
        }

        public void Unsubscribe()
        {
        }
    }

    public sealed class ViewModelDosing : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private long _id, _currentId;

        private int _tempCurrentBatchNum, _tempBatchesQuantity; 
        private double _tempDosingProcess;

        public ViewModelDosing(OpcServer.OpcList opcName)
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

            var currentBatchNumItem = new OpcMonitoredItem(_opc.cl.GetNode("Current_batchNum"), OpcAttribute.Value);
            currentBatchNumItem.DataChangeReceived += HandleCurrentBatchNumChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(currentBatchNumItem);

            var batchesQuantityItem = new OpcMonitoredItem(_opc.cl.GetNode("PAR_BatchesQuantity"), OpcAttribute.Value);
            batchesQuantityItem.DataChangeReceived += HandleBatchesQuantityChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(batchesQuantityItem);

            var dosingProcessItem = new OpcMonitoredItem(_opc.cl.GetNode("CommonProgress"), OpcAttribute.Value);
            dosingProcessItem.DataChangeReceived += HandleDosingProcessChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(dosingProcessItem);
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

        private void HandleDosingProcessChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempDosingProcess = double.Parse(e.Item.Value.ToString());
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

        private double? _dosingProcess;
        public double? DosingProcess
        {
            get { return _dosingProcess; }
            set
            {
                _dosingProcess = value;
                OnPropertyChanged(nameof(DosingProcess));
            }
        }

        public void GetTable()
        {
            if (_id != 0)
            {
                BatchesQuantity = _tempBatchesQuantity;
                CurrentBatchNum = _tempCurrentBatchNum;
                DosingProcess = _tempDosingProcess;
                if (_id != _currentId)
                {
                    try
                    {
                        SelTaskName = ViewModelTabl.SelTaskName;
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
                SelTaskName = null;
                BatchesQuantity = null;
                CurrentBatchNum = null;
                DosingProcess = null;
                _currentId = _id;
            }
        }
    }
}