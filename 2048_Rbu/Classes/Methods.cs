using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using AS_Library.Link;
using AS_Library.Events;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using Lib_2048.Classes;
using System.Windows;
using _2048_Rbu.Classes;
using _2048_Rbu.Windows;
using AS_Library.Events.Classes;

namespace _2048_Rbu.Classes
{
    class Methods
    {
        public static int WordToInt(int val)
        {
            if (val <= 32767)
                return val;
            else
                return (val - 65536);
        }

        public static void ButtonClick(object obj, object obj1, string tag, bool logic, string eventText = null, bool confirm = false)
        {
            if (Static.Link)
            {
                if (Equals(obj, obj1))
                {
                    try
                    {
                        bool err;
                        if (confirm)
                        {
                            var sure = MessageBox.Show("Вы уверены?", "Подтвердите действие", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                            if (sure == MessageBoxResult.OK)
                            {
                                OpcServer.GetInstance().GetOpc(OpcServer.OpcList.Rbu).cl.WriteBool(tag, logic, out err);
                                if (eventText != null)
                                    EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent(eventText, SystemEventType.UserDoing);
                                //if (err)
                                    //MessageBox.Show("Возможно запись не прошла.\nПроверьте OPC-сервер или соответствующий тег", "Предупреждение");
                            }
                        }
                        else
                        {
                            OpcServer.GetInstance().GetOpc(OpcServer.OpcList.Rbu).cl.WriteBool(tag, logic, out err);
                            if (eventText != null)
                                EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent(eventText, SystemEventType.UserDoing);
                            //if (err)
                                //MessageBox.Show("Возможно запись не прошла.\nПроверьте OPC-сервер или соответствующий тег", "Предупреждение");
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Запись не прошла. Повторите ввод", "Ошибка");
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка проверки элемента", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Нет связи с ПЛК", "Ошибка");
            }
        }

        public static void ButtonClick(object obj, object obj1, string tag, double value, string eventText)
        {
            if (Static.Link)
            {
                if (Equals(obj, obj1))
                {
                    try
                    {
                        bool err;
                        OpcServer.GetInstance().GetOpc(OpcServer.OpcList.Rbu).cl.WriteReal(tag, value, out err);
                        EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent(eventText, SystemEventType.UserDoing);
                        //if (err)
                            //MessageBox.Show("Возможно запись не прошла.\nПроверьте OPC-сервер или соответствующий тег", "Предупреждение");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Запись не прошла. Повторите ввод", "Ошибка");
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка проверки элемента", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Нет связи с ПЛК", "Ошибка");
            }
        }

        public static void SetParameter(object lbl, object lbl1, OpcServer.OpcList opcName, string name, double minVal, double maxVal, string parameter, string VariableType, Popup popup, int numStation, int digit)
        {
            if (Static.Link)
            {
                if (Equals(lbl, lbl1))
                {
                    WindowSetParameter window = new WindowSetParameter(opcName, name, minVal, maxVal, parameter, VariableType, popup, numStation, digit);
                    window.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Ошибка проверки элемента", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Нет связи с ПЛК", "Ошибка");
            }
        }

        public static void SetParameter(Label lbl, object lbl1, string name, double minVal, double maxVal, string parameter, int digit)
        {
            if (Static.Link)
            {
                if (Equals(lbl, lbl1))
                {
                    WindowSetParameter window = new WindowSetParameter(lbl, name, minVal, maxVal, parameter, digit);
                    window.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Ошибка проверки элемента", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Нет связи с ПЛК", "Ошибка");
            }
        }
    }
}
