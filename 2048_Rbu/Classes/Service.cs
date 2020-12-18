using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using NLog;

namespace _2048_Rbu.Classes
{
    public class Service
    {
        private static Service _instance;
        private static Dictionary<string, string> _serviceDict = new Dictionary<string, string>();
        private static Logger Logger { get; set; }

        private Service()
        {
            Logger = LogManager.GetCurrentClassLogger();
            Parsing();
        }

        private void Parsing()
        {
            try
            {
                using StreamReader r = new StreamReader("Data\\Service.json");
                string json = r.ReadToEnd();
                _serviceDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessageBox.Show("Проблемы с парсингом Service.json.");
            }
        }

        public Dictionary<string, string> GetOpcDict()
        {
            return _serviceDict;
        }
        public Logger GetLogger()
        {
            return Logger;
        }

        public static Service GetInstance()
        {
            if (_instance == null)
                _instance = new Service();
            return _instance;
        }
    }
}
