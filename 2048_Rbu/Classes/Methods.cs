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
using _2048_Rbu.Classes;
using System.Windows;
using _2048_Rbu.Windows;
using AS_Library.Events.Classes;

namespace _2048_Rbu.Classes
{
    public class Methods
    {
        public static void ButtonClick(object obj, object obj1, string tag, bool logic, string eventText = null)
        {
            if (Static.Link)
            {
                if (Equals(obj, obj1))
                {
                    try
                    {
                        bool err;
                        OpcServer.GetInstance().GetOpc(OpcServer.OpcList.Rbu).cl.WriteBool(tag, logic, out err);
                        if (eventText != null)
                            EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent(eventText, SystemEventType.UserDoing);
                        if (err)
                                MessageBox.Show("Возможно запись не прошла.\nПроверьте OPC-сервер или соответствующий тег",
                                    "Предупреждение");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Запись не прошла. Повторите ввод\n" + ex.Message, "Ошибка");
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

        public static void ButtonClick(string tag, bool logic, string eventText = null)
        {
            if (Static.Link)
            {
                try
                {
                    bool err;
                    OpcServer.GetInstance().GetOpc(OpcServer.OpcList.Rbu).cl.WriteBool(tag, logic, out err);
                    if (eventText != null)
                        EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent(eventText, SystemEventType.UserDoing);
                    if (err)
                        MessageBox.Show("Возможно запись не прошла.\nПроверьте OPC-сервер или соответствующий тег",
                            "Предупреждение");
                }
                catch (Exception)
                {
                    MessageBox.Show("Запись не прошла. Повторите ввод", "Ошибка");
                }
            }
            else

            {
                MessageBox.Show("Нет связи с ПЛК", "Ошибка");
            }
        }

        //public static void ButtonClick(object obj, object obj1, string tag, float[] val)
        //{
        //    if (Static.Link)
        //    {
        //        if (Equals(obj, obj1))
        //        {
        //            try
        //            {
        //                bool err;
        //                OpcServer.GetInstance().GetOpc(OpcServer.OpcList.Rbu).cl.WriteArray(tag, val, out err);
        //                if (err)
        //                    MessageBox.Show("Возможно запись не прошла.\nПроверьте OPC-сервер или соответствующий тег",
        //                        "Предупреждение");
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("Запись не прошла. Повторите ввод\n" + ex.Message, "Ошибка");
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Ошибка проверки элемента", "Ошибка");
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Нет связи с ПЛК", "Ошибка");
        //    }
        //}

        public static void SetParameter(object lbl, object lbl1, OpcServer.OpcList opcName, string parameterName, double minValue, double maxValue, string opcTag, WindowSetParameter.ValueType valueType, Popup popup = null, int digit = 0, double? firstPrompt = null, double? secondPrompt = null, double? thirdPrompt = null, double? fourthPrompt = null, double? stepFeed = null)
        {
            if (Static.Link)
            {
                if (Equals(lbl, lbl1))
                {
                    WindowSetParameter window = new WindowSetParameter(opcName, parameterName, minValue, maxValue, opcTag, valueType, popup, digit, firstPrompt, secondPrompt, thirdPrompt, fourthPrompt, stepFeed);
                    window.Show();
                    window.SelText();
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

        public static void SetParameter(OpcServer.OpcList opcName, string parameterName, double minValue, double maxValue, string opcTag, WindowSetParameter.ValueType valueType, Popup popup = null, int digit = 0, double? firstPrompt = null, double? secondPrompt = null, double? thirdPrompt = null, double? fourthPrompt = null, double? stepFeed = null)
        {
            if (Static.Link)
            {
                WindowSetParameter window = new WindowSetParameter(opcName, parameterName, minValue, maxValue, opcTag, valueType, popup, digit, firstPrompt, secondPrompt, thirdPrompt, fourthPrompt, stepFeed);
                window.Show();
                window.SelText();
            }
            else
            {
                MessageBox.Show("Нет связи с ПЛК", "Ошибка");
            }
        }
    }
}
