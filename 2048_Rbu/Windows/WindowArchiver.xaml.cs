using System.Windows;
using ArchiverLibCore.Elements;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowArchiver : Window
    {
        public WindowArchiver(ElArchiversViewModel elArchiversViewModel)
        {
            InitializeComponent();

            Archivers.DataContext = elArchiversViewModel;
        }
    }
}
