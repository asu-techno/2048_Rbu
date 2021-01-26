using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AS_Library.Link;
using _2048_Rbu.Classes;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using AS_Library.Events;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Control
{
    /// <summary>
    /// Interaction logic for el_SimpleValve.xaml
    /// </summary>
    public partial class ElEvent : INotifyPropertyChanged, IElementsUpdater
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private bool _err;

        public enum Type { Warning, Alarm };

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

        private Type _typeWindow;
        public Type TypeWindow
        {
            get { return _typeWindow; }
            set
            {
                if (value == Type.Warning)
                    ElGrid.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFE100"));
                if (value == Type.Alarm)
                    ElGrid.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF0000"));

                _typeWindow = value;
            }
        }

        public string VisWindowTag { get; set; }
        public string EventsNumTag { get; set; }
        public string CmdAckTag { get; set; }

        #endregion

        public ElEvent()
        {
            InitializeComponent();
        }

        public void Initialize(OpcServer.OpcList opcName)
        {
            _opcName = opcName;

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
            var visWindowItem = new OpcMonitoredItem(_opc.cl.GetNode(VisWindowTag), OpcAttribute.Value);
            visWindowItem.DataChangeReceived += HandleVisWindowChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(visWindowItem);

            var eventsNumValueItem = new OpcMonitoredItem(_opc.cl.GetNode(EventsNumTag), OpcAttribute.Value);
            eventsNumValueItem.DataChangeReceived += HandleEventsNumValueChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(eventsNumValueItem);
        }

        private void HandleVisWindowChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                Vis = bool.Parse(e.Item.Value.ToString());
            }
            catch (Exception exception)
            {
            }
        }

        private void HandleEventsNumValueChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                GetValue(int.Parse(e.Item.Value.ToString()));
            }
            catch (Exception exception)
            {
            }
        }

        public void GetValue(int numValue)
        {
            switch (_typeWindow)
            {
                #region Alarm
                case Type.Alarm:
                    switch (numValue)
                    {
                        case 1: Value = "Насос M-15 - авария ОС"; break;
                        case 2: Value = "Насос M-15 - внешняя авария"; break;
                        case 3: Value = "Насос M-15 - авария ДКС"; break;
                        case 4: Value = "Насос M-16 - авария ОС"; break;
                        case 5: Value = "Насос M-16 - внешняя авария"; break;
                        case 6: Value = "Насос M-16 - авария ДКС"; break;
                        case 7: Value = "Насос M-17 - авария ОС"; break;
                        case 8: Value = "Насос M-17 - внешняя авария"; break;
                        case 9: Value = "Насос M-17 - авария ДКС"; break;
                        case 10: Value = "Бетоносмеситель M-18 - авария ОС"; break;
                        case 11: Value = "Бетоносмеситель M-18 - внешняя авария"; break;
                        case 12: Value = "Бетоносмеситель M-18 - авария системы смазки"; break;
                        case 14: Value = "Конвейер M-9 - авария ОС"; break;
                        case 15: Value = "Конвейер M-9 - внешняя авария"; break;
                        case 16: Value = "Конвейер M-9 - авария ДКС"; break;
                        case 17: Value = "Конвейер M-1 - авария ОС"; break;
                        case 18: Value = "Конвейер M-1 - внешняя авария"; break;
                        case 19: Value = "Конвейер M-1 - авария ДКС"; break;
                        case 20: Value = "Шнек M-11 - авария ОС"; break;
                        case 21: Value = "Шнек M-11 - внешняя авария"; break;
                        case 22: Value = "Шнек M-11 - авария ДКС"; break;
                        case 23: Value = "Шнек M-12 - авария ОС"; break;
                        case 24: Value = "Шнек M-12 - внешняя авария"; break;
                        case 25: Value = "Шнек M-12 - авария ДКС"; break;
                        case 26: Value = "Вибратор M-13 - авария ОС"; break;
                        case 27: Value = "Вибратор M-13 - внешняя авария"; break;
                        case 28: Value = "Вибратор M-13 - авария ДКС"; break;
                        case 30: Value = "Вибратор M-14 - авария ОС"; break;
                        case 31: Value = "Вибратор M-14 - внешняя авария"; break;
                        case 32: Value = "Вибратор M-14 - авария ДКС"; break;
                        case 33: Value = "Вибратор M-10 - авария ОС"; break;
                        case 34: Value = "Вибратор M-10 - внешняя авария"; break;
                        case 35: Value = "Вибратор M-10 - авария ДКС"; break;
                        case 36: Value = "Вибратор M-2 - авария ОС"; break;
                        case 37: Value = "Вибратор M-2 - внешняя авария"; break;
                        case 38: Value = "Вибратор M-2 - авария ДКС"; break;
                        case 39: Value = "Вибратор M-3 - авария ОС"; break;
                        case 40: Value = "Вибратор M-3 - внешняя авария"; break;
                        case 41: Value = "Вибратор M-3 - авария ДКС"; break;
                        case 42: Value = "Вибратор M-4 - авария ОС"; break;
                        case 43: Value = "Вибратор M-4 - внешняя авария"; break;
                        case 44: Value = "Вибратор M-4 - авария ДКС"; break;
                        case 45: Value = "Вибратор M-5 - авария ОС"; break;
                        case 46: Value = "Вибратор M-5 - внешняя авария"; break;
                        case 47: Value = "Вибратор M-5 - авария ДКС"; break;
                        case 48: Value = "Вибратор M-6 - авария ОС"; break;
                        case 49: Value = "Вибратор M-6 - внешняя авария"; break;
                        case 50: Value = "Вибратор M-6 - авария ДКС"; break;
                        case 51: Value = "Вибратор M-7 - авария ОС"; break;
                        case 52: Value = "Вибратор M-7 - внешняя авария"; break;
                        case 53: Value = "Вибратор M-7 - авария ДКС"; break;
                        case 54: Value = "Вибратор M-12-1 - авария ОС"; break;
                        case 55: Value = "Вибратор M-12-1 - внешняя авария"; break;
                        case 56: Value = "Вибратор M-12-1 - авария ДКС"; break;
                        case 57: Value = "Вибратор M-12-2 - авария ОС"; break;
                        case 58: Value = "Вибратор M-12-2 - внешняя авария"; break;
                        case 59: Value = "Вибратор M-12-2 - авария ДКС"; break;
                        case 60: Value = "Задвижка V-1 - авария ОС-Open"; break;
                        case 61: Value = "Задвижка V-1 - авария ОС-Close"; break;
                        case 62: Value = "Задвижка V-1 - авария датчиков"; break;
                        case 63: Value = "Задвижка V-1 - внешняя авария"; break;
                        case 64: Value = "Задвижка V-2 - авария открытия"; break;
                        case 65: Value = "Задвижка V-2 - авария закрытия"; break;
                        case 66: Value = "Задвижка V-3 - авария открытия"; break;
                        case 67: Value = "Задвижка V-3 - авария закрытия"; break;
                        case 68: Value = "Задвижка V-4 - авария открытия"; break;
                        case 69: Value = "Задвижка V-4 - авария закрытия"; break;
                        case 70: Value = "Задвижка V-5 - авария открытия"; break;
                        case 71: Value = "Задвижка V-5 - авария закрытия"; break;
                        case 72: Value = "Задвижка V-6 - авария открытия"; break;
                        case 73: Value = "Задвижка V-6 - авария закрытия"; break;
                        case 74: Value = "Задвижка V-7 - авария открытия"; break;
                        case 75: Value = "Задвижка V-7 - авария закрытия"; break;
                        case 76: Value = "Задвижка V-9-1 - авария открытия"; break;
                        case 77: Value = "Задвижка V-9-1 - авария закрытия"; break;
                        case 78: Value = "Задвижка V-9-2 - авария открытия"; break;
                        case 79: Value = "Задвижка V-9-2 - авария закрытия"; break;
                        case 80: Value = "Задвижка V-10-1 - авария открытия"; break;
                        case 81: Value = "Задвижка V-10-1 - авария закрытия"; break;
                        case 82: Value = "Задвижка V-10-2 - авария открытия"; break;
                        case 83: Value = "Задвижка V-10-2 - авария закрытия"; break;
                        case 84: Value = "Задвижка V-11-1 - авария открытия"; break;
                        case 85: Value = "Задвижка V-11-1 - авария закрытия"; break;
                        case 86: Value = "Задвижка V-11-2 - авария открытия"; break;
                        case 87: Value = "Задвижка V-11-2 - авария закрытия"; break;
                        case 88: Value = "Задвижка V-12-1 - авария открытия"; break;
                        case 89: Value = "Задвижка V-12-1 - авария закрытия"; break;
                        case 90: Value = "Задвижка V-12-2 - авария открытия"; break;
                        case 91: Value = "Задвижка V-12-2 - авария закрытия"; break;
                        case 92: Value = "Насос M-19 - Вода - авария ОС"; break;
                        case 93: Value = "Насос M-19 - Вода - внешняя авария"; break;
                        case 94: Value = "Насос M-19 - Вода - авария ДКС"; break;
                        case 95: Value = "Аэратор силоса цемента 1 - авария открытия"; break;
                        case 96: Value = "Аэратор силоса цемента 1 - авария закрытия"; break;
                        case 97: Value = "Аэратор силоса цемента 2 - авария открытия"; break;
                        case 98: Value = "Аэратор силоса цемента 2 - авария закрытия"; break;
                        case 99: Value = "Насос гидростанции -не сработал пускатель"; break;
                        case 102: Value = "Нажата стоповая кнопка в шкафу управления контроллера"; break;
                        case 103: Value = "Нажата стоповая кнопка в шкафу управления инертными фракциями"; break;
                        case 104: Value = "Нажата стоповая кнопка в шкафу управления дозированием"; break;
                        case 105: Value = "Нажата стоповая кнопка в шкафу управления цементом"; break;
                        case 106: Value = "Нажата стоповая кнопка в шкафу управления жидкими компонентами"; break;
                        case 107: Value = "Нажата стоповая кнопка в шкафу управления бетоносмесителем"; break;
                        case 108: Value = "Общий стоп"; break;
                        case 109: Value = "Нет связи с весовым индикатором цемента"; break;
                        case 110: Value = "Нет связи с весовым индикатором воды"; break;
                        case 111: Value = "Нет связи с весовым индикатором инертных фракций"; break;
                        case 112: Value = "Нет связи с весовым индикатором химических добавок"; break;
                        default:
                            Value = "- - - - - - - -";
                            break;
                    }
                    break;
                #endregion
                #region Warning
                case Type.Warning:
                    switch (numValue)
                    {
                        case 1: Value = "Бетоносмеситель M-18 - нет давления в системе смазки"; break;
                        case 2: Value = "Крышка открыта"; break;
                        case 3: Value = "Бетоносмеситель M-18 - автоматический режим"; break;
                        case 4: Value = "Конвейер M-9 - автоматический"; break;
                        case 5: Value = "Бетоносмеситель M-18 - ручной режим"; break;
                        case 6: Value = "Конвейер M-9 - ручной режим"; break;
                        case 7: Value = "Низкое давление в гидравлической магистрали"; break;
                        case 8: Value = "Нет связи с программой архивации"; break;
                        default:
                            Value = "- - - - - - - -";
                            break;
                    }
                    break;
                    #endregion
            }
        }

        private void BtnAck_OnClick(object sender, RoutedEventArgs e)
        {
            object btn = e.Source;
            Methods.ButtonClick(btn, BtnAck, CmdAckTag, true, TypeWindow == Type.Warning ? "Квитирование предупреждения " + "\"" + Value + "\"" : "Квитирование аварии " + "\"" + Value + "\"");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
