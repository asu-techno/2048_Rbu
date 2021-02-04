using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AS_Library.Classes;
using AsuBetonLibrary.Annotations;
using Microsoft.Xaml.Behaviors;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Логика взаимодействия для Window_setParameter_Volume.xaml
    /// </summary>
    public partial class WindowSetVolume : Window
    {
        private WindowSetVolumeViewModel WindowSetVolumeViewModel { get; set; }
        public delegate void ValueChangedHandler(string value);
        public event ValueChangedHandler ValueChanged;
        public WindowSetVolume(string name, double minVal, double maxVal, string value)
        {
            InitializeComponent();
            TextBox.Text = value;
            WindowSetVolumeViewModel = new WindowSetVolumeViewModel(name, minVal, maxVal, value);
            WindowSetVolumeViewModel.Close += WindowSetVolumeViewModel_Close;
            WindowSetVolumeViewModel.ValueChanged += WindowSetVolumeViewModel_ValueChanged;
            DataContext = WindowSetVolumeViewModel;
        }

        private void WindowSetVolumeViewModel_ValueChanged(string value)
        {
            ValueChanged?.Invoke(value);
        }

        private void WindowSetVolumeViewModel_Close()
        {
            Close();
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var tb = (TextBox) sender;
                WindowSetVolumeViewModel.SetParam(tb.Text);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            WindowSetVolumeViewModel.SetParam(TextBox.Text);
        }

        private void TextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            TextBox.Focus();
            TextBox.SelectAll();
        }
    }

    public class WindowSetVolumeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }
        public string Value { get; set; }

        public WindowSetVolumeViewModel(string name, double minVal, double maxVal, string value)
        {
            Name = name;
            MinVal = minVal;
            MaxVal = maxVal;
            Value = value;
        }

        public delegate void ValueChangedHandler(string value);
        public event ValueChangedHandler ValueChanged;
        public delegate void CloseHandler();
        public event CloseHandler Close;
        public bool Save(string value)
        {
            try
            {
                value = value.Replace(".", ",");
                if (Convert.ToDouble(value) <= MaxVal && Convert.ToDouble(value) >= MinVal)
                {
                    ValueChanged?.Invoke(value);
                    return true;
                }

                MessageBox.Show("Введите число в заданном диапазоне. Повторите ввод.");
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Неправильный формат числа. Повторите ввод.");
                return false;
            }
        }
        internal void SetParam(string value)
        {
            var isSaved = Save(value);
            if (isSaved)
            {
                Close?.Invoke();
            }
        }

        #region Commands

        private RelayCommand _setParameter;
        public RelayCommand SetParameter
        {
            get
            {
                return _setParameter ??= new RelayCommand((o) =>
                {
                    var param = (string)o;
                    var value = param ?? Value;
                    SetParam(value);
                });
            }
        }

        private RelayCommand _closeParameter;
        public RelayCommand CloseParameter
        {
            get
            {
                return _closeParameter ??= new RelayCommand((o) =>
                {
                    Close?.Invoke();
                });
            }
        }

       

        #endregion
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
    }

    public class SelectAllTextBoxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotKeyboardFocus += OnTextBoxGotFocus;
            AssociatedObject.TextChanged += OnTextBoxTextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.GotKeyboardFocus -= OnTextBoxGotFocus;
            AssociatedObject.TextChanged -= OnTextBoxTextChanged;
            base.OnDetaching();
        }

        private void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SelectAll();
        }

        private void OnTextBoxTextChanged(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SelectAll();
            AssociatedObject.TextChanged -= OnTextBoxTextChanged;
        }
    }
}
