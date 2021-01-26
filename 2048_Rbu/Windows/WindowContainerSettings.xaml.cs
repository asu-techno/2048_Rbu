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
using _2048_Rbu.Elements.Indicators;
using _2048_Rbu.Elements.Settings;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Interaction logic for WindowDosingSettings.xaml
    /// </summary>
    public partial class WindowContainerSettings : Window
    {
        public delegate void CloseHandler();
        public event CloseHandler StopUpdate;
        public WindowContainerSettings(ContainerSettingsViewModel containerSettingsViewModel)
        {
            InitializeComponent();

            var elContainerSettings = new ElContainerSettings();
            elContainerSettings.Initialize(containerSettingsViewModel);

            MainWrapPanel.Children.Add(elContainerSettings);

            DataContext = containerSettingsViewModel;

            KeyDown += OnKeyDown;
            Closed += Window_OnClosed;
        }

        public WindowContainerSettings()
        {
            InitializeComponent();

            var listContainers = new List<ElContainerSettings>();
            for (int i = 0; i < 7; i++)
            {
                listContainers.Add(new ElContainerSettings());
            }

            listContainers[0].Initialize(new ContainerSettingsViewModel(OpcServer.OpcList.Rbu, Static.ContainerItem.Silo1));
            listContainers[1].Initialize(new ContainerSettingsViewModel(OpcServer.OpcList.Rbu, Static.ContainerItem.Silo2));
            listContainers[2].Initialize(new ContainerSettingsViewModel(OpcServer.OpcList.Rbu, Static.ContainerItem.Additive1));
            listContainers[3].Initialize(new ContainerSettingsViewModel(OpcServer.OpcList.Rbu, Static.ContainerItem.Additive2));
            listContainers[4].Initialize(new ContainerSettingsViewModel(OpcServer.OpcList.Rbu, Static.ContainerItem.Bunker1));
            listContainers[5].Initialize(new ContainerSettingsViewModel(OpcServer.OpcList.Rbu, Static.ContainerItem.Bunker2));
            listContainers[6].Initialize(new ContainerSettingsViewModel(OpcServer.OpcList.Rbu, Static.ContainerItem.Bunker3));
            //listContainers[7].Initialize(new ContainerSettingsViewModel(OpcServer.OpcList.Rbu, Static.ContainerItem.Bunker4));

            foreach (var item in listContainers)
            {
                MainWrapPanel.Children.Add(item);
            }

            Title = "Настройки масс емкостей";

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
