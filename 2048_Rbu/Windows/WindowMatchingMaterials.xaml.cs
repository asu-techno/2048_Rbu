using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using _2048_Rbu.Classes;
using AS_Library.Classes;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Annotations;
using AsuBetonLibrary.Readers;
using AsuBetonLibrary.Repositories;
using NLog;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Interaction logic for WindowMatchingMaterials.xaml
    /// </summary>
    public partial class WindowMatchingMaterials : Window
    {
        public WindowMatchingMaterials()
        {
            InitializeComponent();

            var matchingMaterialsViewModel = new MatchingMaterialsViewModel();
            DataContext = matchingMaterialsViewModel;
        }

        public WindowMatchingMaterials(Static.ContainerItem containerItem)
        {
            InitializeComponent();

            var matchingMaterialsViewModel = new MatchingMaterialsViewModel(containerItem);
            DataContext = matchingMaterialsViewModel;
        }
    }

    public class MatchingMaterialsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MaterialsReader MaterialsReader { get; set; } = new MaterialsReader();
        private ContainersReader ContainersReader { get; set; } = new ContainersReader();
        private ContainersRepository ContainersRepository { get; set; } = new ContainersRepository();

        private ObservableCollection<ContainerMaterialsViewModel> _containerMaterialsViewModels;
        public ObservableCollection<ContainerMaterialsViewModel> ContainerMaterialsViewModels
        {
            get { return _containerMaterialsViewModels; }
            set
            {
                _containerMaterialsViewModels = value;
                OnPropertyChanged(nameof(ContainerMaterialsViewModels));
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

        public MatchingMaterialsViewModel()
        {
            UpdateContainerMaterials();
        }

        public MatchingMaterialsViewModel(Static.ContainerItem containerItem)
        {
            UpdateContainerMaterials(containerItem);
        }

        private void UpdateContainerMaterials()
        {
            ContainerMaterialsViewModels = new ObservableCollection<ContainerMaterialsViewModel>(GetContainerMaterialsViewModels());
        }

        private void UpdateContainerMaterials(Static.ContainerItem containerItem)
        {
            ContainerMaterialsViewModels = new ObservableCollection<ContainerMaterialsViewModel>(GetContainerMaterialsViewModels(containerItem));
        }

        private List<ContainerMaterialsViewModel> GetContainerMaterialsViewModels()
        {
            var recipeMaterials = new List<ContainerMaterialsViewModel>();
            var containers = ContainersReader.ListContainers();
            foreach (var container in containers)
            {
                if (container.ContainerType?.MaterialType != null)
                {
                    var materialTypeId = container.ContainerType.MaterialType.Id;
                    var materials =
                                  new ObservableCollection<ApiMaterial>(
                                      MaterialsReader.ListMaterialsByMaterialTypeId(materialTypeId));
                    materials.Insert(0, new ApiMaterial { Name = "Нет материала" });
                    var comboboxViewModel = new ContainerMaterialsViewModel()
                    {
                        Container = container,
                        Materials = materials,
                        SelMaterial = materials.FirstOrDefault(x => container.CurrentMaterial != null && x.Id == container.CurrentMaterial.Id)
                                      ?? materials.First(),
                    };
                    recipeMaterials.Add(comboboxViewModel);
                }
            }

            return recipeMaterials;
        }

        private List<ContainerMaterialsViewModel> GetContainerMaterialsViewModels(Static.ContainerItem containerItem)
        {
            var recipeMaterials = new List<ContainerMaterialsViewModel>();
            var containers = ContainersReader.ListContainers();
            var container = containers.FirstOrDefault(x => x.Id == Static.IdСontainerDictionary[containerItem]);

            if (container.ContainerType?.MaterialType != null)
            {
                var materialTypeId = container.ContainerType.MaterialType.Id;
                var materials =
                    new ObservableCollection<ApiMaterial>(
                        MaterialsReader.ListMaterialsByMaterialTypeId(materialTypeId));
                materials.Insert(0, new ApiMaterial { Name = "Нет материала" });
                var comboboxViewModel = new ContainerMaterialsViewModel()
                {
                    Container = container,
                    Materials = materials,
                    SelMaterial = materials.FirstOrDefault(x => container.CurrentMaterial != null && x.Id == container.CurrentMaterial.Id)
                                  ?? materials.First(),
                };
                recipeMaterials.Add(comboboxViewModel);
            }

            return recipeMaterials;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Commands

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand((o) =>
                {
                    var containers = new List<ApiContainer>();
                    foreach (var containerMaterialsViewModel in ContainerMaterialsViewModels)
                    {
                        if (containerMaterialsViewModel.Container.CurrentMaterial?.Id != containerMaterialsViewModel.SelMaterial.Id)
                        {
                            containerMaterialsViewModel.Container.CurrentMaterial =
                                containerMaterialsViewModel.SelMaterial;
                            containers.Add(containerMaterialsViewModel.Container);
                        }
                    }

                    var allMaterials = ContainerMaterialsViewModels.Select(x => x.Container.CurrentMaterial).Where(x => x.Id != 0).ToList();
                    if (allMaterials.GroupBy(x => x.Name).Any(g => g.Count() > 1))
                    {
                        MessageBox.Show("Материалы не сохранились. В емкостях не может быть одинаковых материалов.");
                        UpdateContainerMaterials();
                    }
                    else
                    {
                        ContainersRepository.SaveList(containers);
                    }
                });
            }
        }

        #endregion
    }

    public class ContainerMaterialsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ApiMaterial> _materials;
        public ObservableCollection<ApiMaterial> Materials
        {
            get { return _materials; }
            set
            {
                _materials = value;
                OnPropertyChanged(nameof(Materials));
            }
        }

        private ApiMaterial _selMaterial;
        public ApiMaterial SelMaterial
        {
            get { return _selMaterial; }
            set
            {
                _selMaterial = value;
                OnPropertyChanged(nameof(SelMaterial));
            }
        }

        private ApiContainer _container;
        public ApiContainer Container
        {
            get { return _container; }
            set
            {
                _container = value;
                OnPropertyChanged(nameof(Container));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}