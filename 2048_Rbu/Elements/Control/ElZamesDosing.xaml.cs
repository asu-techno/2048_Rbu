using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using AS_Library.Link;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using DevExpress.Data.Browsing;
using Opc.UaFx;
using Opc.UaFx.Client;


namespace _2048_Rbu.Elements.Control
{
    public partial class ElZamesDosing : IElementsUpdater
    {
        public ViewModelDosing _viewModelDosing;

        public ElZamesDosing()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _viewModelDosing = new ViewModelDosing(opcName);
            DataContext = _viewModelDosing;

        }

        public void Subscribe()
        {
            _viewModelDosing.Subscribe();
        }

        public void Unsubscribe()
        {
        }
    }

    public sealed class ViewModelDosing : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private RecipesReader RecipesReader { get; set; } = new RecipesReader();
        private ContainersReader ContainersReader { get; set; } = new ContainersReader();
        private ObservableCollection<ApiRecipeMaterial> _materials;
        private ObservableCollection<ApiContainer> _containers;
        static object lockerValue = new object();
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private long _id, _currentId;
        private string[,] _materialTags;

        private int _tempCurrentBatchNum, _tempBatchesQuantity;
        private double _tempDosingProcess;

        public ViewModelDosing(OpcServer.OpcList opcName)
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

            var currentBatchNumItem = new OpcMonitoredItem(_opc.cl.GetNode("Current_batchNum"), OpcAttribute.Value);
            currentBatchNumItem.DataChangeReceived += HandleCurrentBatchNumChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(currentBatchNumItem);

            var batchesQuantityItem = new OpcMonitoredItem(_opc.cl.GetNode("PAR_BatchesQuantity"), OpcAttribute.Value);
            batchesQuantityItem.DataChangeReceived += HandleBatchesQuantityChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(batchesQuantityItem);

            var dosingProcessItem = new OpcMonitoredItem(_opc.cl.GetNode("CommonProgress"), OpcAttribute.Value);
            dosingProcessItem.DataChangeReceived += HandleDosingProcessChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(dosingProcessItem);

            var componentWights = new OpcMonitoredItem(_opc.cl.GetNode("ComponentsWeight"), OpcAttribute.Value);
            componentWights.DataChangeReceived += HandleComponentsWeightChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(componentWights);

        }

        private void HandleIdChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _id = long.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleCurrentBatchNumChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempCurrentBatchNum = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleBatchesQuantityChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempBatchesQuantity = int.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleDosingProcessChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _tempDosingProcess = double.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleComponentsWeightChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                ComponentsWeight = double.Parse(e.Item.Value.ToString());
                ProgressBrush = ComponentsWeight == 0 ? System.Windows.Media.Brushes.White : (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF85FC84"));
            }
            catch (Exception exception)
            {
            }
        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int? _currentBatchNum;
        public int? CurrentBatchNum
        {
            get { return _currentBatchNum; }
            set
            {
                _currentBatchNum = value;
                OnPropertyChanged(nameof(CurrentBatchNum));
            }
        }

        private int? _batchesQuantity;
        public int? BatchesQuantity
        {
            get { return _batchesQuantity; }
            set
            {
                _batchesQuantity = value;
                OnPropertyChanged(nameof(BatchesQuantity));
            }
        }

        private double? _dosingProcess;
        public double? DosingProcess
        {
            get { return _dosingProcess; }
            set
            {
                _dosingProcess = value;
                OnPropertyChanged(nameof(DosingProcess));
            }
        }

        private double _componentsWeight;
        public double ComponentsWeight
        {
            get { return _componentsWeight; }
            set
            {
                _componentsWeight = value;
                OnPropertyChanged(nameof(ComponentsWeight));
            }
        }

        private SolidColorBrush _progressBrush;
        public SolidColorBrush ProgressBrush
        {
            get { return _progressBrush; }
            set
            {
                _progressBrush = value;
                OnPropertyChanged(nameof(ProgressBrush));
            }
        }

        private ObservableCollection<ViewDosingTask> _dosingTask;
        public ObservableCollection<ViewDosingTask> DosingTask
        {
            get { return _dosingTask; }
            set
            {
                _dosingTask = value;
                OnPropertyChanged(nameof(DosingTask));
            }
        }

        public async void GetTable()
        {
            await Task.Run(() =>
            {
                lock (lockerValue)
                {
                    try
                    {
                        if (_id != 0)
                        {
                            if (_id != _currentId || ViewModelTabl.CurrentSelTask == null)
                            {
                                GetMaterials();
                                _currentId = _id;
                            }
                            if ((_tempCurrentBatchNum != CurrentBatchNum) || (_tempCurrentBatchNum != 0 && CurrentBatchNum == null))
                                GetTags();
                            CurrentBatchNum = _tempCurrentBatchNum;
                            BatchesQuantity = _tempBatchesQuantity;
                            DosingProcess = _tempDosingProcess;

                            GetValue();
                        }
                        else
                        {
                            DosingTask = null;
                            BatchesQuantity = null;
                            CurrentBatchNum = null;
                            DosingProcess = null;
                            _currentId = _id;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.AppendAllText(@"log.txt", DateTime.Now + " - " + ex.Message + "->" + _id);
                    }
                }
            });
        }

        private void GetMaterials()
        {
            if (ViewModelTabl.CurrentSelTask != null)
            {
                var recipes = new ObservableCollection<ApiRecipe>(RecipesReader.ListRecipes());
                var selRecipe = recipes.FirstOrDefault(x => x.Id == ViewModelTabl.CurrentSelTask.Recipe.Id);
                _materials = new ObservableCollection<ApiRecipeMaterial>(selRecipe.RecipeMaterials);
                _containers = new ObservableCollection<ApiContainer>(ContainersReader.ListContainers());

                DosingTask = new ObservableCollection<ViewDosingTask>();
                _materialTags = new string[_materials.Count, 4];

                for (int i = 0; i < _materials.Count; i++)
                {
                    DosingTask.Add(new ViewDosingTask());

                    if (_materials[i].Material.Name != null)
                        DosingTask[i].MaterialName = _materials[i].Material.Name;
                    else
                        DosingTask[i].MaterialName = "";
                }
            }
        }

        private void GetTags()
        {
            if (ViewModelTabl.CurrentSelTask != null && _materials != null)
            {
                for (int i = 0; i < _materials.Count; i++)
                {
                    var selContainer = _containers.FirstOrDefault(x => x.CurrentMaterial?.Id == _materials[i].Material.Id);
                    _materialTags[i, 0] = "";
                    _materialTags[i, 1] = "";
                    _materialTags[i, 2] = "";
                    _materialTags[i, 3] = "";
                    if (selContainer != null)
                    {

                        if (selContainer.Id == Static.IdСontainerDictionary[Static.ContainerItem.Bunker1])
                        {
                            _materialTags[i, 0] = "PAR_Inert_Bunker_1_set";
                            _materialTags[i, 1] = "Reports[1].Batches[" + _tempCurrentBatchNum +
                                                  "].Batcher_inert.Bunker1.Dozing_DozedValue";
                            _materialTags[i, 2] = "Progress_Components[6]";
                            _materialTags[i, 3] = "InertDozing.Tank_1_ON";
                        }
                        if (selContainer.Id == Static.IdСontainerDictionary[Static.ContainerItem.Bunker2])
                        {
                            _materialTags[i, 0] = "PAR_Inert_Bunker_2_set";
                            _materialTags[i, 1] = "Reports[1].Batches[" + _tempCurrentBatchNum +
                                                  "].Batcher_inert.Bunker2.Dozing_DozedValue";
                            _materialTags[i, 2] = "Progress_Components[7]";
                            _materialTags[i, 3] = "InertDozing.Tank_2_ON";
                        }
                        if (selContainer.Id == Static.IdСontainerDictionary[Static.ContainerItem.Bunker3])
                        {
                            _materialTags[i, 0] = "PAR_Inert_Bunker_3_set";
                            _materialTags[i, 1] = "Reports[1].Batches[" + _tempCurrentBatchNum +
                                                  "].Batcher_inert.Bunker3.Dozing_DozedValue";
                            _materialTags[i, 2] = "Progress_Components[8]";
                            _materialTags[i, 3] = "InertDozing.Tank_3_ON";
                        }
                        if (selContainer.Id == Static.IdСontainerDictionary[Static.ContainerItem.Bunker4])
                        {
                            _materialTags[i, 0] = "PAR_Inert_Bunker_4_set";
                            _materialTags[i, 1] = "Reports[1].Batches[" + _tempCurrentBatchNum +
                                                  "].Batcher_inert.Bunker4.Dozing_DozedValue";
                            _materialTags[i, 2] = "Progress_Components[9]";
                            _materialTags[i, 3] = "InertDozing.Tank_4_ON";
                        }
                        if (selContainer.Id == Static.IdСontainerDictionary[Static.ContainerItem.Silo1])
                        {
                            _materialTags[i, 0] = "PAR_Cement_Silos_1_set";
                            _materialTags[i, 1] = "Reports[1].Batches[" + _tempCurrentBatchNum +
                                                  "].Batcher_cement.Silos1.Dozing_DozedValue";
                            _materialTags[i, 2] = "Progress_Components[3]";
                            _materialTags[i, 3] = "CementDozing.Silo_1_ON";
                        }
                        if (selContainer.Id == Static.IdСontainerDictionary[Static.ContainerItem.Silo2])
                        {
                            _materialTags[i, 0] = "PAR_Cement_Silos_2_set";
                            _materialTags[i, 1] = "Reports[1].Batches[" + _tempCurrentBatchNum +
                                                  "].Batcher_cement.Silos2.Dozing_DozedValue";
                            _materialTags[i, 2] = "Progress_Components[4]";
                            _materialTags[i, 3] = "CementDozing.Silo_2_ON";
                        }
                        if (selContainer.Id == Static.IdСontainerDictionary[Static.ContainerItem.Water])
                        {
                            _materialTags[i, 0] = "Reports[1].Batches[" + _tempCurrentBatchNum +
                                                  "].Batcher_water.Water.Dozing_SetValue";
                            _materialTags[i, 1] = "Reports[1].Batches[" + _tempCurrentBatchNum +
                                                  "].Batcher_water.Water.Dozing_DozedValue";
                            _materialTags[i, 2] = "Progress_Components[5]";
                            _materialTags[i, 3] = "DozingWater_ON";
                        }
                        if (selContainer.Id == Static.IdСontainerDictionary[Static.ContainerItem.Additive1])
                        {
                            _materialTags[i, 0] = "PAR_Additive_Tank_1_set";
                            _materialTags[i, 1] = "Reports[1].Batches[" + _tempCurrentBatchNum +
                                                  "].Batcher_add.Tank1.Dozing_DozedValue";
                            _materialTags[i, 2] = "Progress_Components[1]";
                            _materialTags[i, 3] = "AdditiveDozing.Tank_1_ON";
                        }
                        if (selContainer.Id == Static.IdСontainerDictionary[Static.ContainerItem.Additive2])
                        {
                            _materialTags[i, 0] = "PAR_Additive_Tank_2_set";
                            _materialTags[i, 1] = "Reports[1].Batches[" + _tempCurrentBatchNum +
                                                  "].Batcher_add.Tank2.Dozing_DozedValue";
                            _materialTags[i, 2] = "Progress_Components[2]";
                            _materialTags[i, 3] = "AdditiveDozing.Tank_2_ON";
                        }
                    }
                }
            }
        }

        public void GetValue()
        {
            if (ViewModelTabl.CurrentSelTask != null && _tempCurrentBatchNum != 0 && _materials != null)
            {
                for (int i = 0; i < _materials.Count; i++)
                {
                    DosingTask[i].SetValue = Math.Round(_opc.cl.ReadReal(_materialTags[i, 0], out var errSet), 1).ToString("F1");
                    DosingTask[i].CurrentValue = Math.Round(_opc.cl.ReadReal(_materialTags[i, 1], out var errCurrent), 1).ToString("F1");
                    DosingTask[i].CurrentProgress = _opc.cl.ReadReal(_materialTags[i, 2], out var errProgress);
                    DosingTask[i].InProgress = DosingProcess != 100 ? _opc.cl.ReadBool(_materialTags[i, 3], out var errInProgress) : false;
                    if (errSet)
                        DosingTask[i].SetValue = "";
                    if (errCurrent)
                        DosingTask[i].CurrentValue = "";
                    if (errProgress)
                        DosingTask[i].CurrentProgress = 0;

                    DosingTask[i].ProgressDone = (DosingTask[i].CurrentProgress == ComponentsWeight) || DosingProcess == 100;
                }
            }
        }
    }

    public sealed class ViewDosingTask : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _currentProgress;
        public double CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
            }
        }

        private bool _progressDone;
        public bool ProgressDone
        {
            get { return _progressDone; }
            set
            {
                _progressDone = value;
                OnPropertyChanged(nameof(ProgressDone));
            }
        }

        private bool _inProgress;
        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                OnPropertyChanged(nameof(InProgress));
            }
        }

        private string _materialName;
        public string MaterialName
        {
            get { return _materialName; }
            set
            {
                _materialName = value;
                OnPropertyChanged(nameof(MaterialName));
            }
        }

        private string _setValue;
        public string SetValue
        {
            get { return _setValue; }
            set
            {
                _setValue = value;
                OnPropertyChanged(nameof(SetValue));
            }
        }

        private string _currentValue;
        public string CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                OnPropertyChanged(nameof(CurrentValue));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}