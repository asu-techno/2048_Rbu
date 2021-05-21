using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using _2048_Rbu.Windows;
using AS_Library.Classes;
using AS_Library.Link;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Classes.ViewModel
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
        private string _tagWeight, _tagPreciseDos;
        private Static.ContainerItem _containerItem;
        private DosingSettingType _typeMaterial;

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

        private bool _isWaterOrAdditive;
        public bool IsWaterOrAdditive
        {
            get { return _isWaterOrAdditive; }
            set
            {
                _isWaterOrAdditive = value;
                OnPropertyChanged(nameof(IsWaterOrAdditive));
            }
        }

        private bool _isCement;
        public bool IsCement
        {
            get { return _isCement; }
            set
            {
                _isCement = value;
                OnPropertyChanged(nameof(IsCement));
            }
        }

        private string _weightDiff;
        public string WeightDiff
        {
            get { return _weightDiff; }
            set
            {
                _weightDiff = value;
                OnPropertyChanged(nameof(WeightDiff));
            }
        }

        private string _percentRought;
        public string PercentRought
        {
            get { return _percentRought; }
            set
            {
                _percentRought = value;
                OnPropertyChanged(nameof(PercentRought));
            }
        }

        private string _weightPrecise;
        public string WeightPrecise
        {
            get { return _weightPrecise; }
            set
            {
                _weightPrecise = value;
                OnPropertyChanged(nameof(WeightPrecise));
            }
        }

        private bool _byPercent;
        public bool ByPercent
        {
            get { return _byPercent; }
            set
            {
                _byPercent = value;
                OnPropertyChanged(nameof(ByPercent));
            }
        }

        public DosingSettingsViewModel(OpcServer.OpcList opcName, DosingSettingType typeMaterial, Static.ContainerItem containerItem)
        {
            _opcName = opcName;
            _containerItem = containerItem;
            _typeMaterial = typeMaterial;

            switch (typeMaterial)
            {
                case DosingSettingType.Cement:
                    NameWindow = "Настройки дозирования цемента.\n";
                    IsCement = true;
                    WorkMode = new WorkMode[1];
                    WorkMode[0] = new WorkMode(opcName);
                    WorkMode[0].NameGate = "Выгрузка цемента";
                    WorkMode[0].TagGate = "V_2";
                    switch (_containerItem)
                    {
                        case Static.ContainerItem.Silo1:
                            _tagWeight = "Cement_Silo1";
                            _tagPreciseDos = "Cement_1";
                            break;
                        case Static.ContainerItem.Silo2:
                            _tagWeight = "Cement_Silo2";
                            _tagPreciseDos = "Cement_2";
                            break;
                    }
                    WorkMode[0].HeaderGate = WorkMode[0].NameGate;
                    break;
                case DosingSettingType.Water:
                    NameWindow = "Настройки дозирования воды.\n";
                    IsWaterOrAdditive = true;
                    WorkMode = new WorkMode[2];
                    WorkMode[0] = new WorkMode(opcName);
                    WorkMode[1] = new WorkMode(opcName);
                    WorkMode[0].NameGate = "Дозирование воды";
                    WorkMode[0].TagGate = "V_4";
                    WorkMode[1].NameGate = "Выгрузка воды";
                    WorkMode[1].TagGate = "V_3";
                    _tagWeight= _tagPreciseDos = "Water";
                    WorkMode[0].HeaderGate = WorkMode[0].NameGate;
                    WorkMode[1].HeaderGate = WorkMode[1].NameGate;
                    break;
                case DosingSettingType.Additive:
                    NameWindow = "Настройки дозирования химической добавки.\n";
                    IsWaterOrAdditive = true;
                    WorkMode = new WorkMode[2];
                    WorkMode[0] = new WorkMode(opcName);
                    WorkMode[1] = new WorkMode(opcName);
                    switch (_containerItem)
                    {
                        case Static.ContainerItem.Additive1:
                            WorkMode[0].NameGate = "Источник хим. добавки №1";
                            WorkMode[0].TagGate = "V_6";
                            _tagWeight = "Additive_Tank1";
                            _tagPreciseDos = "Additive_1";
                            break;
                        case Static.ContainerItem.Additive2:
                            WorkMode[0].NameGate = "Источник хим. добавки №2";
                            WorkMode[0].TagGate = "V_7";
                            _tagWeight = "Additive_Tank2";
                            _tagPreciseDos = "Additive_2";
                            break;
                    }
                    WorkMode[1].NameGate = "Выгрузка хим. добавки";
                    WorkMode[1].TagGate = "V_5";
                    WorkMode[0].HeaderGate = WorkMode[0].NameGate;
                    WorkMode[1].HeaderGate = WorkMode[1].NameGate;
                    break;
                case DosingSettingType.Inert:
                    NameWindow = "Настройки дозирования инертного материала.\n";
                    IsInertBunker = true;
                    WorkMode = new WorkMode[3];
                    WorkMode[0] = new WorkMode(opcName, true);
                    WorkMode[1] = new WorkMode(opcName, true);
                    WorkMode[2] = new WorkMode(opcName, true);
                    switch (_containerItem)
                    {
                        case Static.ContainerItem.Bunker1:
                            WorkMode[0].NameGate = "V-9-1";
                            WorkMode[1].NameGate = "V-9-2";
                            WorkMode[2].NameGate = "Обе";
                            WorkMode[0].TagGate = "V_9_1";
                            WorkMode[1].TagGate = "V_9_2";
                            WorkMode[2].TagGate = "V_9_Both";
                            _tagWeight = "Inert_Bunker1";
                            _tagPreciseDos = "Inert_1";
                            break;
                        case Static.ContainerItem.Bunker2:
                            WorkMode[0].NameGate = "V-10-1";
                            WorkMode[1].NameGate = "V-10-2";
                            WorkMode[2].NameGate = "Обе";
                            WorkMode[0].TagGate = "V_10_1";
                            WorkMode[1].TagGate = "V_10_2";
                            WorkMode[2].TagGate = "V_10_Both";
                            _tagWeight = "Inert_Bunker2";
                            _tagPreciseDos = "Inert_2";
                            break;
                        case Static.ContainerItem.Bunker3:
                            WorkMode[0].NameGate = "V-11-1";
                            WorkMode[1].NameGate = "V-11-2";
                            WorkMode[2].NameGate = "Обе";
                            WorkMode[0].TagGate = "V_11_1";
                            WorkMode[1].TagGate = "V_11_2";
                            WorkMode[2].TagGate = "V_11_Both";
                            _tagWeight = "Inert_Bunker3";
                            _tagPreciseDos = "Inert_3";
                            break;
                        case Static.ContainerItem.Bunker4:
                            WorkMode[0].NameGate = "V-12-1";
                            WorkMode[1].NameGate = "V-12-2";
                            WorkMode[2].NameGate = "Обе";
                            WorkMode[0].TagGate = "V_12_1";
                            WorkMode[1].TagGate = "V_12_2";
                            WorkMode[2].TagGate = "V_12_Both";
                            _tagWeight = "Inert_Bunker4";
                            _tagPreciseDos = "Inert_4";
                            break;
                    }
                    WorkMode[0].HeaderGate = "Работа задвижкой " + WorkMode[0].NameGate;
                    WorkMode[1].HeaderGate = "Работа задвижкой " + WorkMode[1].NameGate;
                    WorkMode[2].HeaderGate = "Работа задвижками " + WorkMode[0].NameGate + " и " + WorkMode[1].NameGate;
                    break;
            }

            NameWindow += Static.СontainerNameDictionary[_containerItem] + " (" + Static.СontainerMaterialDictionary[_containerItem] + ")";

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

            var percentRought = new OpcMonitoredItem(_opc.cl.GetNode(_tagPreciseDos + ".Percents"), OpcAttribute.Value);
            percentRought.DataChangeReceived += HandlePercentRoughtChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(percentRought);
            var weightPrecise = new OpcMonitoredItem(_opc.cl.GetNode(_tagPreciseDos + ".PreciseWeight"), OpcAttribute.Value);
            weightPrecise.DataChangeReceived += HandleWeightPreciseChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(weightPrecise);
            var byPercent = new OpcMonitoredItem(_opc.cl.GetNode(_tagPreciseDos + ".Work_ByPercent"), OpcAttribute.Value);
            byPercent.DataChangeReceived += HandleByPercentChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(byPercent);

            for (int i = 0; i < WorkMode.Length; i++)
            {
                WorkMode[i].Subscribe();
            }

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
        }

        private void HandleWeightChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            WeightDiff = double.Parse(e.Item.Value.ToString()).ToString("F1");
        }

        private void HandlePercentRoughtChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            PercentRought = double.Parse(e.Item.Value.ToString()).ToString("F1");
        }

        private void HandleWeightPreciseChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            WeightPrecise = double.Parse(e.Item.Value.ToString()).ToString("F1");
        }

        private void HandleByPercentChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            ByPercent = bool.Parse(e.Item.Value.ToString());
        }

        private RelayCommand _setWeightDiff;
        public RelayCommand SetWeightDiff
        {
            get
            {
                return _setWeightDiff ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameWindow + ". Недосып, кг", 0.0, 100.0, "WeightDiff_" + _tagWeight, WindowSetParameter.ValueType.Real, null, 1, 10.0, 20.0, 50.0, 100.0, 10.0);
                });
            }
        }

        private RelayCommand _setPercentRought;
        public RelayCommand SetPercentRought
        {
            get
            {
                return _setPercentRought ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameWindow + ". Процент грубого дозирования, %", 0.0, 100.0, _tagPreciseDos + ".Percents", WindowSetParameter.ValueType.Real, null, 1, 10.0, 20.0, 50.0, 100.0, 10.0);
                });
            }
        }

        private RelayCommand _setWeightPrecise;
        public RelayCommand SetWeightPrecise
        {
            get
            {
                return _setWeightPrecise ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameWindow + ". Масса точного дозирования, %", 0.0, 1000.0, _tagPreciseDos + ".PreciseWeight", WindowSetParameter.ValueType.Real, null, 1, 10.0, 50.0, 100.0, 500.0, 50.0);
                });
            }
        }

        private RelayCommand _setByPercent;
        public RelayCommand SetByPercent
        {
            get
            {
                return _setByPercent ??= new RelayCommand((o) =>
                {
                    Methods.ButtonClick(_tagPreciseDos + ".Work_ByPercent", true, "Признак точного дозирования по процентам");
                });
            }
        }

        private RelayCommand _setByWeight;
        public RelayCommand SetByWeight
        {
            get
            {
                return _setByWeight ??= new RelayCommand((o) =>
                {
                    Methods.ButtonClick(_tagPreciseDos + ".Work_ByPercent", false, "Признак точного дозирования по массе");
                });
            }
        }

        private RelayCommand _setLeftRought;
        public RelayCommand SetLeftRought
        {
            get
            {
                return _setLeftRought ??= new RelayCommand((o) =>
                {
                    Methods.ButtonClick("Rought." + WorkMode[0].TagGate, true, NameWindow + ". Режим грубо:" + WorkMode[0].HeaderGate);
                    Methods.ButtonClick("Rought." + WorkMode[1].TagGate, false);
                    Methods.ButtonClick("Rought." + WorkMode[2].TagGate, false);
                });
            }
        }

        private RelayCommand _setRightRought;
        public RelayCommand SetRightRought
        {
            get
            {
                return _setRightRought ??= new RelayCommand((o) =>
                {
                    Methods.ButtonClick("Rought." + WorkMode[1].TagGate, true, NameWindow + ". Режим грубо:" + WorkMode[1].HeaderGate);
                    Methods.ButtonClick("Rought." + WorkMode[0].TagGate, false);
                    Methods.ButtonClick("Rought." + WorkMode[2].TagGate, false);
                });
            }
        }

        private RelayCommand _setBothRought;
        public RelayCommand SetBothRought
        {
            get
            {
                return _setBothRought ??= new RelayCommand((o) =>
                {
                    Methods.ButtonClick("Rought." + WorkMode[2].TagGate, true, NameWindow + ". Режим грубо:" + WorkMode[2].HeaderGate);
                    Methods.ButtonClick("Rought." + WorkMode[0].TagGate, false);
                    Methods.ButtonClick("Rought." + WorkMode[1].TagGate, false);
                });
            }
        }
        private RelayCommand _setLeftPrecise;
        public RelayCommand SetLeftPrecise
        {
            get
            {
                return _setLeftPrecise ??= new RelayCommand((o) =>
                {
                    Methods.ButtonClick("Precise." + WorkMode[0].TagGate, true, NameWindow + ". Режим точно:" + WorkMode[0].HeaderGate);
                    Methods.ButtonClick("Precise." + WorkMode[1].TagGate, false);
                    Methods.ButtonClick("Precise." + WorkMode[2].TagGate, false);
                });
            }
        }
        private RelayCommand _setRightPrecise;
        public RelayCommand SetRightPrecise
        {
            get
            {
                return _setRightPrecise ??= new RelayCommand((o) =>
                {
                    Methods.ButtonClick("Precise." + WorkMode[1].TagGate, true, NameWindow + ". Режим точно:" + WorkMode[1].HeaderGate);
                    Methods.ButtonClick("Precise." + WorkMode[0].TagGate, false);
                    Methods.ButtonClick("Precise." + WorkMode[2].TagGate, false);
                });
            }
        }
        private RelayCommand _setBothPrecise;
        public RelayCommand SetBothPrecise
        {
            get
            {
                return _setBothPrecise ??= new RelayCommand((o) =>
                {
                    Methods.ButtonClick("Precise." + WorkMode[2].TagGate, true, NameWindow + ". Режим точно:" + WorkMode[2].HeaderGate);
                    Methods.ButtonClick("Precise." + WorkMode[0].TagGate, false);
                    Methods.ButtonClick("Precise." + WorkMode[1].TagGate, false);
                });
            }
        }
    }

    public sealed class WorkMode : INotifyPropertyChanged
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private readonly bool _isInertBunker;
        public event PropertyChangedEventHandler PropertyChanged;

        public WorkMode(OpcServer.OpcList opcName, bool isInertBunker = false)
        {
            _opcName = opcName;
            _isInertBunker = isInertBunker;
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
            if (_isInertBunker)
            {
                var preciseModeItem = new OpcMonitoredItem(_opc.cl.GetNode("Precise." + TagGate), OpcAttribute.Value);
                preciseModeItem.DataChangeReceived += HandlePreciseModeChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(preciseModeItem);

                var roughtModeItem = new OpcMonitoredItem(_opc.cl.GetNode("Rought." + TagGate), OpcAttribute.Value);
                roughtModeItem.DataChangeReceived += HandleRoughtModeChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(roughtModeItem);
            }
            var preciseWorkItem = new OpcMonitoredItem(_opc.cl.GetNode(TagGate + ".Precise_Work"), OpcAttribute.Value);
            preciseWorkItem.DataChangeReceived += HandlePreciseWorkChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(preciseWorkItem);

            var precisePauseItem = new OpcMonitoredItem(_opc.cl.GetNode(TagGate + ".Precise_Pause"), OpcAttribute.Value);
            precisePauseItem.DataChangeReceived += HandlePrecisePauseChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(precisePauseItem);

            var roughtWorkItem = new OpcMonitoredItem(_opc.cl.GetNode(TagGate + ".Rought_Work"), OpcAttribute.Value);
            roughtWorkItem.DataChangeReceived += HandleRoughtWorkChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(roughtWorkItem);

            var roughtPauseItem = new OpcMonitoredItem(_opc.cl.GetNode(TagGate + ".Rought_Pause"), OpcAttribute.Value);
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
            PreciseWorkParam = double.Parse(e.Item.Value.ToString()).ToString("F1");
        }

        private void HandlePrecisePauseChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            PrecisePauseParam = double.Parse(e.Item.Value.ToString()).ToString("F1");
        }

        private void HandleRoughtWorkChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            RoughtWorkParam = double.Parse(e.Item.Value.ToString()).ToString("F1");
        }

        private void HandleRoughtPauseChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            RoughtPauseParam = double.Parse(e.Item.Value.ToString()).ToString("F1");
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

        private string _preciseWorkParam;
        public string PreciseWorkParam
        {
            get { return _preciseWorkParam; }
            set
            {
                _preciseWorkParam = value;
                OnPropertyChanged(nameof(PreciseWorkParam));
            }
        }

        private string _precisePauseParam;
        public string PrecisePauseParam
        {
            get { return _precisePauseParam; }
            set
            {
                _precisePauseParam = value;
                OnPropertyChanged(nameof(PrecisePauseParam));
            }
        }

        private string _roughtWorkParam;
        public string RoughtWorkParam
        {
            get { return _roughtWorkParam; }
            set
            {
                _roughtWorkParam = value;
                OnPropertyChanged(nameof(RoughtWorkParam));
            }
        }

        private string _roughtPauseParam;
        public string RoughtPauseParam
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
                    Methods.SetParameter(_opcName, HeaderGate + ". Время работы в режиме работы \"Точно\", с", 0.0, 10.0, TagGate + ".Precise_Work", WindowSetParameter.ValueType.Real, null, 1, 0.5, 1.0, 2.0, 3.0, 0.1);
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
                    Methods.SetParameter(_opcName, HeaderGate + ". Время паузы в режиме работы \"Точно\", с", 0.0, 30.0, TagGate + ".Precise_Pause", WindowSetParameter.ValueType.Real, null, 1, 0.5, 1.0, 2.0, 3.0, 0.1);
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
                    Methods.SetParameter(_opcName, HeaderGate + ". Время работы в режиме работы \"Грубо\", с", 0.0, 10.0, TagGate + ".Rought_Work", WindowSetParameter.ValueType.Real, null, 1, 0.5, 1.0, 2.0, 3.0, 0.1);
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
                    Methods.SetParameter(_opcName, HeaderGate + ". Время паузы в режиме работы \"Грубо\", с", 0.0, 30.0, TagGate + ".Rought_Pause", WindowSetParameter.ValueType.Real, null, 1, 0.5, 1.0, 2.0, 3.0, 0.1);
                });
            }
        }
    }
}
