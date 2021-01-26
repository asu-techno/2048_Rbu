using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using _2048_Rbu.Classes;
using _2048_Rbu.Classes.ViewModel;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Interaction logic for WindowDosingSettings.xaml
    /// </summary>
    public partial class WindowDosingSettings : Window
    {
        public delegate void CloseHandler();
        public event CloseHandler StopUpdate;
        public WindowDosingSettings(OpcServer.OpcList opcName, DosingSettingsViewModel dosingSettingsViewModel)
        {
            InitializeComponent();

            ElDosingSettings.Initialize(opcName, dosingSettingsViewModel);

            DataContext = dosingSettingsViewModel;

            KeyDown += OnKeyDown;
            Closed += Window_OnClosed;
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
    }
}
