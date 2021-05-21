using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using _2048_Rbu.Windows;
using AS_Library.Classes;
using AS_Library.Link;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Classes.ViewModel
{
    public class CheckLevelSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private OPC_client _opc;
        private OpcServer.OpcList _opcName;

        private int _checkLevelNum;

        public CheckLevelSettingsViewModel(OpcServer.OpcList opcName, int checkLevelNum)
        {
            _opcName = opcName;
            _checkLevelNum = checkLevelNum;

            NameLevel = "Контроль уровня цмента в силосе "+_checkLevelNum;

            Subscribe();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

            var checkTimeItem = new OpcMonitoredItem(_opc.cl.GetNode("CheckLevel_Control_Cement" + _checkLevelNum), OpcAttribute.Value);
            checkTimeItem.DataChangeReceived += HandleCheckTimeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(checkTimeItem);

            var pauseTimeItem = new OpcMonitoredItem(_opc.cl.GetNode("CheckLevel_Pause_Cement" + _checkLevelNum), OpcAttribute.Value);
            pauseTimeItem.DataChangeReceived += HandlePauseTimeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(pauseTimeItem);

            var delayTimeItem = new OpcMonitoredItem(_opc.cl.GetNode("CheckLevel_Delay_Cement"+_checkLevelNum), OpcAttribute.Value);
            delayTimeItem.DataChangeReceived += HandleDelayTimeChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(delayTimeItem);

            OpcServer.GetInstance().GetSubscription(_opcName).ApplyChanges();
        }

        private void HandleCheckTimeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            CheckTime = int.Parse(e.Item.Value.ToString());
        }

        private void HandlePauseTimeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            PauseTime = int.Parse(e.Item.Value.ToString());
        }

        private void HandleDelayTimeChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            DelayTime = int.Parse(e.Item.Value.ToString());
        }

        private string _nameLevel;
        public string NameLevel
        {
            get { return _nameLevel; }
            set
            {
                _nameLevel = value;
                OnPropertyChanged(nameof(NameLevel));
            }
        }

        private int _checkTime;
        public int CheckTime
        {
            get { return _checkTime; }
            set
            {
                _checkTime = value;
                OnPropertyChanged(nameof(CheckTime));
            }
        }

        private int _pauseTime;
        public int PauseTime
        {
            get { return _pauseTime; }
            set
            {
                _pauseTime = value;
                OnPropertyChanged(nameof(PauseTime));
            }
        }

        private int _delayTime;
        public int DelayTime
        {
            get { return _delayTime; }
            set
            {
                _delayTime = value;
                OnPropertyChanged(nameof(DelayTime));
            }
        }

        private RelayCommand _setCheckLevel;
        public RelayCommand SetCheckLevel
        {
            get
            {
                return _setCheckLevel ??= new RelayCommand((o) =>
                {
                    Methods.ButtonClick("btn_CheckLevel_Cement_" + _checkLevelNum, true, NameLevel + ". Проверить уровень");
                });
            }
        }

        private RelayCommand _setCheckTime;
        public RelayCommand SetCheckTime
        {
            get
            {
                return _setCheckTime ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameLevel + ". Длительность контроля, с", 0, 1000, "CheckLevel_Control_Cement" + _checkLevelNum, WindowSetParameter.ValueType.Int16, null, 0);
                });
            }
        }

        private RelayCommand _setPauseTime;
        public RelayCommand SetPauseTime
        {
            get
            {
                return _setPauseTime ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameLevel + ". Длительность паузы, с", 0, 1000, "CheckLevel_Pause_Cement" + _checkLevelNum, WindowSetParameter.ValueType.Int16, null, 0);
                });
            }
        }

        private RelayCommand _setDelayTime;
        public RelayCommand SetDelayTime
        {
            get
            {
                return _setDelayTime ??= new RelayCommand((o) =>
                {
                    Methods.SetParameter(_opcName, NameLevel + ". Задержка применения уровня, с", 0, 1000, "CheckLevel_Delay_Cement" + _checkLevelNum, WindowSetParameter.ValueType.Int16, null, 0);
                });
            }
        }
    }
}
