using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using AS_Library.Classes;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Annotations;
using AsuBetonLibrary.Elements;
using AsuBetonLibrary.Readers;
using AsuBetonLibrary.Services;
using _2048_Rbu.Classes;
using NLog;


namespace _2048_Rbu.Windows
{
    public partial class WindowTask : Window
    {
        public WindowTask(TaskQueueItemsService taskQueueItemsService, int maxOrder, Logger logger)
        {
            InitializeComponent();

            var newTaskViewModel = new NewTaskViewModel(taskQueueItemsService, maxOrder, logger);
            newTaskViewModel.Close += NewTaskViewModelOnClose;
            DataContext = newTaskViewModel;
        }

        private void NewTaskViewModelOnClose()
        {
            Close();
        }
    }

    public class NewTaskViewModel : INotifyPropertyChanged
    {
        public delegate void CloseWindowHandler();
        public event CloseWindowHandler Close;
        private RecipesReader RecipesReader { get; set; } = new RecipesReader();
        private CustomersService CustomersService { get; set; } = new CustomersService();
        private CustomersReader CustomersReader { get; set; } = new CustomersReader();
        private CommonOpcParametersReader CommonOpcParametersReader { get; set; } = new CommonOpcParametersReader();
        private TaskQueueItemsService TaskQueueItemsService { get; set; }

        private ObservableCollection<ApiRecipeGroup> _recipeGroups;
        public ObservableCollection<ApiRecipeGroup> RecipeGroups
        {
            get { return _recipeGroups; }
            set
            {
                _recipeGroups = value;
                OnPropertyChanged(nameof(RecipeGroups));
            }
        }

        private ApiRecipeGroup _selRecipeGroup;
        public ApiRecipeGroup SelRecipeGroup
        {
            get { return _selRecipeGroup; }
            set
            {
                _selRecipeGroup = value;
                OnPropertyChanged(nameof(SelRecipeGroup));
            }
        }

        private ApiRecipe _selRecipe;
        public ApiRecipe SelRecipe
        {
            get { return _selRecipe; }
            set
            {
                _selRecipe = value;
                OnPropertyChanged(nameof(SelRecipe));
                UpdateRecipeDetails();
            }
        }

        private ElRecipeDetailsViewModel _elRecipeDetailsViewModel;
        public ElRecipeDetailsViewModel ElRecipeDetailsViewModel
        {
            get { return _elRecipeDetailsViewModel; }
            set
            {
                _elRecipeDetailsViewModel = value;
                OnPropertyChanged(nameof(ElRecipeDetailsViewModel));
            }
        }

        private ObservableCollection<ApiCustomer> _customers;
        public ObservableCollection<ApiCustomer> Customers
        {
            get { return _customers; }
            set
            {
                _customers = value;
                OnPropertyChanged(nameof(Customers));
            }
        }

        private ApiCustomer _selCustomer;
        public ApiCustomer SelCustomer
        {
            get { return _selCustomer; }
            set
            {
                _selCustomer = value;
                OnPropertyChanged(nameof(SelCustomer));
            }
        }

        private ApiTask _task;
        public ApiTask Task
        {
            get { return _task; }
            set
            {
                _task = value;
                OnPropertyChanged(nameof(Task));
            }
        }

        private decimal _volume;
        public decimal Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                BatchesCalculation();
                OnPropertyChanged(nameof(Volume));
            }
        }

        private decimal _batchesAmount;
        public decimal BatchesAmount
        {
            get { return _batchesAmount; }
            set
            {
                _batchesAmount = value;
                OnPropertyChanged(nameof(BatchesAmount));
            }
        }

        private decimal _batchVolume;
        public decimal BatchVolume
        {
            get { return _batchVolume; }
            set
            {
                _batchVolume = value;
                OnPropertyChanged(nameof(BatchVolume));
            }
        }
        private void BatchesCalculation()
        {
            Task.Volume = Convert.ToDecimal(Math.Round(Volume, 2));
            BatchesAmount = Convert.ToInt16(Math.Ceiling(Math.Round(Volume / MixerVolume, 2)));
            BatchVolume = Convert.ToDecimal(Math.Round(Volume / BatchesAmount, 2));
        }

        private decimal MixerVolume { get; set; }
        private int MaxOrder { get; set; }
        private Logger Logger { get; set; }

        public NewTaskViewModel(TaskQueueItemsService taskQueueItemsService, int maxOrder, Logger logger)
        {
            Logger = logger;
            Task = new ApiTask();
            MixerVolume = GetMixerVolume();
            Volume = (decimal)0.2;
            MaxOrder = maxOrder;
            TaskQueueItemsService = taskQueueItemsService;
            UpdateRecipes();
            UpdateCustomers();
        }

        private decimal GetMixerVolume()
        {
            var mixerVolumeParam = CommonOpcParametersReader.GetCommonOpcParameterByName(OpcHelper.GetTagName(OpcHelper.TagNames.MixerVolume));
            if (mixerVolumeParam != null)
            {
                var mixerVolumeValue = OpcHelper.ReadTag(NewOpcServer.OpcList.Rbu, mixerVolumeParam.Tag);
                var mixerVolume = Convert.ToDecimal(OpcHelper.GetParameterValue(mixerVolumeValue, mixerVolumeParam));
                if (mixerVolume != 0)
                {
                    return mixerVolume;
                }

                Logger.Error("Объем миксера не может быть равен 0.");
                MessageBox.Show("Объем миксера не может быть равен 0.");
                return (decimal)0.2;
            }

            Logger.Error("Остутствует название тега - MixerVolume.");
            MessageBox.Show("Остутствует название тега - MixerVolume.");
            return (decimal)0.2;
        }

        private void UpdateRecipes()
        {
            var recipes = RecipesReader.ListRecipes();
            RecipeGroups = new ObservableCollection<ApiRecipeGroup>(recipes.GroupBy(x => x.Group)
                .Select(x => new ApiRecipeGroup
                {
                    Name = x.Key.Name,
                    Recipes = x.Select(p => p).ToList()
                }).ToList());
        }

        private void UpdateRecipeDetails()
        {
            ElRecipeDetailsViewModel = new ElRecipeDetailsViewModel(SelRecipe);
        }
        private void UpdateCustomers()
        {
            Customers = new ObservableCollection<ApiCustomer>(CustomersReader.ListCustomers());
        }
        private void UpdateSelCustomer()
        {
            var maxId = Customers.Max(x => x.Id);
            SelCustomer = Customers.FirstOrDefault(x => x.Id == maxId);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Commands

        private RelayCommand _addCustomerCommand;
        public RelayCommand AddCustomerCommand
        {
            get
            {
                return _addCustomerCommand ??= new RelayCommand((o) =>
                {
                    CustomersService.Add();
                    CustomersService.Updated += UpdateCustomers;
                    CustomersService.Updated += UpdateSelCustomer;
                });
            }
        }

        private RelayCommand _saveTaskCommand;
        public RelayCommand SaveTaskCommand
        {
            get
            {
                return _saveTaskCommand ??= new RelayCommand((o) =>
                {
                    Task.Volume = Volume;
                    Task.BatchesAmount = BatchesAmount;
                    Task.BatchVolume = BatchVolume;
                    Task.Recipe = SelRecipe;
                    Task.Customer = SelCustomer;
                    TaskQueueItemsService.Add(new ApiTaskQueueItem { Task = Task, Order = MaxOrder + 1 });
                    Close?.Invoke();
                });

            }
        }
        #endregion
    }
}

