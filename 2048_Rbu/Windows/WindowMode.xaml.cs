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
    public partial class WindowMode : INotifyPropertyChanged
    {
        public delegate void CloseHandler();
        public event CloseHandler StopUpdate;

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

        private void BtnManual_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ModeManual = true;
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnManual, "btn_All_Manual", true, "Перевод всех механизмов в ручной режим работы");
        }

        private void BtnManual_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ModeManual = false;
        }

        private void BtnAutomat_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ModeAutomat = true;
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnAutomat, "btn_All_Automat", true, "Перевод всех механизмов в автоматический режим работы");
        }

        private void BtnAutomat_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ModeAutomat = false;
        }

        public WindowMode()
        {
            InitializeComponent();
            KeyDown += OnKeyDown;
            Closed += Window_OnClosed;

            DataContext = this;
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
