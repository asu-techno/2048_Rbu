using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using AS_Library.Link;
using System.ComponentModel;
using _2048_Rbu.Classes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using AsuBetonLibrary.Services;
using Opc.UaFx;
using Opc.UaFx.Client;
using AS_Library.Classes;
using System.Windows;
using AS_Library.Events.Classes;

namespace _2048_Rbu.Elements.Control
{
    public partial class ElTabl : IElementsUpdater
    {
        public ViewModelTabl _viewmodelTabl;

        public ElTabl()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _viewmodelTabl = new ViewModelTabl(opcName);
            DataContext = _viewmodelTabl;

        }

        public void Subscribe()
        {
            _viewmodelTabl.Subscribe();
        }

        public void Unsubscribe()
        {
        }
    }

    public sealed class ViewModelTabl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TasksReader TasksReader { get; set; } = new TasksReader();
        public static ApiTask CurrentSelTask { get; set; }
        private RecipesReader RecipesReader { get; set; } = new RecipesReader();
        //private ContainersReader ContainersReader { get; set; } = new ContainersReader();
        //private ObservableCollection<ApiRecipeMaterial> _materials;
        //private ObservableCollection<ApiContainer> _containers;

        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private long _id, _currentId;
        //private int _batchNum;
        //private string[,] _materialTags;

        private long _taskId;
        public long TaskId
        {
            get { return _taskId; }
            set
            {
                _taskId = value;
                OnPropertyChanged(nameof(TaskId));
            }
        }

        public ViewModelTabl(OpcServer.OpcList opcName)
        {
            _opcName = opcName;
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
            var idItem = new OpcMonitoredItem(_opc.cl.GetNode("TaskID"), OpcAttribute.Value);
            idItem.DataChangeReceived += HandleIdChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(idItem);
            
            //var batchNumItem = new OpcMonitoredItem(_opc.cl.GetNode("Current_batchNum"), OpcAttribute.Value);
            //batchNumItem.DataChangeReceived += HandleBatchNumChanged;
            //OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(batchNumItem);
            //var componentWights = new OpcMonitoredItem(_opc.cl.GetNode("ComponentsWeight"), OpcAttribute.Value);
            //componentWights.DataChangeReceived += HandleComponentsWeightChanged;
            //OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(componentWights);
        }

        private void HandleIdChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                TaskId = _id = long.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        //private void HandleBatchNumChanged(object sender, OpcDataChangeReceivedEventArgs e)
        //{
        //    try
        //    {
        //        _batchNum = int.Parse(e.Item.Value.ToString());
        //        GetTags();
        //    }
        //    catch (Exception exception)
        //    {
        //    }
        //}

        //private void HandleComponentsWeightChanged(object sender, OpcDataChangeReceivedEventArgs e)
        //{
        //    try
        //    {
        //        ComponentsWeight = double.Parse(e.Item.Value.ToString());
        //    }
        //    catch (Exception exception)
        //    {
        //    }
        //}

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<ApiTask> _tasks;
        public ObservableCollection<ApiTask> Tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        private ApiTask _selTask;
        public ApiTask SelTask
        {
            get { return _selTask; }
            set
            {
                _selTask = value;
                OnPropertyChanged(nameof(SelTask));
            }
        }

        private string _selTaskGroupName;
        public string SelTaskGroupName
        {
            get { return _selTaskGroupName; }
            set
            {
                _selTaskGroupName = value;
                OnPropertyChanged(nameof(SelTaskGroupName));
            }
        }

        private string _selTaskName;
        public string SelTaskName
        {
            get { return _selTaskName; }
            set
            {
                _selTaskName = value;
                OnPropertyChanged(nameof(SelTaskName));
            }
        }

        private string _selTaskVolume;
        public string SelTaskVolume
        {
            get { return _selTaskVolume; }
            set
            {
                _selTaskVolume = value;
                OnPropertyChanged(nameof(SelTaskVolume));
            }
        }

        //private ObservableCollection<ViewDosingTask> _dosingTask;
        //public ObservableCollection<ViewDosingTask> DosingTask
        //{
        //    get { return _dosingTask; }
        //    set
        //    {
        //        _dosingTask = value;
        //        OnPropertyChanged(nameof(DosingTask));
        //    }
        //}

        //private double _componentsWeight;
        //public double ComponentsWeight
        //{
        //    get { return _componentsWeight; }
        //    set
        //    {
        //        _componentsWeight = value;
        //        OnPropertyChanged(nameof(ComponentsWeight));
        //    }
        //}

        public void GetTable()
        {
            try
            {
                if (_id != 0)
                {
                    if (_id != _currentId)
                    {
                        GetTask(_id);
                        //GetMaterials();
                        //GetTags();
                        _currentId = _id;
                    }
                    //GetValue();
                }
                else
                {
                    SelTaskVolume = null;
                    SelTaskGroupName = null;
                    SelTaskName = null;
                    CurrentSelTask = null;
                    Tasks = null;
                    SelTask = null;
                    //DosingTask = null;
                    _currentId = _id;
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"log.txt", DateTime.Now + " - " + ex.Message + "->" + _id);
            }
        }

        private void GetTask(long id)
        {
            Tasks = new ObservableCollection<ApiTask>(TasksReader.ListTasks());
            SelTask = Tasks.FirstOrDefault(x => x?.Id == id);
            var recipes = new ObservableCollection<ApiRecipe>(RecipesReader.ListRecipes());
            var selRecipe = recipes.FirstOrDefault(x => x?.Id == SelTask.Recipe.Id);
            SelTaskGroupName = selRecipe.Group.Name;
            SelTaskName = SelTask.Id + " - " + SelTask.Recipe.Name;
            var ending = SelTask.BatchesAmount == 1 ? "замес" : SelTask.BatchesAmount > 4 ? "замесов" : "замеса";
            SelTaskVolume = SelTask.Volume + " м³" + " (" + SelTask.BatchesAmount + " " + ending + " × " + SelTask.BatchVolume + " м³)";
            CurrentSelTask = SelTask;
        }

        #region Command
        private RelayCommand _unloadTask;
        public RelayCommand UnloadTask
        {
            get
            {
                return _unloadTask ??= new RelayCommand((o) =>
                {
                    try
                    {
                        _opc.cl.WriteBool("btn_UnloadRecipe", true, out var err);
                        EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent("Выгрузка текущего рецепта из ПЛК", SystemEventType.UserDoing);
                        if (err)
                            MessageBox.Show("Ошибка записи", "Ошибка");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Ошибка записи", "Ошибка");
                    }
                });
            }
        }

        #endregion

        //private void GetMaterials()
        //{
        //    if (SelTask != null)
        //    {
        //        var recipes = new ObservableCollection<ApiRecipe>(RecipesReader.ListRecipes());
        //        var selRecipe = recipes.FirstOrDefault(x => x.Id == SelTask.Recipe.Id);
        //        _materials = new ObservableCollection<ApiRecipeMaterial>(selRecipe.RecipeMaterials);
        //        _containers = new ObservableCollection<ApiContainer>(ContainersReader.ListContainers());

        //        DosingTask = new ObservableCollection<ViewDosingTask>();
        //        _materialTags = new string[_materials.Count, 3];

        //        for (int i = 0; i < _materials.Count; i++)
        //        {
        //            DosingTask.Add(new ViewDosingTask());

        //            if (_materials[i].Material.Name != null)
        //                DosingTask[i].MaterialName = _materials[i].Material.Name;
        //            else
        //                DosingTask[i].MaterialName = "";
        //        }
        //    }
        //}

        //private void GetTags()
        //{
        //    if (SelTask != null)
        //    {
        //        for (int i = 0; i < _materials.Count; i++)
        //        {
        //            var selContainer = _containers.FirstOrDefault(x => x.CurrentMaterial?.Id == _materials[i].Material.Id);
        //            if (selContainer != null)
        //            {
        //                switch (selContainer.Id)
        //                {
        //                    case 1:
        //                        _materialTags[i, 0] = "PAR_Inert_Bunker_1_set";
        //                        _materialTags[i, 1] = "Reports[1].Batches[" + _batchNum +
        //                                              "].Batcher_inert.Bunker1.Dozing_DozedValue";
        //                        _materialTags[i, 2] = "Progress_Components[6]";
        //                        break;
        //                    case 2:
        //                        _materialTags[i, 0] = "PAR_Inert_Bunker_2_set";
        //                        _materialTags[i, 1] = "Reports[1].Batches[" + _batchNum +
        //                                              "].Batcher_inert.Bunker2.Dozing_DozedValue";
        //                        _materialTags[i, 2] = "Progress_Components[7]";
        //                        break;
        //                    case 3:
        //                        _materialTags[i, 0] = "PAR_Inert_Bunker_3_set";
        //                        _materialTags[i, 1] = "Reports[1].Batches[" + _batchNum +
        //                                              "].Batcher_inert.Bunker3.Dozing_DozedValue";
        //                        _materialTags[i, 2] = "Progress_Components[8]";
        //                        break;
        //                    case 4:
        //                        _materialTags[i, 0] = "PAR_Inert_Bunker_4_set";
        //                        _materialTags[i, 1] = "Reports[1].Batches[" + _batchNum +
        //                                              "].Batcher_inert.Bunker4.Dozing_DozedValue";
        //                        _materialTags[i, 2] = "Progress_Components[9]";
        //                        break;
        //                    case 5:
        //                        _materialTags[i, 0] = "PAR_Cement_Silos_1_set";
        //                        _materialTags[i, 1] = "Reports[1].Batches[" + _batchNum +
        //                                              "].Batcher_cement.Silos1.Dozing_DozedValue";
        //                        _materialTags[i, 2] = "Progress_Components[3]";
        //                        break;
        //                    case 6:
        //                        _materialTags[i, 0] = "PAR_Cement_Silos_2_set";
        //                        _materialTags[i, 1] = "Reports[1].Batches[" + _batchNum +
        //                                              "].Batcher_cement.Silos2.Dozing_DozedValue";
        //                        _materialTags[i, 2] = "Progress_Components[4]";
        //                        break;
        //                    case 7:
        //                        _materialTags[i, 0] = "PAR_Water_set";
        //                        _materialTags[i, 1] = "Reports[1].Batches[" + _batchNum +
        //                                              "].Batcher_water.Water.Dozing_DozedValue";
        //                        _materialTags[i, 2] = "Progress_Components[5]";
        //                        break;
        //                    case 8:
        //                        _materialTags[i, 0] = "PAR_Additive_Tank_1_set";
        //                        _materialTags[i, 1] = "Reports[1].Batches[" + _batchNum +
        //                                              "].Batcher_add.Tank1.Dozing_DozedValue";
        //                        _materialTags[i, 2] = "Progress_Components[1]";
        //                        break;
        //                    case 9:
        //                        _materialTags[i, 0] = "PAR_Additive_Tank_2_set";
        //                        _materialTags[i, 1] = "Reports[1].Batches[" + _batchNum +
        //                                              "].Batcher_add.Tank2.Dozing_DozedValue";
        //                        _materialTags[i, 2] = "Progress_Components[2]";
        //                        break;
        //                    default:
        //                        _materialTags[i, 0] = "";
        //                        _materialTags[i, 1] = "";
        //                        _materialTags[i, 2] = "";
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                _materialTags[i, 0] = "";
        //                _materialTags[i, 1] = "";
        //                _materialTags[i, 2] = "";
        //            }
        //        }
        //    }
        //}

        //public void GetValue()
        //{
        //    if (SelTask != null)
        //    {
        //        for (int i = 0; i < _materials.Count; i++)
        //        {
        //            DosingTask[i].SetValue = Math.Round(_opc.cl.ReadReal(_materialTags[i, 0], out var errSet), 1).ToString("F1");
        //            DosingTask[i].CurrentValue = Math.Round(_opc.cl.ReadReal(_materialTags[i, 1], out var errCurrent), 1).ToString("F1");
        //            DosingTask[i].CurrentProgress = _opc.cl.ReadReal(_materialTags[i, 2], out var errProgress);
        //            if (errSet)
        //                DosingTask[i].SetValue = "";
        //            if (errCurrent)
        //                DosingTask[i].CurrentValue = "";
        //            if (errProgress)
        //                DosingTask[i].CurrentProgress = 0;

        //            DosingTask[i].ProgressDone = DosingTask[i].CurrentProgress == ComponentsWeight;
        //        }
        //    }
        //}
    }

    //public sealed class ViewDosingTask : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    private double _currentProgress;
    //    public double CurrentProgress
    //    {
    //        get { return _currentProgress; }
    //        set
    //        {
    //            _currentProgress = value;
    //            OnPropertyChanged(nameof(CurrentProgress));
    //        }
    //    }

    //    private bool _progressDone;
    //    public bool ProgressDone
    //    {
    //        get { return _progressDone; }
    //        set
    //        {
    //            _progressDone = value;
    //            OnPropertyChanged(nameof(ProgressDone));
    //        }
    //    }

    //    private string _materialName;
    //    public string MaterialName
    //    {
    //        get { return _materialName; }
    //        set
    //        {
    //            _materialName = value;
    //            OnPropertyChanged(nameof(MaterialName));
    //        }
    //    }

    //    private string _setValue;
    //    public string SetValue
    //    {
    //        get { return _setValue; }
    //        set
    //        {
    //            _setValue = value;
    //            OnPropertyChanged(nameof(SetValue));
    //        }
    //    }

    //    private string _currentValue;
    //    public string CurrentValue
    //    {
    //        get { return _currentValue; }
    //        set
    //        {
    //            _currentValue = value;
    //            OnPropertyChanged(nameof(CurrentValue));
    //        }
    //    }

    //    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}
}
