using System.Windows.Controls;
using _2048_Rbu.Classes;
using _2048_Rbu.Classes.ViewModel;

namespace _2048_Rbu.Elements.Settings
{
    /// <summary>
    /// Interaction logic for ElVibratorSettings.xaml
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
