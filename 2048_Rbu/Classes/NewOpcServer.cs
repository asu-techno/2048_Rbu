using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using NLog;
using Opc.UaFx.Client;

namespace _2048_Rbu.Classes
{
    public class NewOpcServer
    {
        private static NewOpcServer _instance;
        private readonly Dictionary<OpcList, ObjectData> _objectDict = new Dictionary<OpcList, ObjectData>();
        private readonly Dictionary<OpcList, OpcClient> _opcDict = new Dictionary<OpcList, OpcClient>();
        private readonly Dictionary<OpcList, OpcSubscription> _opcSubscriptions = new Dictionary<OpcList, OpcSubscription>();
        private static Logger Logger { get; set; }
        public enum OpcList
        {
            Rbu
        }

        private NewOpcServer()
        {
            if (Logger == null)
                throw new ArgumentException("OpcServer is not inited.");
        }

        public void ConnectOpc(OpcList obj)
        {
            if (_opcDict.ContainsKey(obj))
            {
                _opcDict[obj].Connect();
            }
        }
        public OpcSubscription GetSubscription(OpcList obj)
        {
            if (_opcDict.ContainsKey(obj))
            {
                if (!_opcSubscriptions.ContainsKey(obj))
                {
                    var opcSubscription = _opcDict[obj].SubscribeNodes();
                    opcSubscription.PublishingInterval = 500;
                    _opcSubscriptions.Add(obj, opcSubscription);
                    return opcSubscription;
                }

                return _opcSubscriptions[obj];
            }
            return null;
        }
        public OpcClient InitOpc(OpcList obj)
        {
            if (!_opcDict.ContainsKey(obj))
            {
                var opc = new OpcClient(Service.GetInstance().GetOpcDict()["OpcServerAddress"]);
                _opcDict.Add(obj, opc);
                return opc;
            }

            return _opcDict[obj];
        }
        public OpcClient GetOpc(OpcList obj)
        {
            if (_opcDict.ContainsKey(obj))
            {
                return _opcDict[obj];
            }
            return null;
        }
        public Dictionary<OpcList, OpcClient> GetOpcDict()
        {
            return _opcDict;
        }
        public Dictionary<OpcList, ObjectData> GetObjects()
        {
            return _objectDict;
        }
        public ObjectData GetObjectData(OpcList objectName)
        {
            if (_objectDict.ContainsKey(objectName))
            {
                return _objectDict[objectName];
            }
            return null;
        }
        public static NewOpcServer GetInstance()
        {
            if (_instance == null)
                _instance = new NewOpcServer();
            return _instance;
        }
        public static void Init(Logger logger)
        {
            if (Logger == null)
            {
                Logger = logger;
            }
            else
            {
                throw new ArgumentException("OpcServer is already inited.");
            }
        }
    }
}
