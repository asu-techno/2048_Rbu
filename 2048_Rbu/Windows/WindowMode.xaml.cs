using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Classes;
using AS_Library.Annotations;
using AS_Library.Link;
using Lib_2048.Classes;

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

        public void Update()
        {
            if (_opc != null)
            {
                ModeManual = _opc.cl.ReadBool("gMode_Manual", out _err);
                ModeAutomat = _opc.cl.ReadBool("gMode_Automat", out _err);
            }
            else
            {
                _opc = OpcServer.GetInstance().GetOpc(_opcName);
            }
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
