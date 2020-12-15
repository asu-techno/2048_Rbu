using ServiceLib.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using AS_Library.Link;
using Excel;
using Opc.UaFx.Client;

namespace Lib_2048.Classes
{
    public class OpcServer
    {
        private static OpcServer _instance;
        private static string _tableName;
        private readonly Dictionary<OpcList, ObjectData> _objectDict = new Dictionary<OpcList, ObjectData>();
        private readonly Dictionary<OpcList, OPC_client> _opcDict = new Dictionary<OpcList, OPC_client>();
        private readonly Dictionary<OpcList, OpcSubscription> _opcSubscriptions = new Dictionary<OpcList, OpcSubscription>();
        private static int _err1;
        private static readonly int[] ErrArray1 = new int[20];
        private static readonly int[] ErrArray2 = new int[20];


        public enum OpcList
        {
            Rbu,
        }

        private OpcServer()
        {
            if (_tableName == null)
                throw new ArgumentException("KepServer is not inited.");
            ExcelTable_Parsing(_tableName);
        }

        private void ExcelTable_Parsing(string pathFile)
        {
            FileStream stream = File.Open(pathFile, FileMode.Open, FileAccess.Read);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            excelReader.IsFirstRowAsColumnNames = false;
            DataSet dataSet = excelReader.AsDataSet();

            _objectDict.Add(OpcList.Rbu, new ObjectData
            {
                DataBaseName = dataSet.Tables[0].Rows[1].ItemArray[3].ToString(),
                EventBaseName = dataSet.Tables[0].Rows[2].ItemArray[3].ToString(),
                WaterTableName = dataSet.Tables[0].Rows[3].ItemArray[3].ToString(),
                AirTableName = dataSet.Tables[0].Rows[4].ItemArray[3].ToString(),
                SqlTableName = dataSet.Tables[0].Rows[5].ItemArray[3].ToString(),
                ElectricityTableName = dataSet.Tables[0].Rows[6].ItemArray[3].ToString()
            });
        }

        private string ErrCodeParse(int errCode)
        {
            switch (errCode)
            {
                case 0:
                    return "ОК";
                case 1:
                    return "Файл инициализации не найден";
                case 2:
                    return "Файл инициализации заполнен неверно";
                case 3:
                    return "Ошибка доступа к серверу";
                case 4:
                    return "Ошибка связи с сервером";
                case 5:
                    return "Адреса переменных для чтения не корректны";
                case 6:
                    return "Адреса переменных для записи не корректны";
                case 7:
                    return "Переменная с таким именем не существует";
                case 8:
                    return "Переменная с таким адресом не существует";
                case 9:
                    return "Переменная с таким именем имеет другой тип";
                case 10:
                    return "Файл инициализации заполнен неверно - имя или адрес повторяются";
                case 11:
                    return "Xml файл заполнен неверно - отсутcтвуют дочерние элементы узлов Item";
                case 12:
                    return "Xml файл инициализации заполнен неверно - отсутствуют некоторые дочерние элементы узлов Item";
                case 13:
                    return "Xml файл заполнен неверно - отсутcтвуют узлы Item";
                case 14:
                    return "Xml файл инициализации заполнен неверно - TypeRW не определен";
                default:
                    return "";
            }
        }

