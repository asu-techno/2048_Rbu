using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AS_Library.Link;
using _2048_Rbu.Classes;
using _2048_Rbu.Windows;
using AS_Library.Annotations;
using Opc.UaFx;
using Opc.UaFx.Client;
using ElOpt = _2048_Rbu.Elements.Settings.ElOpt;

namespace _2048_Rbu.Elements.Settings
{
    /// <summary>
    /// Логика взаимодействия для el_Screen.xaml
    /// </summary>
    public partial class ElMaterialSpeed : INotifyPropertyChanged
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        readonly List<ElOpt> _settings = new List<ElOpt>();

        public ElMaterialSpeed()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            DataContext = this;

            #region Settings

            for (int i = 0; i <= 11; i++)
            {
                _settings.Add(new ElOpt());
            }

            _settings[0].Initialize(_opcName, Static.СontainerNameDictionary[Static.ContainerItem.Silo1] + " (" + Static.СontainerMaterialDictionary[Static.ContainerItem.Silo1] + "). Предельная скорость дозирования, кг/с", 0, 100.0, "Cement.Load_Silo1", WindowSetParameter.ValueType.Real, 1, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[1].Initialize(_opcName, Static.СontainerNameDictionary[Static.ContainerItem.Silo2] + " (" + Static.СontainerMaterialDictionary[Static.ContainerItem.Silo2] + "). Предельная скорость дозирования, кг/с", 0, 100.0, "Cement.Load_Silo2", WindowSetParameter.ValueType.Real, 1, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[2].Initialize(_opcName, "Цемент. Предельная скорость сброса, кг/с", 0, 100.0, "Cement.Unload", WindowSetParameter.ValueType.Real, 1, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[3].Initialize(_opcName, Static.СontainerNameDictionary[Static.ContainerItem.Water] + " (" + Static.СontainerMaterialDictionary[Static.ContainerItem.Water] + "). Предельная скорость дозирования, кг/с", 0, 100.0, "Water.Load", WindowSetParameter.ValueType.Real, 1, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[4].Initialize(_opcName, "Вода. Предельная скорость сброса, кг/с", 0, 100.0, "Water.Unload", WindowSetParameter.ValueType.Real, 1, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[5].Initialize(_opcName, Static.СontainerNameDictionary[Static.ContainerItem.Additive1] + " (" + Static.СontainerMaterialDictionary[Static.ContainerItem.Additive1] + "). добавка №1. Предельная скорость дозирования, кг/с", 0, 100.0, "Additive.Load_Tank1", WindowSetParameter.ValueType.Real, 2, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[6].Initialize(_opcName, Static.СontainerNameDictionary[Static.ContainerItem.Additive2] + " (" + Static.СontainerMaterialDictionary[Static.ContainerItem.Additive2] + "). добавка №2. Предельная скорость дозирования, кг/с", 0, 100.0, "Additive.Load_Tank2", WindowSetParameter.ValueType.Real, 1, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[7].Initialize(_opcName, "Химическая добавка. Предельная скорость сброса, кг/с", 0, 100.0, "Additive.Unload", WindowSetParameter.ValueType.Real, 2, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[8].Initialize(_opcName, Static.СontainerNameDictionary[Static.ContainerItem.Bunker1] + " (" + Static.СontainerMaterialDictionary[Static.ContainerItem.Bunker1] + "). Предельная скорость дозирования, кг/с", 0, 100.0, "Inert.Load_Bunker1", WindowSetParameter.ValueType.Real, 1, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[9].Initialize(_opcName, Static.СontainerNameDictionary[Static.ContainerItem.Bunker2] + " (" + Static.СontainerMaterialDictionary[Static.ContainerItem.Bunker2] + "). Предельная скорость дозирования, кг/с", 0, 100.0, "Inert.Load_Bunker2", WindowSetParameter.ValueType.Real, 1, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[10].Initialize(_opcName, Static.СontainerNameDictionary[Static.ContainerItem.Bunker3] + " (" + Static.СontainerMaterialDictionary[Static.ContainerItem.Bunker3] + "). Предельная скорость дозирования, кг/с", 0, 100.0, "Inert.Load_Bunker3", WindowSetParameter.ValueType.Real, 1, 0.5, 5.0, 10.0, 50.0, 1.0);
            _settings[11].Initialize(_opcName, "Инертные материалы. Предельная скорость сброса, кг/с", 0, 100.0, "Inert.Unload", WindowSetParameter.ValueType.Real, 1, 0.5, 5.0, 10.0, 50.0, 1.0);


            foreach (var item in _settings)
            {
                Settings.Children.Add(item);

                item.Subscribe();
            }

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
            #endregion
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
