using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using AS_Library.Classes;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using AsuBetonLibrary.Services;
using AsuBetonLibrary.Windows;
using _2048_Rbu.Handlers;
using _2048_Rbu.Windows;
using NLog;

namespace _2048_Rbu.Elements
{
    /// <summary>
    /// Interaction logic for ElTaskQueue.xaml
    /// </summary>
    public partial class ElTaskQueue : UserControl
    {
        public ElTaskQueue()
        {
            InitializeComponent();
        }
    }

    public sealed class ElTaskQueueViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Logger Logger { get; set; }
        private TaskQueueItemsService Service { get; set; }
        private RecipesReader RecipesReader { get; set; } = new RecipesReader();

        private ObservableCollection<ApiTaskQueueItem> _taskQueue;
        public ObservableCollection<ApiTaskQueueItem> TaskQueue
        {
            get { return _taskQueue; }
            set
            {
                _taskQueue = value;
                OnPropertyChanged(nameof(TaskQueue));
            }
        }

        private ApiTaskQueueItem _selTaskQueueItem;
        public ApiTaskQueueItem SelTaskQueueItem
        {
            get { return _selTaskQueueItem; }
            set
            {
                _selTaskQueueItem = value;
                OnPropertyChanged(nameof(SelTaskQueueItem));
            }
        }
        private bool _stopLoadTasks;
        public bool StopLoadTasks
        {
            get { return _stopLoadTasks; }
            set
            {
                _stopLoadTasks = value;
                OnPropertyChanged(nameof(StopLoadTasks));
            }
        }
        private int MaxOrder { get; set; }

        public delegate void NotStopLoadTasksHandler(bool stopLoadTasks);
        public NotStopLoadTasksHandler NotStopLoadTasks;


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ElTaskQueueViewModel(TaskQueueItemsService service, Logger logger)
        {
            Logger = logger;
            Service = service;
            Service.Updated += ServiceOnUpdate;
            ServiceOnUpdate();
            StopLoadTasks = true;
            var loadTaskHandler = new LoadTaskHandler(Service, logger);
            NotStopLoadTasks += loadTaskHandler.NotStopLoadTasks;
        }

        private void ServiceOnUpdate()
        {
            TaskQueue = new ObservableCollection<ApiTaskQueueItem>(Service.List());
            MaxOrder = TaskQueue.Any() ? TaskQueue.Max(x => x.Order) : 0;
        }

        #region Commands

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??= new RelayCommand((o) =>
                {
                    WindowTask window = new WindowTask(Service, MaxOrder, Logger);
                    window.Show();
                });
            }
        }

        private RelayCommand _upRecipeCommand;
        public RelayCommand UpRecipeCommand
        {
            get
            {
                return _upRecipeCommand ??= new RelayCommand((o) =>
                {
                    if (SelTaskQueueItem.Order > 0)
                    {
                        var recipeQueueItems = new List<ApiTaskQueueItem>();
                        var previousItem = TaskQueue.FirstOrDefault(x => x.Order == SelTaskQueueItem.Order - 1);
                        if (previousItem != null)
                        {
                            previousItem.Order += 1;
                            recipeQueueItems.Add(previousItem);
                        }
                        SelTaskQueueItem.Order -= 1;
                        recipeQueueItems.Add(SelTaskQueueItem);
                        Service.Update(recipeQueueItems);
                    }
                });
            }
        }

        private RelayCommand _downRecipeCommand;
        public RelayCommand DownRecipeCommand
        {
            get
            {
                return _downRecipeCommand ??= new RelayCommand((o) =>
                {
                    if (SelTaskQueueItem.Order < MaxOrder)
                    {
                        var recipeQueueItems = new List<ApiTaskQueueItem>();
                        var nextItem = TaskQueue.FirstOrDefault(x => x.Order == SelTaskQueueItem.Order + 1);
                        if (nextItem != null)
                        {
                            nextItem.Order -= 1;
                            recipeQueueItems.Add(nextItem);
                        }
                        SelTaskQueueItem.Order += 1;
                        recipeQueueItems.Add(SelTaskQueueItem);
                        Service.Update(recipeQueueItems);
                    }
                });
            }
        }

        private RelayCommand _detailsCommand;
        public RelayCommand DetailsCommand
        {
            get
            {
                return _detailsCommand ??= new RelayCommand((o) =>
                {
                    var recipe = RecipesReader.GetById(SelTaskQueueItem.Task.Recipe.Id);
                    var task = SelTaskQueueItem.Task;
                    task.Recipe = recipe;
                    WindowTaskDetails window = new WindowTaskDetails(task);
                    window.Show();
                });
            }
        }

        private RelayCommand _copyCommand;
        public RelayCommand CopyCommand
        {
            get
            {
                return _copyCommand ??= new RelayCommand((o) =>
                {
                    SelTaskQueueItem.Task.Id = 0;
                    var recipeQueueItem = new ApiTaskQueueItem { Task = SelTaskQueueItem.Task, Order = MaxOrder + 1 };
                    Service.Add(recipeQueueItem);
                });
            }
        }

        private RelayCommand _deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new RelayCommand((o) =>
                {
                    Service.Delete(SelTaskQueueItem.Id);
                });
            }
        }

        private RelayCommand _stopLoadTaskCommand;
        public RelayCommand StopLoadTaskCommand
        {
            get
            {
                return _stopLoadTaskCommand ??= new RelayCommand((o) =>
                {
                    StopLoadTasks = !StopLoadTasks;
                    NotStopLoadTasks?.Invoke(StopLoadTasks);
                });
            }
        }
        #endregion
    }
}
