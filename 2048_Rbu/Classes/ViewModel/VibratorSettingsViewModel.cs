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
    public class VibratorSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum VibratorSettingsItem
        {
            M13,
            M14,
            M10,
            M2,
            M3,
            M4,
            M5,
            M6,
            M7,
            M121,
            M122,
            Aeration1,
            Aeration2
        }

        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        private string _tagVibro;

        public VibratorSettingsViewModel(OpcServer.OpcList opcName, VibratorSettingsItem vibratorSettingsItem)
        {
            _opcName = opcName;

            switch (vibratorSettingsItem)
            {
                case VibratorSettingsItem.M13:
                    _tagVibro = "M_13";
                    NameVibro = "Вибратор M-13 (Силос 1)";
                    break;
                case VibratorSettingsItem.M14:
                    _tagVibro = "M_14";
                    NameVibro = "Вибратор M-14 (Силос 2)";
                    break;
                case VibratorSettingsItem.M10:
                    _tagVibro = "M_10";
                    NameVibro = "Вибратор M-10 (Дозатор цемента)";
                    break;
                case VibratorSettingsItem.M2:
                    _tagVibro = "M_2";
                    NameVibro = "Вибратор M-2 (Бункер 1)";
                    break;
                case VibratorSettingsItem.M3:
                    _tagVibro = "M_3";
                    NameVibro = "Вибратор M-3 (Бункер 1)";
                    break;
                case VibratorSettingsItem.M4:
                    _tagVibro = "M_4";
                    NameVibro = "Вибратор M-4 (Бункер 2)";
                    break;
                case VibratorSettingsItem.M5:
                    _tagVibro = "M_5";
                    NameVibro = "Вибратор M-5 (Бункер 2)";
                    break;
                case VibratorSettingsItem.M6:
                    _tagVibro = "M_6";
                    NameVibro = "Вибратор M-6 (Бункер 3)";
                    break;
                case VibratorSettingsItem.M7:
                    _tagVibro = "M_7";
                    NameVibro = "Вибратор M-7 (Бункер 3)";
                    break;
                case VibratorSettingsItem.M121:
                    _tagVibro = "M_12_1";
                    NameVibro = "Вибратор M-12-1 (Бункер 4)";
                    break;
                case VibratorSettingsItem.M122:
                    _tagVibro = "M_12_2";
                    NameVibro = "Вибратор M-12-2 (Бункер 4)";
                    break;
                case VibratorSettingsItem.Aeration1:
                    _tagVibro = "Air_Cement1";
                    NameVibro = "Аэратор силоса цемента 1";
                    break;
                case VibratorSettingsItem.Aeration2:
                    _tagVibro = "Air_Cement2";
                    NameVibro = "Аэратор силоса цемента 2";
                    break;
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
            var activeVibroItem = new OpcMonitoredItem(_opc.cl.GetNode(_tagVibro + ".Active"), OpcAttribute.Value);
            activeVibroItem.DataChangeReceived += HandleActiveVibroChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(activeVibroItem);

            var onQuantityItem = new OpcMonitoredItem(_opc.cl.GetNode(_tagVibro + ".Work_Count"), OpcAttribute.Value);
            onQuantityItem.DataChangeReceived += HandleOnQuantityChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(onQuantityItem);

            var onTimeItem = new OpcMonitoredItem(_opc.cl.GetNode(_tagVibro + ".Work_Time"), OpcAttribute.Value);
            onTimeItem.DataChangeReceived += HandleOnTimeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(onTimeItem);

            var pauseTimeItem = new OpcMonitoredItem(_opc.cl.GetNode(_tagVibro + ".Pause_Time"), OpcAttribute.Value);
            pauseTimeItem.DataChangeReceived += HandlePauseTimeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(pauseTimeItem);

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
        }

        private void HandleActiveVibroChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            ActiveVibro = bool.Parse(e.Item.Value.ToString());
        }

        private void HandleOnQuantityChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            OnQuantity = int.Parse(e.Item.Value.ToString());
        }

        private void HandleOnTimeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            OnTime = double.Parse(e.Item.Value.ToString()).ToString("F1");
        }

        private void HandlePauseTimeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            PauseTime = double.Parse(e.Item.Value.ToString()).ToString("F1");
        }

        private string _nameVibro;
        public string NameVibro
        {
            get { return _nameVibro; }
            set
            {
                _nameVibro = value;
                OnPropertyChanged(nameof(NameVibro));
            }
        }

        private bool _activeVibro;
        public bool ActiveVibro
        {
            get { return _activeVibro; }
            set
            {
                _activeVibro = value;
                OnPropertyChanged(nameof(ActiveVibro));
            }
        }

        private int _onQuantity;
        public int OnQuantity
        {
            get { return _onQuantity; }
            set
            {
                _onQuantity = value;
                OnPropertyChanged(nameof(OnQuantity));
            }
        }

        private string _onTime;
        public string OnTime
        {
            get { return _onTime; }
            set
            {
                _onTime = value;
                OnPropertyChanged(nameof(OnTime));
            }
        }

        private string _pauseTime;
        public string PauseTime
        {
            get { return _pauseTime; }
            set
            {
                _pauseTime = value;
                OnPropertyChanged(nameof(PauseTime));
            }
        }

        private RelayCommand _setActiveVibro;
        public RelayCommand SetActiveVibro
        {
            get
            {
                return _setActiveVibro ??= new RelayCommand((o) =>
                {
                    if (!_opc.cl.ReadBool(_tagVibro + ".Active", out var err))
                        Methods.ButtonClick(_tagVibro + ".Active", true, NameVibro + ". Активен");
                    else
                        Methods.ButtonClick(_tagVibro + ".Active", false, NameVibro + ". Не активен");
                });
            }
        }

        private RelayCommand _setOnQuantity;
        public RelayCommand SetOnQuantity
        {
            get
            {
                return _setOnQuantity ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameVibro + ". Количество включений", 0, 10, _tagVibro + ".Work_Count", WindowSetParameter.ValueType.Int16, null, 0, 0, 1, 3, 5, 1);
                });
            }
        }

        private RelayCommand _setOnTime;
        public RelayCommand SetOnTime
        {
            get
            {
                return _setOnTime ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameVibro + ". Длительность включений, с", 0.0, 10.0, _tagVibro + ".Work_Time", WindowSetParameter.ValueType.Real, null, 1, 0.5, 1.0, 2.0, 5.0, 1.0);
                });
            }
        }

        private RelayCommand _setPauseTime;
        public RelayCommand SetPauseTime
        {
            get
            {
                return _setPauseTime ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameVibro + ". Длительность паузы, с", 0.0, 3600.0, _tagVibro + ".Pause_Time", WindowSetParameter.ValueType.Real, null, 1, 1.0, 100.0, 1000.0, 3000.0, 50.0);
                });
            }
        }
    }
}
