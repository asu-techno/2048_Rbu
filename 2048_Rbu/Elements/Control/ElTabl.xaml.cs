using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using AS_Library.Link;
using System.ComponentModel;
using _2048_Rbu.Classes;
using System.Linq;
using System.Runtime.CompilerServices;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using AsuBetonLibrary.Services;
using Opc.UaFx;
using Opc.UaFx.Client;

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

        public void GetValue()
        {
            _viewmodelTabl.GetValue();
        }
    }

    public sealed class ViewModelTabl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TasksReader TasksReader { get; set; } = new TasksReader();
        private RecipesReader RecipesReader { get; set; } = new RecipesReader();
        private ContainersReader ContainersReader { get; set; } = new ContainersReader();
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private long _id, _currentId;

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

        private List<string> _materialName;
        public List<string> MaterialName
        {
            get { return _materialName; }
            set
            {
                _materialName = value;
                OnPropertyChanged(nameof(MaterialName));
            }
        }

        private string[] _setValueTag;
        private string[] _currentValueTag;

        private string[] _setValue;
        public string[] SetValue
        {
            get { return _setValue; }
            set
            {
                _setValue = value;
                OnPropertyChanged(nameof(SetValue));
            }
        }

        private string[] _currentValue;
        public string[] CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                OnPropertyChanged(nameof(CurrentValue));
            }
        }

        public void GetTable()
        {
            if (_id != 0)
            {
                if (_id != _currentId)
                {
                    try
                    {
                        GetTask(_id);
                        GetValue();
                        _currentId = _id;
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.WriteAllText(@"Log\log.txt", DateTime.Now + " - " + ex.Message + "->" + _id);
                    }
                }
            }
            else
            {
                SelTask = null;
                MaterialName = null;
                SetValue = null;
                CurrentValue = null;
                _currentId = _id;
            }
        }

        private void GetTask(long id)
        {
            Tasks = new ObservableCollection<ApiTask>(TasksReader.ListTasks());
            SelTask = Tasks.FirstOrDefault(x => x.Id == id);

            GetMaterial();
        }

        private void GetMaterial()
        {
            var recipes = new ObservableCollection<ApiRecipe>(RecipesReader.ListRecipes());
            var selRecipe = recipes.FirstOrDefault(x => x.Id == SelTask.Recipe.Id);
            var materials = new ObservableCollection<ApiRecipeMaterial>(selRecipe.RecipeMaterials);
            MaterialName = new List<string>();
            foreach (var mat in materials)
            {
                if (mat.Material.Name != null)
                    MaterialName.Add(mat.Material.Name);
                else
                    MaterialName.Add("");
            }

            _setValueTag = new string[materials.Count];
            _currentValueTag = new string[materials.Count];
            var containers = new ObservableCollection<ApiContainer>(ContainersReader.ListContainers());
            for (int i = 0; i < materials.Count; i++)
            {
                var selContainer = containers.FirstOrDefault(x => x.CurrentMaterial.Id == materials[i].Material.Id);
                switch (selContainer.Id)
                {
                    case 0:
                        _setValueTag[i] = null;
                        _currentValueTag[i] = null;
                        break;
                    case 1:
                        _setValueTag[i] = "Reports[1].Batchers[1].Batcher_inert.Bunker1.Dozing_SetValue";
                        _currentValueTag[i] = "Reports[1].Batchers[1].Batcher_inert.Bunker1.Dozing_DozedValue";
                        break;
                    case 2:
                        _setValueTag[i] = "Reports[1].Batchers[1].Batcher_inert.Bunker2.Dozing_SetValue";
                        _currentValueTag[i] = "Reports[1].Batchers[1].Batcher_inert.Bunker2.Dozing_DozedValue";
                        break;
                    case 3:
                        _setValueTag[i] = "Reports[1].Batchers[1].Batcher_inert.Bunker3.Dozing_SetValue";
                        _currentValueTag[i] = "Reports[1].Batchers[1].Batcher_inert.Bunker3.Dozing_DozedValue";
                        break;
                    case 4:
                        _setValueTag[i] = "Reports[1].Batchers[1].Batcher_inert.Bunker4.Dozing_SetValue";
                        _currentValueTag[i] = "Reports[1].Batchers[1].Batcher_inert.Bunker4.Dozing_DozedValue";
                        break;
                    case 5:
                        _setValueTag[i] = "Reports[1].Batchers[1].Batcher_cement.Silos1.Dozing_SetValue";
                        _currentValueTag[i] = "Reports[1].Batchers[1].Batcher_cement.Silos1.Dozing_DozedValue";
                        break;
                    case 6:
                        _setValueTag[i] = "Reports[1].Batchers[1].Batcher_cement.Silos2.Dozing_SetValue";
                        _currentValueTag[i] = "Reports[1].Batchers[1].Batcher_cement.Silos2.Dozing_DozedValue";
                        break;
                    case 7:
                        _setValueTag[i] = "Reports[1].Batchers[1].Batcher_water.Water.Dozing_SetValue";
                        _currentValueTag[i] = "Reports[1].Batchers[1].Batcher_water.Water.Dozing_DozedValue";
                        break;
                    case 8:
                        _setValueTag[i] = "Reports[1].Batchers[1].Batcher_add.Tank1.Dozing_StartTime";
                        _currentValueTag[i] = "Reports[1].Batchers[1].Batcher_add.Tank1.Dozing_DozedValue";
                        break;
                    case 9:
                        _setValueTag[i] = "Reports[1].Batchers[1].Batcher_add.Tank2.Dozing_SetValue";
                        _currentValueTag[i] = "Reports[1].Batchers[1].Batcher_add.Tank2.Dozing_DozedValue";
                        break;
                    default:
                        _setValueTag[i] = null;
                        _currentValueTag[i] = null;
                        break;
                }
            }
        }

        public void GetValue()
        {
            if (_id != 0)
            {
                if (_currentId != _id)
                {
                    SetValue = new string[_setValueTag.Length];
                    CurrentValue = new string[_setValueTag.Length];
                }
                for (int i = 0; i < _setValueTag.Length; i++)
                {
                    if (_setValueTag[i] == null || _currentValueTag[i] == null)
                    {
                        SetValue[i] = "";
                        CurrentValue[i] = "";
                    }
                    else
                    {
                        SetValue[i] = _opc.cl.ReadReal(_setValueTag[i], out var err).ToString();
                        CurrentValue[i] = _opc.cl.ReadReal(_currentValueTag[i], out var err1).ToString();
                        if (err)
                            SetValue[i] = "ошибка";
                        if (err1)
                            CurrentValue[i] = "ошибка";
                    }
                }
            }
        }
    }
}