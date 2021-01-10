using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using _2048_Rbu.Interfaces;
using AS_Library.Link;
using AS_Library.Annotations;
using _2048_Rbu.Classes;
using _2048_Rbu.Windows;
using Opc.UaFx;
using Opc.UaFx.Client;
using Unme.Common;

namespace _2048_Rbu.Elements.Indicators
{
    /// <summary>
    /// Interaction logic for el_SimpleValve.xaml
    /// </summary>
    public partial class ElValueBox : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private string _readVal;

        private WindowWeight _windowWeight;

        private Visibility _vis;
        public Visibility Vis
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

        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        #region MyRegion

        public int GroupNumber { get; set; }
        public string VisValue { get; set; }
        public bool Logic { get; set; }
        public string Prefix { get; set; }
        public string ValuePcay { get; set; }
        public int Digit { get; set; }

        private bool _isSmall;
        public bool IsSmall
        {
            get { return _isSmall; }
            set
            {
                if (value)
                {
                    LblText.Height = LblValue.Height = 18;
                    TxtText.FontSize = TxtValue.FontSize = 12;
                }
                _isSmall = value;
            }
        }

        private string _measure;
        public new string Measure
        {
            get { return _measure; }
            set
            {
                TxtText.Text = value;
                var length = value.Length;
                if (length != 0)
                    LblText.Width = IsSmall ? Math.Max(length * 5 + 1, 18) : Math.Max(length * 8 + 1, 25);
                else
                    LblText.Width = 0;
                _measure = value;
            }
        }

        private int _count;
        public int Count
        {
            get { return _count; }
            set
            {
                var length = value;
                LblValue.Width = Math.Max(length * 10 + 1, 20);
                LblText.Margin = new Thickness(LblValue.Width - 1, 0, 0, 0);
                _count = value;
            }
        }

        private Brush _color;
        public Brush Color
        {
            set
            {
                LblText.Background = value;
                LblValue.Background = value;
                _color = value;
            }
            get
            {
                return _color;
            }
        }

        #endregion

        public ElValueBox()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName, string readVal = null)
        {
            _opcName = opcName;
            _readVal = readVal;

            DataContext = this;
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
            var readVal = _readVal ?? ValuePcay;
            var valueItem = new OpcMonitoredItem(_opc.cl.GetNode(Prefix + readVal), OpcAttribute.Value);
            valueItem.DataChangeReceived += HandleValueChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(valueItem);
            if (VisValue != null)
            {
                var visItem = new OpcMonitoredItem(_opc.cl.GetNode(Prefix + VisValue), OpcAttribute.Value);
                visItem.DataChangeReceived += HandleVisChanged;
                visItem.SamplingInterval = 200;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visItem);
            }
        }

        private void HandleValueChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                Value = decimal.Parse(e.Item.Value.ToString(), System.Globalization.NumberStyles.Float).ToString($"F{Digit}");
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleVisChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            Vis = bool.Parse(e.Item.Value.ToString()) == Logic ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ElValueBox_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GroupNumber != 0)
            {
                _windowWeight = new WindowWeight(_opcName, (WindowWeight.TypeMaterial)GroupNumber);
                _windowWeight.ShowDialog();
            }
        }

        private void Rect_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (GroupNumber != 0)
                RectObject.Opacity = 1;
        }

        private void Rect_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (GroupNumber != 0)
                RectObject.Opacity = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
