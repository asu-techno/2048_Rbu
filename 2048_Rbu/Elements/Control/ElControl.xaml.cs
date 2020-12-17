using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AS_Library.Link;
using Lib_2048.Classes;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;

namespace _2048_Rbu.Elements.Control
{
    /// <summary>
    /// Interaction logic for el_SimpleValve.xaml
    /// </summary>
    public partial class ElControl : INotifyPropertyChanged
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private bool _err;

        private bool _running;
        public bool Running
        {
            get
            {
                return _running;
            }
            set
            {
                _running = value;
                OnPropertyChanged(nameof(Running));
            }
        }

        private bool _readyStart;
        public bool ReadyStart
        {
            get
            {
                return _readyStart;
            }
            set
            {
                _readyStart = value;
                OnPropertyChanged(nameof(ReadyStart));
            }
        }

        private bool _readyStop;
        public bool ReadyStop
        {
            get
            {
                return _readyStop;
            }
            set
            {
                _readyStop = value;
                OnPropertyChanged(nameof(ReadyStop));
            }
        }

        public ElControl()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName, string marhName, string marhOpcName)
        {
            _opcName = opcName;

            DataContext = this;
        }

        public void Update()
        {
            if (_opc != null)
            {
                Running = _opc.cl.ReadBool("Marh_isRunning", out _err);
                ReadyStart = _opc.cl.ReadBool("btn_rdy_Start", out _err);
                ReadyStop = _opc.cl.ReadBool("btn_rdy_Stop", out _err);
            }
            else
            {
                _opc = OpcServer.GetInstance().GetOpc(_opcName);
            }
        }

        private void Rect_OnMouseEnter(object sender, MouseEventArgs e)
        {
            //rect_object.Opacity = 1;
        }

        private void Rect_OnMouseLeave(object sender, MouseEventArgs e)
        {
            //rect_object.Opacity = 0;
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnStart, "btn_Start", true, "Маршрут Старт");
        }

        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnStop, "btn_Stop", true, "Маршрут Стоп");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
