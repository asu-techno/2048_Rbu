using System.Windows;
using System.Windows.Controls;
using _2048_Rbu.Classes;
using _2048_Rbu.Classes.ViewModel;

namespace _2048_Rbu.Elements.Settings
{
    /// <summary>
    /// Interaction logic for ElContainerSettings.xaml
    /// </summary>
    public partial class ElContainerSettings : UserControl
    {
        public ElContainerSettings()
        {
            InitializeComponent();
        }

        public void Initialize(ContainerSettingsViewModel containerSettingsViewModel)
        {
            DataContext = containerSettingsViewModel;
        }
    }
}
