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
    public class ContainerSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private OPC_client _opc;
        private OpcServer.OpcList _opcName;


        public static Dictionary<Static.ContainerItem, int> _contNumDictionary = new Dictionary<Static.ContainerItem, int>
            {
                {Static.ContainerItem.Additive1, (int)Static.ContainerItem.Additive1},
                {Static.ContainerItem.Additive2, (int)Static.ContainerItem.Additive2},
                {Static.ContainerItem.Silo1, (int)Static.ContainerItem.Silo1},
                {Static.ContainerItem.Silo2, (int)Static.ContainerItem.Silo2},
                {Static.ContainerItem.Water, (int)Static.ContainerItem.Water},
                {Static.ContainerItem.Bunker1, (int)Static.ContainerItem.Bunker1},
                {Static.ContainerItem.Bunker2, (int)Static.ContainerItem.Bunker2},
                {Static.ContainerItem.Bunker3, (int)Static.ContainerItem.Bunker3},
                {Static.ContainerItem.Bunker4, (int)Static.ContainerItem.Bunker4}
            };

        private Static.ContainerItem _containerItem;

        public ContainerSettingsViewModel(OpcServer.OpcList opcName, Static.ContainerItem containerSettingsItem)
        {
            _opcName = opcName;
            _containerItem = containerSettingsItem;

            NameContainer = "Настройки массы емкости. "+Static.СontainerNameDictionary[_containerItem]+" (" + Static.СontainerMaterialDictionary[_containerItem]+")";

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
            var addVolumeItem = new OpcMonitoredItem(_opc.cl.GetNode("Add_Volume[" + _contNumDictionary[_containerItem] + "]"), OpcAttribute.Value);
            addVolumeItem.DataChangeReceived += HandleAddVolumeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(addVolumeItem);

            var parVolumeItem = new OpcMonitoredItem(_opc.cl.GetNode("PAR_Volume[" + _contNumDictionary[_containerItem] + "]"), OpcAttribute.Value);
            parVolumeItem.DataChangeReceived += HandleParVolumeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(parVolumeItem);

            var currentVolumeItem = new OpcMonitoredItem(_opc.cl.GetNode("Volume[" + _contNumDictionary[_containerItem] + "]"), OpcAttribute.Value);
            currentVolumeItem.DataChangeReceived += HandleCurrentVolumeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(currentVolumeItem);

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
        }

        private void HandleAddVolumeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            AddVolume = double.Parse(e.Item.Value.ToString());
        }

        private void HandleParVolumeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            ParVolume = double.Parse(e.Item.Value.ToString());
        }

        private void HandleCurrentVolumeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            CurrentVolume = double.Parse(e.Item.Value.ToString());
        }

        private string _nameContainer;
        public string NameContainer
        {
            get { return _nameContainer; }
            set
            {
                _nameContainer = value;
                OnPropertyChanged(nameof(NameContainer));
            }
        }

        private double _addVolume;
        public double AddVolume
        {
            get { return _addVolume; }
            set
            {
                _addVolume = value;
                OnPropertyChanged(nameof(AddVolume));
            }
        }

        private double _parVolume;
        public double ParVolume
        {
            get { return _parVolume; }
            set
            {
                _parVolume = value;
                OnPropertyChanged(nameof(ParVolume));
            }
        }

        private double _currentVolume;
        public double CurrentVolume
        {
            get { return _currentVolume; }
            set
            {
                _currentVolume = value;
                OnPropertyChanged(nameof(CurrentVolume));
            }
        }

        private RelayCommand _setAddVolume;
        public RelayCommand SetAddVolume
        {
            get
            {
                return _setAddVolume ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameContainer + ". Масса загружаемого материала", 0, 10000,
                        "Add_Volume[" + _contNumDictionary[_containerItem] + "]", WindowSetParameter.ValueType.Real, null, 1, 100, 200, 500, 1000, 100);
                });
            }
        }

        private RelayCommand _setParVolume;
        public RelayCommand SetParVolume
        {
            get
            {
                return _setParVolume ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameContainer + ". Вместимость емкости", 0, 10000,
                        "PAR_Volume[" + _contNumDictionary[_containerItem] + "]", WindowSetParameter.ValueType.Real, null, 1, 100, 200, 500, 1000, 100);
                });
            }
        }

        private RelayCommand _setVolume;
        public RelayCommand SetVolume
        {
            get
            {
                return _setVolume ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameContainer + ". Текущая масса, кг", 0, 10000,
                        "Volume[" + _contNumDictionary[_containerItem] + "]", WindowSetParameter.ValueType.Real, null, 1, 100, 200, 500, 1000, 100);
                });
            }
        }

        private RelayCommand _setAdd;
        public RelayCommand SetAdd
        {
            get
            {
                return _setAdd ??= new RelayCommand((o) =>
                {
                    Methods.ButtonClick("btn_Add[" + _contNumDictionary[_containerItem] + "]", true, NameContainer + ". Загрузка заданной массы материала");
                });
            }
        }
    }
}
