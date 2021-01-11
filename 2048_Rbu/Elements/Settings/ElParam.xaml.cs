using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AS_Library.Link;
using _2048_Rbu.Classes;
using AS_Library.Annotations;
using ElOpt = _2048_Rbu.Elements.Settings.ElOpt;

namespace _2048_Rbu.Elements.Settings
{
    /// <summary>
    /// Логика взаимодействия для el_Screen.xaml
    /// </summary>
    public partial class ElParam : UserControl
    {
        private OpcServer.OpcList _opcName;

        readonly List<ElOpt> _settings = new List<ElOpt>();

        public ElParam()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            #region Settings

            for (int i = 0; i <= 8; i++)
            {
                _settings.Add(new ElOpt());
            }

            _settings[0].Initialize(_opcName, "Объем бетоносмесителя, м³", 0, 20, "Mixer_Volume", "Real", 0, 1);
            _settings[1].Initialize(_opcName, "Время движения инертных материалов по конвейеру, с", 0, 100, "UnloadInert_TimeAfterEmptyWeights", "Int16", 0, 0);
            _settings[2].Initialize(_opcName, "Задержка выгрузки воды и хим. добавок, с", 0, 100, "UnloadWater_Delay", "Int16", 0, 0);
            _settings[3].Initialize(_opcName, "Задержка выгрузки цемента, с", 0, 100, "UnloadCement_Delay", "Int16", 0, 0);
            _settings[4].Initialize(_opcName, "Время полного открытия выгрузной задвижки, с", 0, 100, "FullOpenTime_V_1", "Int16", 0, 0);
            _settings[5].Initialize(_opcName, "Минимальное значение индикатора тока, А", 0, 500, "Current_Min", "Real", 0, 1);
            _settings[6].Initialize(_opcName, "Максимальное значение индикатора тока, А", 0, 500, "Current_Max", "Real", 0, 1);
            _settings[7].Initialize(_opcName, "Минимальное допустимое значение тока, А", 0, 500, "Current_NormalMin", "Real", 0, 1);
            _settings[8].Initialize(_opcName, "Максимальное допустимое значение тока, А", 0, 500, "Current_NormalMax", "Real", 0, 1);


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
