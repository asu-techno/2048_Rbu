using System;
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
    public partial class ElControl : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        private bool _running;
        public bool Running
        {
            get
            {
                return _running;
            }
            set
            {
                _running = value;
                OnPropertyChanged(nameof(Running));
            }
        }

        private bool _readyStart;
        public bool ReadyStart
        {
            get
            {
                return _readyStart;
            }
            set
            {
                _readyStart = value;
                OnPropertyChanged(nameof(ReadyStart));
            }
        }

        private bool _readyStop;
        public bool ReadyStop
        {
            get
            {
                return _readyStop;
            }
            set
            {
                _readyStop = value;
                OnPropertyChanged(nameof(ReadyStop));
            }
        }

        public ElControl()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;
            
            DataContext = this;
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
            var runningItem = new OpcMonitoredItem(_opc.cl.GetNode(""), OpcAttribute.Value);
            runningItem.DataChangeReceived += HandleRunningChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(runningItem);

            var rdyStartItem = new OpcMonitoredItem(_opc.cl.GetNode(""), OpcAttribute.Value);
            rdyStartItem.DataChangeReceived += HandleRdyStartChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(rdyStartItem);

            var rdyStopItem = new OpcMonitoredItem(_opc.cl.GetNode(""), OpcAttribute.Value);
            rdyStopItem.DataChangeReceived += HandleRdyStopChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(rdyStopItem);
        }

        private void HandleRunningChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                Running = bool.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleRdyStartChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                ReadyStart = bool.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleRdyStopChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                ReadyStop = bool.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnStart, "btn_Start", true, "Маршрут Старт");
        }

        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnStop, "btn_Stop", true, "Маршрут Стоп");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
