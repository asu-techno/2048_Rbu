using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using AS_Library.Classes;
using AS_Library.Link;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Classes
{
    public class DosingSettingsViewModel : INotifyPropertyChanged
    {
        public enum DosingSettingType
        {
            Cement = 1,
            Water,
            Additive,
            Inert
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private ContainersReader ContainersReader { get; set; } = new ContainersReader();
        private ObservableCollection<ApiContainer> _containers;
        private ApiContainer _selContainer;
        private string _tagWeight;
        private int _containerId;

        public DosingSettingsViewModel(OpcServer.OpcList opcName, DosingSettingType typeMaterial, int containerId)
        {
            _opcName = opcName;
            _containerId = containerId;

            _containers = new ObservableCollection<ApiContainer>(ContainersReader.ListContainers());
            _selContainer = _containers.FirstOrDefault(x => x.Id == containerId);

            switch (typeMaterial)
            {
                case DosingSettingType.Cement:
                    NameWindow = "Настройки дозирования цемента. ";
                    break;
                case DosingSettingType.Water:
                    NameWindow = "Настройки дозирования воды. ";
                    break;
                case DosingSettingType.Additive:
                    NameWindow = "Настройки дозирования химической добавки. ";
                    break;
                case DosingSettingType.Inert:
                    NameWindow = "Настройки дозирования инертного материала. ";
                    IsInertBunker = true;
                    break;
            }

            if (_selContainer.CurrentMaterial != null)
                NameWindow += _selContainer.Name + " (" + _selContainer.CurrentMaterial.Name + ")";
            else
                NameWindow += _selContainer.Name;

            if (containerId > 0 && containerId < 5)
            {
                WorkMode = new WorkMode[3];
                WorkMode[0] = new WorkMode(opcName);
                WorkMode[1] = new WorkMode(opcName);
                WorkMode[2] = new WorkMode(opcName);
            }

            switch (containerId)
            {
                case 1:
                    WorkMode[0].NameGate = "V-9-1";
                    WorkMode[1].NameGate = "V-9-2";
                    WorkMode[2].NameGate = "Обе";
                    WorkMode[0].TagGate = "V_9_1.";
                    WorkMode[1].TagGate = "V_9_2.";
                    WorkMode[2].TagGate = "V_9_Both.";
                    _tagWeight = "Inert_Bunker1";
                    break;
                case 2:
                    WorkMode[0].NameGate = "V-10-1";
                    WorkMode[1].NameGate = "V-10-2";
                    WorkMode[2].NameGate = "Обе";
                    WorkMode[0].TagGate = "V_10_1.";
                    WorkMode[1].TagGate = "V_10_2.";
                    WorkMode[2].TagGate = "V_10_Both.";
                    _tagWeight = "Inert_Bunker2";
                    break;
                case 3:
                    WorkMode[0].NameGate = "V-11-1";
                    WorkMode[1].NameGate = "V-11-2";
                    WorkMode[2].NameGate = "Обе";
                    WorkMode[0].TagGate = "V_11_1.";
                    WorkMode[1].TagGate = "V_11_2.";
                    WorkMode[2].TagGate = "V_11_Both.";
                    _tagWeight = "Inert_Bunker3";
                    break;
                case 4:
                    WorkMode[0].NameGate = "V-12-1";
                    WorkMode[1].NameGate = "V-12-2";
                    WorkMode[2].NameGate = "Обе";
                    WorkMode[0].TagGate = "V_12_1.";
                    WorkMode[1].TagGate = "V_12_2.";
                    WorkMode[2].TagGate = "V_12_Both.";
                    _tagWeight = "Inert_Bunker4";
                    break;
                case 5:
                    _tagWeight = "Cement_Silo1";
                    break;
                case 6:
                    _tagWeight = "Cement_Silo2";
                    break;
                case 7:
                    _tagWeight = "Water";
                    break;
                case 8:
                    _tagWeight = "Additive_Tank1";
                    break;
                case 9:
                    _tagWeight = "Additive_Tank2";
                    break;
            }

            if (containerId > 0 && containerId < 5)
            {
                WorkMode[0].HeaderGate = "Работа задвижкой " + WorkMode[0].NameGate;
                WorkMode[1].HeaderGate = "Работа задвижкой " + WorkMode[1].NameGate;
                WorkMode[2].HeaderGate = "Работа задвижками " + WorkMode[0].NameGate + " и " + WorkMode[1].NameGate;
            }

            Subscribe();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            var weightItem = new OpcMonitoredItem(_opc.cl.GetNode("WeightDiff_" + _tagWeight), OpcAttribute.Value);
            weightItem.DataChangeReceived += HandleWeightChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(weightItem);

            if (_containerId > 0 && _containerId < 5)
                for (int i = 0; i < 3; i++)
                    WorkMode[i].Subscribe();

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
        }

        private void HandleWeightChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            WeightDiff = double.Parse(e.Item.Value.ToString());
        }

        private WorkMode[] _workMode;
        public WorkMode[] WorkMode
        {
            get { return _workMode; }
            set
            {
                _workMode = value;
                OnPropertyChanged(nameof(WorkMode));
            }
        }

        private string _nameWindow;
        public string NameWindow
        {
            get { return _nameWindow; }
            set
            {
                _nameWindow = value;
                OnPropertyChanged(nameof(NameWindow));
            }
        }

        private bool _isInertBunker;
        public bool IsInertBunker
        {
            get { return _isInertBunker; }
            set
            {
                _isInertBunker = value;
                OnPropertyChanged(nameof(IsInertBunker));
            }
        }

        private double _weightDiff;
        public double WeightDiff
        {
            get { return _weightDiff; }
            set
            {
                _weightDiff = value;
                OnPropertyChanged(nameof(WeightDiff));
            }
        }

        private RelayCommand _setWeightDiff;
        public RelayCommand SetWeightDiff
        {
            get
            {
                return _setWeightDiff ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(null, null, _opcName, NameWindow + ". Недосып", 0, 100, "WeightDiff_" + _tagWeight, "Real", null, 0, 1);
                });
            }
        }
    }

    public sealed class WorkMode : INotifyPropertyChanged
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        public event PropertyChangedEventHandler PropertyChanged;

        public WorkMode(OpcServer.OpcList opcName)
        {
            _opcName = opcName;
        }

        public string TagGate;

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

            var preciseModeItem = new OpcMonitoredItem(_opc.cl.GetNode(TagGate + "Precise_Active"), OpcAttribute.Value);
            preciseModeItem.DataChangeReceived += HandlePreciseModeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(preciseModeItem);

            var roughtModeItem = new OpcMonitoredItem(_opc.cl.GetNode(TagGate + "Rought_Active"), OpcAttribute.Value);
            roughtModeItem.DataChangeReceived += HandleRoughtModeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(roughtModeItem);

            var preciseWorkItem = new OpcMonitoredItem(_opc.cl.GetNode(TagGate + "Precise_Work"), OpcAttribute.Value);
            preciseWorkItem.DataChangeReceived += HandlePreciseWorkChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(preciseWorkItem);

            var precisePauseItem = new OpcMonitoredItem(_opc.cl.GetNode(TagGate + "Precise_Pause"), OpcAttribute.Value);
            precisePauseItem.DataChangeReceived += HandlePrecisePauseChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(precisePauseItem);

            var roughtWorkItem = new OpcMonitoredItem(_opc.cl.GetNode(TagGate + "Rought_Work"), OpcAttribute.Value);
            roughtWorkItem.DataChangeReceived += HandleRoughtWorkChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(roughtWorkItem);

            var roughtPauseItem = new OpcMonitoredItem(_opc.cl.GetNode(TagGate + "Rought_Pause"), OpcAttribute.Value);
            roughtPauseItem.DataChangeReceived += HandleRoughtPauseChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(roughtPauseItem);
        }

        private void HandlePreciseModeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            PreciseWork = bool.Parse(e.Item.Value.ToString());
        }

        private void HandleRoughtModeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            RoughtWork = bool.Parse(e.Item.Value.ToString());
        }

        private void HandlePreciseWorkChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            PreciseWorkParam = double.Parse(e.Item.Value.ToString());
        }

        private void HandlePrecisePauseChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            PrecisePauseParam = double.Parse(e.Item.Value.ToString());
        }

        private void HandleRoughtWorkChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            RoughtWorkParam = double.Parse(e.Item.Value.ToString());
        }

        private void HandleRoughtPauseChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            RoughtPauseParam = double.Parse(e.Item.Value.ToString());
        }

        private bool _preciseWork;
        public bool PreciseWork
        {
            get { return _preciseWork; }
            set
            {
                _preciseWork = value;
                OnPropertyChanged(nameof(PreciseWork));
            }
        }

        private bool _roughtWork;
        public bool RoughtWork
        {
            get { return _roughtWork; }
            set
            {
                _roughtWork = value;
                OnPropertyChanged(nameof(RoughtWork));
            }
        }

        private double _preciseWorkParam;
        public double PreciseWorkParam
        {
            get { return _preciseWorkParam; }
            set
            {
                _preciseWorkParam = value;
                OnPropertyChanged(nameof(PreciseWorkParam));
            }
        }

        private double _precisePauseParam;
        public double PrecisePauseParam
        {
            get { return _precisePauseParam; }
            set
            {
                _precisePauseParam = value;
                OnPropertyChanged(nameof(PrecisePauseParam));
            }
        }

        private double _roughtWorkParam;
        public double RoughtWorkParam
        {
            get { return _roughtWorkParam; }
            set
            {
                _roughtWorkParam = value;
                OnPropertyChanged(nameof(RoughtWorkParam));
            }
        }

        private double _roughtPauseParam;
        public double RoughtPauseParam
        {
            get { return _roughtPauseParam; }
            set
            {
                _roughtPauseParam = value;
                OnPropertyChanged(nameof(RoughtPauseParam));
            }
        }

        private string _nameGate;
        public string NameGate
        {
            get { return _nameGate; }
            set
            {
                _nameGate = value;
                OnPropertyChanged(nameof(NameGate));
            }
        }

        private string _headerGate;
        public string HeaderGate
        {
            get { return _headerGate; }
            set
            {
                _headerGate = value;
                OnPropertyChanged(nameof(HeaderGate));
            }
        }

        void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private RelayCommand _setPreciseWorkParam;
        public RelayCommand SetPreciseWorkParam
        {
            get
            {
                return _setPreciseWorkParam ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(null, null, _opcName, HeaderGate + ". Время работы в режиме работы \"Точно, с\"", 0, 100, TagGate + "Precise_Work", "Real", null, 0, 0);
                });
            }
        }

        private RelayCommand _setPrecisePauseParam;
        public RelayCommand SetPrecisePauseParam
        {
            get
            {
                return _setPrecisePauseParam ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(null, null, _opcName, HeaderGate + ". Время паузы в режиме работы \"Точно, с\"", 0, 100, TagGate + "Precise_Pause", "Real", null, 0, 0);
                });
            }
        }

        private RelayCommand _setRoughtWorkParam;
        public RelayCommand SetRoughtWorkParam
        {
            get
            {
                return _setRoughtWorkParam ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(null, null, _opcName, HeaderGate + ". Время работы в режиме работы \"Грубо, с\"", 0, 100, TagGate + "Rought_Work", "Real", null, 0, 0);
                });
            }
        }

        private RelayCommand _setRoughtPauseParam;
        public RelayCommand SetRoughtPauseParam
        {
            get
            {
                return _setRoughtPauseParam ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(null, null, _opcName, HeaderGate + ". Время паузы в режиме работы \"Грубо, с\"", 0, 100, TagGate + "Rought_Pause", "Real", null, 0, 0);
                });
            }
        }
    }
}
