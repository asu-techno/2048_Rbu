using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS_Library.Events.Classes;
using AS_Library.Link;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Classes
{
    class AutoEvent
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private SystemEventType _eventType;
        private string _opcTag, _eventText;
        private bool _logic;

        public AutoEvent(OpcServer.OpcList opcName, string opcTag, SystemEventType eventType, string eventText, bool logic)
        {
            _opcName = opcName;
            _opcTag = opcTag;
            _eventType = eventType;
            _eventText = eventText;
            _logic = logic;

            Subscribe();
        }
        
        public void Subscribe()
        {
            CreateSubscription();
        }

        private void CreateSubscription()
        {
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var value = new OpcMonitoredItem(_opc.cl.GetNode(_opcTag), OpcAttribute.Value);
            value.DataChangeReceived += HandleValueChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(value);
        }

        private void HandleValueChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                if (bool.Parse(e.Item.Value.ToString()) == _logic)
                {
                    EventsBase.GetInstance().GetControlEvents(_opcName).AddEvent(_eventText, _eventType);
                }
            }
            catch (Exception exception)
            {
            }
        }
    }
}
