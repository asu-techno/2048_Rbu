using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AS_Library.Link;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using AsuBetonLibrary.Windows;
using Opc.UaFx;
using Opc.UaFx.Client;


namespace _2048_Rbu.Elements.Control
{
    public partial class ElDosingWait : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        private bool _vis;
        public bool Vis
        {
            get
            {
                return _vis;
            }
            set
            {
                _vis = value;
                OnPropertyChanged(nameof(Vis));
            }
        }

        private string _dosingName;
        public string DosingName
        {
            get
            {
                return _dosingName;
            }
            set
            {
                _dosingName = value;
                OnPropertyChanged(nameof(DosingName));
            }
        }

        public string TagContainer { get; set; }
        public string NameContainer { get; set; }
        public Static.ContainerItem ContainerItem { get; set; }

        public ElDosingWait()
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
            var visItem = new OpcMonitoredItem(_opc.cl.GetNode(TagContainer + ".Pause"), OpcAttribute.Value);
            visItem.DataChangeReceived += HandleVisChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visItem);
        }

        private void HandleVisChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                Vis = bool.Parse(e.Item.Value.ToString());
                GetName();
            }
            catch (Exception exception)
            {
            }
        }

        private void GetName()
        {
            try
            {
                if (ContainerItem != 0)
                {
                    DosingName = Static.СontainerNameDictionary[ContainerItem] + " (" + Static.СontainerMaterialDictionary[ContainerItem] + ")" + ". Дозирование";
                }
                else
                {
                    DosingName = NameContainer != null ? NameContainer : "Дозирование";
                }
            }
            catch (Exception exception)
            {
                DosingName = NameContainer != null ? NameContainer : "Дозирование";
            }
        }

        private void BtnContinue_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnContinue, TagContainer + ".btn_Reset", true, DosingName + ". Продолжить дозирование");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
