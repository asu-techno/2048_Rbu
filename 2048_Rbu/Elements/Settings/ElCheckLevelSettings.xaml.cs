using System.Windows.Controls;
using _2048_Rbu.Classes;
using _2048_Rbu.Classes.ViewModel;

namespace _2048_Rbu.Elements.Settings
{
    /// <summary>
    /// Interaction logic for ElContainerSettings.xaml
    /// </summary>
    public partial class ElCheckLevelSettings : UserControl
    {
        public ElCheckLevelSettings()
        {
            InitializeComponent();
        }

        public void Initialize(CheckLevelSettingsViewModel checkLevelSettingsView)
        {
            DataContext = checkLevelSettingsView;
        }
    }
}
