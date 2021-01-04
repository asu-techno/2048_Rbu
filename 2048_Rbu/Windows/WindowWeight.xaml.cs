﻿using System;
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
    public partial class WindowWeight : INotifyPropertyChanged
    {
        OpcServer.OpcList _opcName;
        public delegate void CloseHandler();
        public event CloseHandler StopUpdate;
        private OPC_client _opc;
        private bool _err;
        private string _materialName;
        private string _tagName;

        public enum TypeMaterial
        {
            Cement = 1,
            Water,
            Additive,
            Inert
        }

        private double _signEmpty;
        public double SignEmpty
        {
            get
            {
                return _signEmpty;
            }
            set
            {
                _signEmpty = value;
                OnPropertyChanged(nameof(SignEmpty));
            }
        }

        private bool _modeUseWeight;
        public bool ModeUseWeight
        {
            get
            {
                return _modeUseWeight;
            }
            set
            {
                _modeUseWeight = value;
                OnPropertyChanged(nameof(ModeUseWeight));
            }
        }

        private bool _btnClick;
        public bool BtnClick
        {
            get
            {
                return _btnClick;
            }
            set
            {
                _btnClick = value;
                OnPropertyChanged(nameof(BtnClick));
            }
        }

        public WindowWeight(OpcServer.OpcList opcName, TypeMaterial typeMaterial)
        {
            _opcName = opcName;

            InitializeComponent();

            switch (typeMaterial)
            {
                case TypeMaterial.Cement:
                    _tagName = "Cement";
                    _materialName = "цемента";
                    break;
                case TypeMaterial.Water:
                    _tagName = "Water";
                    _materialName = "воды";
                    break;
                case TypeMaterial.Additive:
                    _tagName = "Additive";
                    _materialName = "химических добавок";
                    break;
                case TypeMaterial.Inert:
                    _tagName = "Inert";
                    _materialName = "интертных материалов";
                    break;
            }

            KeyDown += OnKeyDown;
            Closed += Window_OnClosed;

            Title += _materialName;

            DataContext = this;

            Subscribe();
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
            var modeItem = new OpcMonitoredItem(_opc.cl.GetNode("UseWeight_" + _tagName), OpcAttribute.Value);
            modeItem.DataChangeReceived += HandleModeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(modeItem);

            var emptyItem = new OpcMonitoredItem(_opc.cl.GetNode("Scale_isEmpty_" + _tagName), OpcAttribute.Value);
            emptyItem.DataChangeReceived += HandleEmptyChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(emptyItem);

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
        }

        private void HandleModeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            ModeUseWeight = bool.Parse(e.Item.Value.ToString());
        }

        private void HandleEmptyChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            SignEmpty = double.Parse(e.Item.Value.ToString());
        }

        private void BtnUseWeight_Click(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            if (!_opc.cl.ReadBool("UseWeight_"+_tagName, out _err))
                Methods.ButtonClick(btn, BtnUseWeight, "UseWeight_" + _tagName, true, "Весы " + _materialName + ". Использовать вес");
            else
                Methods.ButtonClick(btn, BtnUseWeight, "UseWeight_" + _tagName, false, "Весы " + _materialName + ". Не использовать вес");
        }

        private void BtnEndUnload_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BtnClick = true;
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnEndUnload, "btn_DozingDone_" + _tagName, true, "Весы " + _materialName + ". Закончить выгрузку");
        }

        private void BtnEndUnload_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BtnClick = false;
        }

        private void LblEmpty_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object btn = e.Source;

            Methods.SetParameter(LblEmpty, btn, _opcName, "Весы " + _materialName + ".Признак пустоты", 0, 100, "Scale_isEmpty_" + _tagName, "Real", null, 0, 1);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (StopUpdate != null) StopUpdate();
                KeyDown -= OnKeyDown;
                Closed -= Window_OnClosed;
                Close();
            }
        }

        private void Window_OnClosed(object sender, EventArgs e)
        {
            if (StopUpdate != null) StopUpdate();
            KeyDown -= OnKeyDown;
            Closed -= Window_OnClosed;
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
