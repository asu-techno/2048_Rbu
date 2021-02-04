using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AS_Library.Link;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Control
{
    public partial class ElStatus : INotifyPropertyChanged, IElementsUpdater
    {
        private OpcServer.OpcList _opcName;

        private Dictionary<int, string> _tagsDict = new Dictionary<int, string>
        {
            {0, "DI_NotStop_BTN_PLC"},
            {1, "DI_NotStop_BTN_Inert"},
            {2, "DI_NotStop_BTN_Doz"},
            {3, "DI_NotStop_BTN_Cem"},
            {4, "DI_NotStop_BTN_Water"},
            {5, "DI_NotStop_BTN_BS"},
            {6, "gb_AL_Hydro_Feedback"},
            {7, "gb_AL_Hydro_Pressure"},
            {8, "M_18.gb_Warning_Oil"},
            {9, "DI_M18_sw_AutomatMode"},
            {10, "DI_M9_sw_AutomatMode"}
        };

        private ObservableCollection<StatusArray> _statusArray;
        public ObservableCollection<StatusArray> StatusArray
        {
            get
            {
                return _statusArray;
            }
            set
            {
                _statusArray = value;
                OnPropertyChanged(nameof(StatusArray));
            }
        }

        public ElStatus()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            StatusArray = new ObservableCollection<StatusArray>();
            for (int i = 0; i < _tagsDict.Count; i++)
            {
                if (_tagsDict.ContainsKey(i))
                    StatusArray.Add(new StatusArray(_opcName, _tagsDict[i]));
            }

            DataContext = this;
        }

        public void Subscribe()
        {
            foreach (var item in StatusArray)
            {
                item.Subscribe();
            }
        }
        public void Unsubscribe()
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class StatusArray : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        private string _tag;

        public StatusArray(OpcServer.OpcList opcName, string tag)
        {
            _opcName = opcName;
            _tag = tag;
        }

        private bool _statusItem;
        public bool StatusItem
        {
            get { return _statusItem; }
            set
            {
                _statusItem = value;
                OnPropertyChanged(nameof(StatusItem));
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
            var statusItem = new OpcMonitoredItem(_opc.cl.GetNode(_tag), OpcAttribute.Value);
            statusItem.DataChangeReceived += HandleStatusItemChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(statusItem);
        }

        private void HandleStatusItemChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                StatusItem = bool.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }
    }
}
