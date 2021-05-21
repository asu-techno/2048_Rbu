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

namespace _2048_Rbu.Elements.Indicators
{
    public partial class ElContainer : IElementsUpdater, INotifyPropertyChanged
    {
        private static ContainersReader ContainersReader { get; set; } = new ContainersReader();

        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        private string _nameContainerMaterial;
        public string NameContainerMaterial
        {
            get { return _nameContainerMaterial; }
            set
            {
                _nameContainerMaterial = value;
                OnPropertyChanged(nameof(NameContainerMaterial));
            }
        }

        private bool _permitDos;
        public bool PermitDos
        {
            get { return _permitDos; }
            set
            {
                _permitDos = value;
                OnPropertyChanged(nameof(PermitDos));
            }
        }

        public string PermitTag { get; set; }
        public Static.ContainerItem ContainerItem { get; set; }

        public ElContainer()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            DataContext = this;

            _opcName = opcName;

            GetContainerMaterial();
            GetMaterialName();
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
            if (!string.IsNullOrEmpty(PermitTag))
            {
                _opc = OpcServer.GetInstance().GetOpc(_opcName);
                var permitDos = new OpcMonitoredItem(_opc.cl.GetNode(PermitTag), OpcAttribute.Value);
                permitDos.DataChangeReceived += HandlePermitDosChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(permitDos);
            }
        }

        private void HandlePermitDosChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                PermitDos = bool.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        public static void GetContainerMaterial()
        {
            try
            {
                var containers = new ObservableCollection<ApiContainer>(ContainersReader.ListContainers());

                foreach (var item in containers)
                {
                    if (Static.IdСontainerDictionary.ContainsValue(item.Id))
                        Static.СontainerMaterialDictionary[Static.IdСontainerDictionary.FirstOrDefault(x => x.Value == item.Id).Key] = item.CurrentMaterial != null ? (item.CurrentMaterial.Name != null ? item.CurrentMaterial.Name : "") : "";
                }
            }
            catch (Exception e)
            {

            }

        }

        public void GetMaterialName()
        {
            NameContainerMaterial = Static.СontainerMaterialDictionary[ContainerItem];
        }

        private void Rect_OnMouseEnter(object sender, MouseEventArgs e)
        {
            RectObject.Opacity = 1;
        }

        private void Rect_OnMouseLeave(object sender, MouseEventArgs e)
        {
            RectObject.Opacity = 0;
        }

        private void Double_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ClickCount == 2)
                {
                    Windows.WindowMatchingMaterials window = new Windows.WindowMatchingMaterials(ContainerItem);
                    window.Show();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Ошибка");
            }
        }

        private void RectObject_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PermitDos)
                Methods.ButtonClick(PermitTag, false, NameContainerMaterial+". Запрет дозирования");
            else
                Methods.ButtonClick(PermitTag, true, NameContainerMaterial + ". Разрешение дозирования");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
