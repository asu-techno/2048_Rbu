using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using AS_Library.Link;
using _2048_Rbu.Classes;
using AS_Library.Annotations;
using _2048_Rbu.Classes;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Control
{
    /// <summary>
    /// Логика взаимодействия для el_AutoControl.xaml
    /// </summary>
    public partial class ElDosingWait : INotifyPropertyChanged
    {
        private OpcServer.OpcList _opcName;

        private bool _vis;
        public bool Vis
        {
            get
            {
                return _vis;
            }
            set
            {
                _vis = value;
                OnPropertyChanged(nameof(Vis));
            }
        }

        public ElDosingWait()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            DataContext = this;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
        }

        private void BtnContinue_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;//здесь
            Methods.ButtonClick(btn, BtnContinue, "btn_Stop", true, "Дозирование цемента. Продолжить");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
