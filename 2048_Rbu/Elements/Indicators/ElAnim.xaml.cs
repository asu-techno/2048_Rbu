using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Indicators
{
    /// <summary>
    /// Interaction logic for el_SimpleValve.xaml
    /// </summary>
    public partial class ElAnim : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        public enum Environment { Solution, Water, Inert, Cement}

        private Visibility _visSens;

        public Visibility VisSens
        {
            get
            {
                return _visSens;
            }
            set
            {
                _visSens = value;
                OnPropertyChanged(nameof(VisSens));
            }
        }

        #region MyRegion

        public string Prefix { get; set; }
        public string OnPcy { get; set; }
        public bool Logic { get; set; }

        private Environment _substance;

        public Environment Substance
        {
            get { return _substance; }
            set
            {
                if (value == Environment.Cement)
                    ImgSubstance.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Indicators/img_Cement.png", UriKind.Relative));
                if (value == Environment.Water)
                    ImgSubstance.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Indicators/img_Water.png", UriKind.Relative));
                if (value == Environment.Inert)
                    ImgSubstance.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Indicators/img_Inert.png", UriKind.Relative));
                if (value == Environment.Solution)
                    ImgSubstance.Source = new BitmapImage(new Uri("/2048_Rbu;component/Images/Indicators/img_Solution.png", UriKind.Relative));

                _substance = value;
            }
        }

        #endregion

        public ElAnim()
        {
            InitializeComponent();

            HideImages();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

            DataContext = this;
        }

        void HideImages()
        {
            VisSens = Visibility.Collapsed;
        }

        public void Subscribe()
        {
            CreateSubscription();
        }

        public void Unsubscribe()
        {
        }

        private void CreateSubscription()
        {
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var visItem = new OpcMonitoredItem(_opc.cl.GetNode(Prefix + OnPcy), OpcAttribute.Value);
            visItem.DataChangeReceived += HandleVisChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visItem);
        }

        private void HandleVisChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            VisSens = (bool.Parse(e.Item.Value.ToString()) == Logic) ? Visibility.Visible : Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
