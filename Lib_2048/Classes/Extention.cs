using System;
using System.Text.RegularExpressions;

namespace Lib_2048.Classes
{
    public static class Extensions
    {
        public static DateTime StringDtParsing(this string stringDt)
        {
            Regex regex = new Regex(@"\d{1}\:\d{2}\:\d{2}");
            Regex regex2 = new Regex(@"\d{2}\:\d{2}\:\d{2}");
            try
            {
                var ind = stringDt.IndexOf(" ", StringComparison.Ordinal);
                var time1 = stringDt.Remove(0, ind);
                var countTime1 = time1.Length;
                var timeString = "";

                if (countTime1 == 12)
                    timeString = regex2.Match(stringDt).ToString();
                else
                    timeString = regex.Match(stringDt).ToString();

                var time = TimeSpan.Parse(timeString);
                if (stringDt.EndsWith("PM"))
                {
                    if (time.Hours != 12)
                        time = time + new TimeSpan(0, 12, 0, 0);
                }
                else
                {
                    if (time.Hours == 12)
                        time = time - new TimeSpan(0, 12, 0, 0);
                }
                var index = stringDt.IndexOf(" ", StringComparison.Ordinal);
                var test3 = stringDt.Remove(index);
                string[] subStrings = test3.Split('/');
                //var dateTime = DateTime.ParseExact(test3, "MM/d/yyyy", new CultureInfo("ru-RU")).Date;
                int year;
                int month;
                int day;
                Int32.TryParse(subStrings[2], out year);
                Int32.TryParse(subStrings[1], out day);
                Int32.TryParse(subStrings[0], out month);
                var dateTime = new DateTime(year, month, day, time.Hours, time.Minutes, time.Seconds);
                return dateTime;
            }
            catch (Exception)
            {
                return DateTime.Now;
            }


        }
    }
}
