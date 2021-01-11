using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for WindowPostParam.xaml
    /// </summary>
    public partial class WindowParam : Window
    {
        public delegate void CloseHandler();
        public event CloseHandler StopUpdate;

        OpcServer.OpcList _opcName;
        
        public WindowParam(OpcServer.OpcList opcName)
        {
            InitializeComponent();

            KeyDown += OnKeyDown;
            Closed += Window_OnClosed;

            _opcName = opcName;

            Param.Initialize(_opcName);
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