        public void ConnectOpc(OpcList obj)
        {
            if (_opcDict.ContainsKey(obj))
            {
                _opcDict[obj].cl.Connect(ServiceData.GetInstance().GetOpcAddress(), ServiceData.GetInstance().GetOpcName(), 100);
            }
        }
        public OpcSubscription GetSubscription(OpcList obj)
        {
            if (_opcDict.ContainsKey(obj))
            {
                if (!_opcSubscriptions.ContainsKey(obj))
                {
                    var opcSubscription = _opcDict[obj].cl.SubscribeNodes();
                    opcSubscription.PublishingInterval = 2000;
                    _opcSubscriptions.Add(obj, opcSubscription);
                    return opcSubscription;
                }

                return _opcSubscriptions[obj];
            }
            return null;
        }
        public OPC_client CreateOpc(OpcList opcName)
        {
            var postgresql = ServiceData.GetInstance().GetSqlName() == "PostgreSQL";

            if (GetOpc(opcName) != null)
            {
                _opcDict[opcName].cl.Disconnect();
                _opcDict[opcName] = null;
            }

            var opc = new OPC_client(ServiceData.GetInstance().GetOpcTablesBase(), GetObjectData(opcName).SqlTableName, postgresql, true, out _err1);
            if (_err1 == 0)
            {
                var err2 = opc.cl.Connect(ServiceData.GetInstance().GetOpcAddress(), ServiceData.GetInstance().GetOpcName(), 100);
                if (err2 != 0)
                {
                    //MessageBox.Show("Ошибка №" + err2);
                    opc = null;
                }
                else
                {
                    if (!_opcDict.ContainsKey(opcName))
                    {
                        _opcDict.Add(opcName, opc);
                    }
                    else
                    {
                        _opcDict[opcName] = opc;
                    }
                }
            }
            else
            {
                //MessageBox.Show("Ошибка №" + _err1 + " - " + opc.cl.ErrTxt);
                opc = null;
            }

            return opc;
        }
        public OPC_client InitOpc(OpcList obj, bool serverInit = false)
        {
            bool postgresql = ServiceData.GetInstance().GetSqlName() == "PostgreSQL";
            var opc = new OPC_client(ServiceData.GetInstance().GetOpcTablesBase(), GetObjectData(obj).SqlTableName, postgresql, serverInit, out _err1);
            if (!_opcDict.ContainsKey(obj))
            {
                _opcDict.Add(obj, opc);
            }
            else
            {
                _opcDict[obj] = opc;
            }
            return opc;
        }
        public OPC_client InitOpc(OpcList obj, string serverAddress)
        {
            if (!_opcDict.ContainsKey(obj))
            {
                var opc = new OPC_client(serverAddress, ServiceData.GetInstance().GetOpcTablesBase(), GetObjectData(obj).SqlTableName);
                _opcDict.Add(obj, opc);
                return opc;
            }

            return _opcDict[obj];
        }
        public OPC_client InitOpc(OpcList obj, bool sim, bool serverInit)
        {
            bool postgresql = ServiceData.GetInstance().GetSqlName() == "PostgreSQL";
            string simTablePath = "server=dalmioserver;user id=AS_Library;password=asuasu123;database=DbOpcTables";
            var opc = new OPC_client(simTablePath, GetObjectData(obj).SqlTableName, postgresql, serverInit, out _err1);
            if (!_opcDict.ContainsKey(obj))
            {
                _opcDict.Add(obj, opc);
            }
            else
            {
                _opcDict[obj] = opc;
            }
            return opc;
        }
        public OPC_client GetOpc(OpcList obj)
        {
            if (_opcDict.ContainsKey(obj))
            {
                return _opcDict[obj];
            }
            return null;
        }
        public Dictionary<OpcList, OPC_client> GetOpcDict()
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
        public string GetConnectionStringData(OpcList opcName)
        {
            if (ServiceData.GetInstance().GetSqlName() == "PostgreSQL")
            {
                return "server=" + ServiceData.GetInstance().GetSqlAddress() + ";user id=AS_Library;password=asuasu123;database=" +
                       GetObjectData(opcName).DataBaseName;
            }

            return "Data Source=" + ServiceData.GetInstance().GetSqlAddress() +
                   "\\" + ServiceData.GetInstance().GetSqlName() + ";Initial Catalog=" +
                   GetObjectData(opcName).DataBaseName +
                   "; User ID=AS_Library;Password=asuasu123;";
        }
        public static OpcServer GetInstance()
        {
            if (_instance == null)
                _instance = new OpcServer();
            return _instance;
        }
        public static void Init(string tableName)
        {
            if (_tableName == null)
            {
                _tableName = tableName;
            }
            else
            {
                throw new ArgumentException("KepServer is already inited.");
            }
        }

    }
    public class ObjectData
    {
        public string TableName;
        public string DataBaseName;
        public string WaterTableName;
        public string ElectricityTableName;
        public string EventBaseName;
        public string AirTableName;
        public string SqlTableName;
    }
}
