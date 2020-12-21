using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Classes;
using Newtonsoft.Json;
using NLog;
using Opc.UaFx;

namespace _2048_Rbu.Classes
{
    public static class OpcHelper
    {
        public enum TagNames
        {
            CurrentTaskId,
            MaterialSet,
            MixingTime,
            PercentOpenGate,
            TimeFullUnload,
            TimePartialUnload,
            BatchesAmount,
            MixerVolume
        }

        public static Dictionary<TagNames, string> TagNamesDictionary = GetTagNamesDictionary();

        private static Dictionary<TagNames, string> GetTagNamesDictionary()
        {
            try
            {
                using StreamReader r = new StreamReader("Data\\TagNames.json");
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<Dictionary<TagNames, string>>(json);
            }
            catch (Exception ex)
            {
                Service.GetInstance().GetLogger().Error(ex);
                MessageBox.Show("Проблемы с парсингом TagNames.json.");
                return new Dictionary<TagNames, string>();
            }
        }

        public static object GetParameterValue(OpcValue opcValue, ApiOpcParameter parameter)
        {
            if (opcValue != null)
            {
                try
                {
                    var value = opcValue.ToString();
                    switch (parameter.Type)
                    {
                        case TagTypes.Decimal:
                            return Math.Round(Convert.ToDecimal(value), parameter.CharactersAmount);
                        case TagTypes.Int16:
                            return Convert.ToInt16(value);
                        case TagTypes.Int32:
                            return Convert.ToInt32(value);
                        case TagTypes.Long:
                            return Convert.ToInt64(value);
                        case TagTypes.Float:
                            return Convert.ToSingle(Math.Round(Convert.ToSingle(value), parameter.CharactersAmount));
                        default:
                            return opcValue.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Service.GetInstance().GetLogger().Error(ex);
                    return null;
                }
            }

            return null;
        }

        public static OpcWriteNode GetOpcWriteNode(string value, ApiOpcParameter parameter)
        {
            switch (parameter.Type)
            {
                case TagTypes.Decimal:
                    return new OpcWriteNode(parameter.Tag.ToNode(), OpcAttribute.Value, Convert.ToDecimal(value));
                case TagTypes.Int16:
                    return new OpcWriteNode(parameter.Tag.ToNode(), OpcAttribute.Value, Convert.ToInt16(value));
                case TagTypes.Int32:
                    return new OpcWriteNode(parameter.Tag.ToNode(), OpcAttribute.Value, Convert.ToInt32(value));
                case TagTypes.Long:
                    return new OpcWriteNode(parameter.Tag.ToNode(), OpcAttribute.Value, Convert.ToInt64(value));
                case TagTypes.Float:
                    return new OpcWriteNode(parameter.Tag.ToNode(), OpcAttribute.Value, Convert.ToSingle(value));
                default:
                    return new OpcWriteNode(parameter.Tag.ToNode(), OpcAttribute.Value, value);
            }
        }

        public static string GetTagName(TagNames tagName)
        {
            return TagNamesDictionary.ContainsKey(tagName) ? TagNamesDictionary[tagName] : "";
        }

        public static OpcValue ReadTag(NewOpcServer.OpcList obj, string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                var result = NewOpcServer.GetInstance().GetOpc(obj)?.ReadNode(tag.ToNode(), OpcAttribute.Value);
                return result;
            }

            return null;
        }

        public static bool WriteTags(NewOpcServer.OpcList obj, Dictionary<ApiOpcParameter, string> parameters)
        {
            var opcWriteNodeList = new List<OpcWriteNode>();
            foreach (var parameter in parameters)
            {
                var opcWriteNode = GetOpcWriteNode(parameter.Value, parameter.Key);
                opcWriteNodeList.Add(opcWriteNode);
            }

            var commands = opcWriteNodeList.ToArray();
            var results = NewOpcServer.GetInstance().GetOpc(obj)?.WriteNodes(commands);
            var goodResult = (results ?? throw new InvalidOperationException()).Count(x => x.Code == 0);
            return goodResult == parameters.Count;
        }
        public static string ToNode(this string tag)
        {
            return $"ns=2;s=Siem_Rbu.Rbu.{tag}";
        }

        public static DateTime StringDtParsing(this string stringDt)
        {
            Regex regex = new Regex(@"\d{1}\:\d{2}\:\d{2}");
            Regex regex2 = new Regex(@"\d{2}\:\d{2}\:\d{2}");
            try
            {
                var ind = stringDt.IndexOf(" ", StringComparison.Ordinal);
                var time1 = stringDt.Remove(0, ind);
                var countTime1 = time1.Length;
                var timeString = countTime1 == 12 ? regex2.Match(stringDt).ToString() : regex.Match(stringDt).ToString();

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
                var subStrings = test3.Split('/');
                int.TryParse(subStrings[2], out var year);
                int.TryParse(subStrings[1], out var day);
                int.TryParse(subStrings[0], out var month);
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
