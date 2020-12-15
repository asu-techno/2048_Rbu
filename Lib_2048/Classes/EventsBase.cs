using System.Collections.Generic;
using AS_Library.Events.Classes;
using ServiceLib.Classes;

namespace Lib_2048.Classes
{
    public class EventsBase
    {
        private static EventsBase _instance;

        readonly Dictionary<OpcServer.OpcList, ControlEventsEntity> _eventsDict = new Dictionary<OpcServer.OpcList, ControlEventsEntity>();

        public string GetConnectionStringEvents(OpcServer.OpcList opcName)
        {
            if (ServiceData.GetInstance().GetSqlName() == "PostgreSQL")
            {
                return "server=" + ServiceData.GetInstance().GetSqlAddress() + ";user id=AS_Library;password=asuasu123;database=" +
                       OpcServer.GetInstance().GetObjectData(opcName).EventBaseName;

            }

            return "Data Source=" + ServiceData.GetInstance().GetSqlAddress() +
                   "\\" + ServiceData.GetInstance().GetSqlName() + ";Initial Catalog=" +
                   OpcServer.GetInstance().GetObjectData(opcName).EventBaseName +
                   "; User ID=AS_Library;Password=asuasu123;";
        }

        public void CreateControlEvents(OpcServer.OpcList objectName)
        {
            if (!_eventsDict.ContainsKey(objectName))
            {
                var postgresql = ServiceData.GetInstance().GetSqlName() == "PostgreSQL";
                _eventsDict.Add(objectName,
               new ControlEventsEntity(OpcServer.GetInstance().GetOpc(objectName), GetConnectionStringEvents(objectName), postgresql, true, true, false, false));
            }
        }

        public ControlEventsEntity GetControlEvents(OpcServer.OpcList objectName)
        {
            if (_eventsDict.ContainsKey(objectName))
            {
                return _eventsDict[objectName];
            }
            return null;
        }

        public static EventsBase GetInstance()
        {
            if (_instance == null)
                _instance = new EventsBase();
            return _instance;
        }
    }
}
