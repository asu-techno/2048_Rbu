﻿using System;
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
using _2048_Rbu.Classes.ViewModel;
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

        private bool _permitDos;

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

        private bool _visError;
        public bool VisError
        {
            get
            {
                return _visError;
            }
            set
            {
                _visError = value;
                OnPropertyChanged(nameof(VisError));
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

        public string VisValue { get; set; }
        public string VisErrorTag { get; set; }
        public bool Logic { get; set; }
        public string Prefix { get; set; }
        public string ValuePcay { get; set; }
        public int Digit { get; set; }
        public WindowWeight.TypeMaterial TypeMaterial { get; set; }
        public Static.ContainerItem ContainerItem { get; set; }
        public string PermitTag { get; set; }
        public string EventPermit { get; set; }

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
                if (value == Brushes.Transparent)
                {
                    LblText.BorderBrush = LblValue.BorderBrush = value;
                }
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
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visItem);
            }
            if (VisErrorTag != null)
            {
                var visErrorItem = new OpcMonitoredItem(_opc.cl.GetNode(Prefix + VisErrorTag), OpcAttribute.Value);
                visErrorItem.DataChangeReceived += HandleVisErrorChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visErrorItem);
            }
            if (!string.IsNullOrEmpty(PermitTag))
            {
                _opc = OpcServer.GetInstance().GetOpc(_opcName);
                var permitDos = new OpcMonitoredItem(_opc.cl.GetNode(PermitTag), OpcAttribute.Value);
                permitDos.DataChangeReceived += HandlePermitDosChanged;
                OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(permitDos);
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
            try
            {
                Vis = bool.Parse(e.Item.Value.ToString()) == Logic ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleVisErrorChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                VisError = bool.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void HandlePermitDosChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _permitDos = bool.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void ElValueBox_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TypeMaterial != 0)
            {
                var window = new WindowWeight(_opcName, TypeMaterial);
                window.Show();
            }

            if (ContainerItem != 0)
            {
                var window = new WindowContainerSettings(new ContainerSettingsViewModel(_opcName, ContainerItem));
                window.Show();
            }
        }

        private void Rect_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (TypeMaterial != 0 || ContainerItem != 0)
            {
                RectObject.Opacity = 1;
                RectObject.ToolTip = TypeMaterial != 0 ? "Настройки весов\nПКМ для разрешения(запрета) сброса" : "Настройки массы емкости";
            }
        }

        private void Rect_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (TypeMaterial != 0 || ContainerItem != 0)
                RectObject.Opacity = 0;
        }

        private void RectObject_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(PermitTag))
            {
                if (_permitDos)
                    Methods.ButtonClick(PermitTag, false,
                        !string.IsNullOrEmpty(EventPermit) ? EventPermit + ". Запрет дозирования" : null);
                else
                    Methods.ButtonClick(PermitTag, true,
                        !string.IsNullOrEmpty(EventPermit) ? EventPermit + ". Разрешение дозирования" : null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
