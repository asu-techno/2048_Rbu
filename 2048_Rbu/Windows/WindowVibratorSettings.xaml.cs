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
    /// Interaction logic for WindowVibratorSettings.xaml
    /// </summary>
    public partial class WindowVibratorSettings : Window
    {
        public delegate void CloseHandler();
        public event CloseHandler StopUpdate;
        public WindowVibratorSettings(OpcServer.OpcList opcName)
        {
            InitializeComponent();

            ElVibratorM13.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M13);
            ElVibratorM14.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M14);
            ElVibratorM10.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M10);
            ElVibratorM2.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M2);
            ElVibratorM3.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M3);
            ElVibratorM4.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M4);
            ElVibratorM5.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M5);
            ElVibratorM6.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M6);
            ElVibratorM7.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M7);
            //ElVibratorM121.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M121);
            //ElVibratorM122.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M122);

            VibratorsParameter.Visibility = Visibility.Visible;

            KeyDown += OnKeyDown;
            Closed += Window_OnClosed;
        }

        public WindowVibratorSettings(OpcServer.OpcList opcName, bool isAeration)
        {
            InitializeComponent();

            Title = "Настройки работы аэраторов";

            ElAeration1.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.Aeration1);
            ElAeration2.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.Aeration2);
            
            AeratorsParameter.Visibility = Visibility.Visible;

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
