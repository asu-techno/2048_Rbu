using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Reflection;
using System.Runtime.CompilerServices;
using _2048_Rbu.Classes;
using AS_Library.Annotations;
using ServiceLib.Classes;
using Unme.Common;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Логика взаимодействия для Window_Wait.xaml
    /// </summary>
    public partial class WindowSplash : INotifyPropertyChanged
    {
        private double _posX, _posY;

        private string _projectName;
        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
            }
        }

        private string _projectСipher;
        public string ProjectСipher
        {
            get { return _projectСipher; }
            set
            {
                _projectСipher = value;
                OnPropertyChanged(nameof(ProjectСipher));
            }
        }

        private string _copyright;
        public string Copyright
        {
            get { return _copyright; }
            set
            {
                _copyright = value;
                OnPropertyChanged(nameof(Copyright));
            }
        }

        public WindowSplash()
        {
            InitializeComponent();

            string cipher = ServiceData.GetInstance().GetContractNubmer();
            ProjectName = ServiceData.GetInstance().GetTitle();
            ProjectСipher = (cipher.IsNullOrEmpty() ? "" : "АНКМ." + cipher + ".030") + Static.Version;
            Copyright = Static.Copyright;

            DataContext = this;
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
                MessageBox.Show("Почтовый клиент не найден", "Ошибка");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_posX < 0 || _posX > this.Width || _posY < 0 || _posY > this.Height)
                Close();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(this);

            _posX = p.X;
            _posY = p.Y;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Mouse.Capture(this, CaptureMode.SubTree);
        }
    }
}
