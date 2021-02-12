using System;
using System.Reflection;

namespace _2048_Rbu.Classes
{
    public class Release
    {
        private Version _myVersion;
        private DateTime _buildDateTime;
        private static Release _instance;

        private Release()
        {
            _myVersion = Assembly.GetExecutingAssembly().GetName().Version;
            _buildDateTime = new DateTime(2000, 1, 1).AddDays(_myVersion.Build).AddSeconds(_myVersion.Revision * 2);
        }

        public DateTime GetBuildDateTime()
        {
            return _buildDateTime;
        }

        public string GetVersion()
        {

            return "v." + _myVersion.Major + "." + _myVersion.Minor + "." + _buildDateTime.Day.ToString("D2") + _buildDateTime.Month.ToString("D2") + "." + _buildDateTime.Hour.ToString("D2") + _buildDateTime.Minute.ToString("D2");
        }

        public string GetCopyright()
        {
            return "© ООО «‎АСУ-Техно»‎, " + _buildDateTime.Year;
        }

        public static Release GetInstance()
        {
            if (_instance == null)
                _instance = new Release();
            return _instance;
        }
    }
}
