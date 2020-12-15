using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Reflection;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Логика взаимодействия для Window_Wait.xaml
    /// </summary>
    public partial class WindowSplash : Window
    {
        public WindowSplash()
        {
            InitializeComponent();
            Version.Text += " v." + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void Win_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.AbsoluteUri);
                e.Handled = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Почтовый клиент не найден","Ошибка");
            }
        }
    }
}
