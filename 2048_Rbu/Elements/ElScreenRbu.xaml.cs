using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using _2048_Rbu.Classes;
using AS_Library.Annotations;
using AS_Library.Link;
using Lib_2048.Classes;
using _2048_Rbu.Interfaces;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements
{
    /// <summary>
    /// Interaction logic for ElScreenRbu.xaml
    /// </summary>
    public partial class ElScreenRbu : INotifyPropertyChanged
    {
        OpcServer.OpcList _opcName;
        private OPC_client _opc;

        private readonly List<IElementsUpdater> _elementList = new List<IElementsUpdater>();

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

        //public async void Initialize(OpcServer.OpcList opcName)
        public void Initialize()
        {
            IsUpdating = true;

            _opcName = OpcServer.OpcList.Rbu;
            DataContext = this;

            //await Task.Run(() =>
            //{
            OpcServer.GetInstance().InitOpc(_opcName, "opc.tcp://192.168.100.70:49320");
            OpcServer.GetInstance().ConnectOpc(_opcName);
            EventsBase.GetInstance().CreateControlEvents(_opcName);
            //});

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
            //CreateSubscription();
            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();

            IsUpdating = false;
        }

        //private void CreateSubscription()
        //{
        //    _opc = OpcServer.GetInstance().GetOpc(_opcName);
        //    var visItem = new OpcMonitoredItem(_opc.cl.GetNode("_System._NoError"), OpcAttribute.Value);
        //    visItem.DataChangeReceived += HandleVisChanged;
        //    OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visItem);
        //}

        //private void HandleVisChanged(object sender, OpcDataChangeReceivedEventArgs e)
        //{
        //    Static.Link = bool.Parse(e.Item.Value.ToString());
        //    LinkMessage = !Static.Link;
        //}

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
