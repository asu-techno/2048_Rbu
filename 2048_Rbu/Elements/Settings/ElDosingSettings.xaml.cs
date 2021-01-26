using System.Windows.Controls;
using _2048_Rbu.Classes;
using _2048_Rbu.Classes.ViewModel;

namespace _2048_Rbu.Elements.Settings
{
    /// <summary>
    /// Interaction logic for ElDosingSettings.xaml
    /// </summary>
    public partial class ElDosingSettings : UserControl
    {
        public ElDosingSettings()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName, DosingSettingsViewModel dosingSettingsViewModel)
        {
            DataContext = dosingSettingsViewModel;
        }
    }
}
