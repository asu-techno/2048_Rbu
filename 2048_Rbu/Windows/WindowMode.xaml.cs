using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Classes;
using AS_Library.Annotations;
using AS_Library.Link;
using _2048_Rbu.Classes;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Логика взаимодействия для xaml
    /// </summary>
    public partial class WindowMode : INotifyPropertyChanged
    {
        OpcServer.OpcList _opcName;
        public delegate void CloseHandler();
        public event CloseHandler StopUpdate;
        private OPC_client _opc;
        private bool _err;

        private bool _modeAutomat;
        public bool ModeAutomat
        {
            get
            {
                return _modeAutomat;
            }
            set
            {
                _modeAutomat = value;
                OnPropertyChanged(nameof(ModeAutomat));
            }
        }

        private bool _modeManual;
        public bool ModeManual
        {
            get
            {
                return _modeManual;
            }
            set
            {
                _modeManual = value;
                OnPropertyChanged(nameof(ModeManual));
            }
        }

        public WindowMode(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            InitializeComponent();
            KeyDown += OnKeyDown;
            Closed += Window_OnClosed;

            DataContext = this;
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
            var manualItem = new OpcMonitoredItem(_opc.cl.GetNode("gMode_Manual"), OpcAttribute.Value);
            manualItem.DataChangeReceived += HandleManualChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(manualItem);

            var automatItem = new OpcMonitoredItem(_opc.cl.GetNode("gMode_Automat"), OpcAttribute.Value);
            automatItem.DataChangeReceived += HandleAutomatChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(automatItem);

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
        }

        private void HandleManualChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            ModeManual = bool.Parse(e.Item.Value.ToString());
        }

        private void HandleAutomatChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            ModeAutomat = bool.Parse(e.Item.Value.ToString());
        }

        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnManual, "gMode_Manual", true, "Ручной режим работы узла");
            Methods.ButtonClick(btn, BtnManual, "gMode_Automat", false);
        }

        private void BtnAutomat_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnAutomat, "gMode_Automat", true, "Автоматический режим работы узла");
            Methods.ButtonClick(btn, BtnAutomat, "gMode_Manual", false);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (StopUpdate != null) StopUpdate();
                KeyDown -= OnKeyDown;
                Closed -= Window_OnClosed;
                Close();
            }
        }

        private void Window_OnClosed(object sender, EventArgs e)
        {
            if (StopUpdate != null) StopUpdate();
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
