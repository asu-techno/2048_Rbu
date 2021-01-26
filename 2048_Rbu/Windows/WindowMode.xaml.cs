using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Classes;
using AS_Library.Annotations;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Windows
{
    /// <summary>
    /// Логика взаимодействия для xaml
    /// </summary>
    public partial class WindowMode : Window
    {
        public WindowMode()
        {
            InitializeComponent();
            KeyDown += OnKeyDown;
            Closed += Window_OnClosed;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                KeyDown -= OnKeyDown;
                Closed -= Window_OnClosed;
                Close();
            }
        }

        private void Window_OnClosed(object sender, EventArgs e)
        {
            KeyDown -= OnKeyDown;
            Closed -= Window_OnClosed;
            Close();
        }

        private void BtnManual_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnManual, "btn_All_Manual", true, "Перевод всех механизмов в ручной режим работы");
        }

        private void BtnAutomat_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnAutomat, "btn_All_Automat", true, "Перевод всех механизмов в автоматический режим работы");
        }
    }
}
