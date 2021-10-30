using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Classes;
using AS_Library.Annotations;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Логика взаимодействия для xaml
    /// </summary>
    public partial class WindowWeight : INotifyPropertyChanged
    {
        public enum TypeMaterial
        {
            Cement = 1,
            Water,
            Additive,
            Inert
        }

        OpcServer.OpcList _opcName;
        private OPC_client _opc;
        private bool _err;
        private string _materialName;
        private string _tagName;
        private TypeMaterial _typeMaterial;

        private string _signEmpty;
        public string SignEmpty
        {
            get
            {
                return _signEmpty;
            }
            set
            {
                _signEmpty = value;
                OnPropertyChanged(nameof(SignEmpty));
            }
        }

        private bool _modeUseWeight;
        public bool ModeUseWeight
        {
            get
            {
                return _modeUseWeight;
            }
            set
            {
                _modeUseWeight = value;
                OnPropertyChanged(nameof(ModeUseWeight));
            }
        }

        public WindowWeight(OpcServer.OpcList opcName, TypeMaterial typeMaterial)
        {
            _opcName = opcName;
            _typeMaterial = typeMaterial;
            InitializeComponent();

            switch (_typeMaterial)
            {
                case TypeMaterial.Cement:
                    _tagName = "Cement";
                    _materialName = "цемента";
                    break;
                case TypeMaterial.Water:
                    _tagName = "Water";
                    _materialName = "воды";
                    break;
                case TypeMaterial.Additive:
                    _tagName = "Additive";
                    _materialName = "химических добавок";
                    break;
                case TypeMaterial.Inert:
                    _tagName = "Inert";
                    _materialName = "интертных материалов";
                    break;
            }

            KeyDown += OnKeyDown;
            Closed += Window_OnClosed;

            Title += _materialName;

            DataContext = this;

            Subscribe();
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
            var modeItem = new OpcMonitoredItem(_opc.cl.GetNode("UseWeight_" + _tagName), OpcAttribute.Value);
            modeItem.DataChangeReceived += HandleModeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeItem);

            var emptyItem = new OpcMonitoredItem(_opc.cl.GetNode("Scale_isEmpty_" + _tagName), OpcAttribute.Value);
            emptyItem.DataChangeReceived += HandleEmptyChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(emptyItem);

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
        }

        private void HandleModeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            ModeUseWeight = bool.Parse(e.Item.Value.ToString());
        }

        private void HandleEmptyChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            SignEmpty = double.Parse(e.Item.Value.ToString()).ToString(_typeMaterial==TypeMaterial.Additive ? "F2" : "F1");
        }

        private void LblEmpty_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;

            Methods.SetParameter(LblEmpty, btn, _opcName, "Весы " + _materialName + ". Признак пустоты, кг", 0.0, 100.0, "Scale_isEmpty_" + _tagName, WindowSetParameter.ValueType.Real, null, _typeMaterial == TypeMaterial.Additive ? 2:1, 10.0, 20.0, 50.0, 100.0, 10.0);
        }

        private void BtnUseWeight_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            if (!_opc.cl.ReadBool("UseWeight_" + _tagName, out _err))
                Methods.ButtonClick(btn, BtnUseWeight, "UseWeight_" + _tagName, true, "Весы " + _materialName + ". Использовать вес");
            else
                Methods.ButtonClick(btn, BtnUseWeight, "UseWeight_" + _tagName, false, "Весы " + _materialName + ". Не использовать вес");
        }

        private void BtnEndDosing_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnEndDosing, "btn_DozingDone_" + _tagName, true, "Весы " + _materialName + ". Закончить дозирование");
        }

        private void BtnEndUnload_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnEndUnload, "btn_UnloadingDone_" + _tagName, true, "Весы " + _materialName + ". Закончить выгрузку");
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                KeyDown -= OnKeyDown;
                Closed -= Window_OnClosed;
                Close();
            }
        }

        private void Window_OnClosed(object sender, EventArgs e)
        {
            KeyDown -= OnKeyDown;
            Closed -= Window_OnClosed;
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
