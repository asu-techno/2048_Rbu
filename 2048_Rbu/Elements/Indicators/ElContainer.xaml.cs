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
        private ContainersReader ContainersReader { get; set; } = new ContainersReader();

        private ObservableCollection<ApiContainer> _containers;
        public ObservableCollection<ApiContainer> Containers
        {
            get { return _containers; }
            set
            {
                _containers = value;
                OnPropertyChanged(nameof(Containers));
            }
        }

        private ApiContainer _selContainer;
        public ApiContainer SelContainer
        {
            get { return _selContainer; }
            set
            {
                _selContainer = value;
                OnPropertyChanged(nameof(SelContainer));
            }
        }

        public int ContainerId { get; set; }

        public ElContainer()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            DataContext = this;

            GetMaterial();
        }

        public async void GetMaterial()
        {
            await Task.Run(() =>
            {
                Containers = new ObservableCollection<ApiContainer>(ContainersReader.ListContainers());
                SelContainer = Containers.FirstOrDefault(x => x.Id == ContainerId);
            });
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
                WindowMatchingMaterials window = new WindowMatchingMaterials();
                window.Show();
            }
        }
    }
}
