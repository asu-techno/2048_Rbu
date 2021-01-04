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
using System.Windows.Navigation;
using System.Windows.Shapes;
using _2048_Rbu.Classes;

namespace _2048_Rbu.Elements.Settings
{
    /// <summary>
    /// Interaction logic for Vibrator.xaml
    /// </summary>
    public partial class ElVibratorSettings : UserControl
    {
        public ElVibratorSettings()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName, VibratorSettingsViewModel.VibratorSettingsItem vibratorSettingsItem)
        {
            VibratorSettingsViewModel vibratorSettingsViewModel = new VibratorSettingsViewModel(opcName, vibratorSettingsItem);

            DataContext = vibratorSettingsViewModel;
        }
    }
}
