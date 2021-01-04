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
    public class VibratorSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum VibratorSettingsItem
        {
            M6,
            M7,
            M8,
            M91,
            M92,
            M101,
            M102,
            M111,
            M112,
            M121,
            M122
        }

        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        private string _tagVibro;

        public VibratorSettingsViewModel(OpcServer.OpcList opcName, VibratorSettingsItem vibratorSettingsItem)
        {
            _opcName = opcName;

            switch (vibratorSettingsItem)
            {
                case VibratorSettingsItem.M6:
                    _tagVibro = "M_6";
                    NameVibro = "Вибратор М-6";
                    break;
                case VibratorSettingsItem.M7:
                    _tagVibro = "M_7";
                    NameVibro = "Вибратор М-7";
                    break;
                case VibratorSettingsItem.M8:
                    _tagVibro = "M_8";
                    NameVibro = "Вибратор М-8";
                    break;
                case VibratorSettingsItem.M91:
                    _tagVibro = "M_9_1";
                    NameVibro = "Вибратор М-9-1";
                    break;
                case VibratorSettingsItem.M92:
                    _tagVibro = "M_9_2";
                    NameVibro = "Вибратор М-9-2";
                    break;
                case VibratorSettingsItem.M101:
                    _tagVibro = "M_10_1";
                    NameVibro = "Вибратор М-10-1";
                    break;
                case VibratorSettingsItem.M102:
                    _tagVibro = "M_10_2";
                    NameVibro = "Вибратор М-10-2";
                    break;
                case VibratorSettingsItem.M111:
                    _tagVibro = "M_11_1";
                    NameVibro = "Вибратор М-11-1";
                    break;
                case VibratorSettingsItem.M112:
                    _tagVibro = "M_11_2";
                    NameVibro = "Вибратор М-11-2";
                    break;
                case VibratorSettingsItem.M121:
                    _tagVibro = "M_12_1";
                    NameVibro = "Вибратор М-12-1";
                    break;
                case VibratorSettingsItem.M122:
                    _tagVibro = "M_12_2";
                    NameVibro = "Вибратор М-12-2";
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
            var activeVibroItem = new OpcMonitoredItem(_opc.cl.GetNode(_tagVibro+""), OpcAttribute.Value);
            activeVibroItem.DataChangeReceived += HandleActiveVibroChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(activeVibroItem);

            var onQuantityItem = new OpcMonitoredItem(_opc.cl.GetNode(_tagVibro + ""), OpcAttribute.Value);
            onQuantityItem.DataChangeReceived += HandleOnQuantityChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(onQuantityItem);

            var onTimeItem = new OpcMonitoredItem(_opc.cl.GetNode(_tagVibro + ""), OpcAttribute.Value);
            onTimeItem.DataChangeReceived += HandleOnTimeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(onTimeItem);

            var pauseTimeItem = new OpcMonitoredItem(_opc.cl.GetNode(_tagVibro + ""), OpcAttribute.Value);
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
            OnTime = double.Parse(e.Item.Value.ToString());
        }

        private void HandlePauseTimeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            PauseTime = double.Parse(e.Item.Value.ToString());
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

        private double _onTime;
        public double OnTime
        {
            get { return _onTime; }
            set
            {
                _onTime = value;
                OnPropertyChanged(nameof(OnTime));
            }
        }

        private double _pauseTime;
        public double PauseTime
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
                    if (!_opc.cl.ReadBool(_tagVibro, out var err))
                        Methods.ButtonClick(null, null, _tagVibro, true, NameVibro + ". Активен");
                    else
                        Methods.ButtonClick(null, null, _tagVibro, false, NameVibro + ". Не активен");
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
                    Methods.SetParameter(null, null, _opcName, NameVibro+". Количество включений", 0, 100, _tagVibro+"", "Real", null, 0, 0);
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
                    Methods.SetParameter(null, null, _opcName, NameVibro+". Длительность включений", 0, 100, _tagVibro + "", "Real", null, 0, 1);
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
                    Methods.SetParameter(null, null, _opcName, NameVibro + ". Длительность паузы", 0, 100, _tagVibro + "", "Real", null, 0, 1);
                });
            }
        }
    }
}
