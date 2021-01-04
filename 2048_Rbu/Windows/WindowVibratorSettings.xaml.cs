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

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Interaction logic for WindowVibratorSettings.xaml
    /// </summary>
    public partial class WindowVibratorSettings : Window
    {
        public WindowVibratorSettings(OpcServer.OpcList opcName)
        {
            InitializeComponent();

            ElVibratorM6.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M6);
            ElVibratorM7.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M7);
            ElVibratorM8.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M8);
            ElVibratorM91.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M91);
            ElVibratorM92.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M92);
            ElVibratorM101.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M101);
            ElVibratorM102.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M102);
            ElVibratorM111.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M111);
            ElVibratorM112.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M112);
            ElVibratorM121.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M121);
            ElVibratorM122.Initialize(opcName, VibratorSettingsViewModel.VibratorSettingsItem.M122);
        }
    }
}
