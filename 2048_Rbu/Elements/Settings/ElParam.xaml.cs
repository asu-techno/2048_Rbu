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
    public partial class ElParam : INotifyPropertyChanged
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        readonly List<ElOpt> _settings = new List<ElOpt>();

        private bool _bySensor;
        public bool BySensor
        {
            get { return _bySensor; }
            set
            {
                _bySensor = value;
                OnPropertyChanged(nameof(BySensor));
            }
        }

        public ElParam()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            DataContext = this;

            #region Settings

            for (int i = 0; i <= 8; i++)
            {
                _settings.Add(new ElOpt());
            }

            _settings[0].Initialize(_opcName, "Объем бетоносмесителя, м³", 0, 20.0, "Mixer_Volume", WindowSetParameter.ValueType.Real, 1, 0.5, 1.0, 5.0, 10.0, 1.0);
            _settings[1].Initialize(_opcName, "Время движения инертных материалов по конвейеру, с", 0, 100, "UnloadInert_TimeAfterEmptyWeights", WindowSetParameter.ValueType.Int16, 0, 5, 10, 20, 50, 5);
            _settings[2].Initialize(_opcName, "Задержка выгрузки воды и хим. добавок, с", 0, 100, "UnloadWater_Delay", WindowSetParameter.ValueType.Int16, 0, 5, 10, 20, 50, 5);
            _settings[3].Initialize(_opcName, "Задержка выгрузки цемента, с", 0, 100, "UnloadCement_Delay", WindowSetParameter.ValueType.Int16, 0, 5, 10, 20, 50, 5);
            _settings[4].Initialize(_opcName, "Время полного открытия выгрузной задвижки, с", 0, 100, "FullOpenTime_V_1", WindowSetParameter.ValueType.Int16, 0, 5, 10, 20, 50, 5);
            _settings[5].Initialize(_opcName, "Минимальное значение индикатора тока, А", 0, 500.0, "Current_Min", WindowSetParameter.ValueType.Real, 1, 20, 50, 100, 200, 5);
            _settings[6].Initialize(_opcName, "Максимальное значение индикатора тока, А", 0, 500.0, "Current_Max", WindowSetParameter.ValueType.Real, 1, 20, 50, 100, 200, 5);
            _settings[7].Initialize(_opcName, "Минимальное допустимое значение тока, А", 0, 500.0, "Current_NormalMin", WindowSetParameter.ValueType.Real, 1, 20, 50, 100, 200, 5);
            _settings[8].Initialize(_opcName, "Максимальное допустимое значение тока, А", 0, 500.0, "Current_NormalMax", WindowSetParameter.ValueType.Real, 1, 20, 50, 100, 200, 5);


            foreach (var item in _settings)
            {
                Settings.Children.Add(item);

                item.Subscribe();
            }

            Subscribe();

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
            #endregion
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
            var bySensorItem = new OpcMonitoredItem(_opc.cl.GetNode("WorkByMaterialSensor"), OpcAttribute.Value);
            bySensorItem.DataChangeReceived += HandleBySensorChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(bySensorItem);
        }

        private void HandleBySensorChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            BySensor = bool.Parse(e.Item.Value.ToString());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BtnTimer_OnClick(object sender, RoutedEventArgs e)
        {
            Methods.ButtonClick("WorkByMaterialSensor", false, "Режим выгрузки компонентов - по времени");
        }

        private void BtnSensor_OnClick(object sender, RoutedEventArgs e)
        {
            Methods.ButtonClick("WorkByMaterialSensor", true, "Режим выгрузки компонентов - по датчику");
        }
    }
}
