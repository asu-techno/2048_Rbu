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
    public partial class ElContainer : INotifyPropertyChanged
    {
        private static ContainersReader ContainersReader { get; set; } = new ContainersReader();

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

        public Static.ContainerItem ContainerItem { get; set; }

        public ElContainer()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            DataContext = this;

            GetContainerMaterial();
            GetMaterialName();
        }

        public static void GetContainerMaterial()
        {
            try
            {
                var containers = new ObservableCollection<ApiContainer>(ContainersReader.ListContainers());

                foreach (var item in containers)
                {
                    if (Static.IdСontainerDictionary.ContainsValue(item.Id))
                        Static.СontainerMaterialDictionary[Static.IdСontainerDictionary.FirstOrDefault(x=>x.Value== item.Id).Key] = item.CurrentMaterial != null ? (item.CurrentMaterial.Name != null ? item.CurrentMaterial.Name : "") : "";
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Double_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Windows.WindowMatchingMaterials window = new Windows.WindowMatchingMaterials(ContainerItem);
                window.Show();
            }
        }
    }
}
