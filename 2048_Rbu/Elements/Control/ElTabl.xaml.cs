using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using AS_Library.Link;
using System.Windows.Threading;
using System.ComponentModel;
using _2048_Rbu.Classes;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using System.Runtime.CompilerServices;
using _2048_Rbu.Interfaces;
using AS_Library.Annotations;
using Lib_2048.Classes;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace _2048_Rbu.Elements.Control
{
    /// <summary>
    /// Логика взаимодействия для el_AutoControl.xaml
    /// </summary>
    public partial class ElTabl : IElementsUpdater
    {
        public ViewModelTabl _viewmodelTabl;

        public ElTabl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            _viewmodelTabl = new ViewModelTabl(OpcServer.OpcList.Rbu);
            this.DataContext = _viewmodelTabl;
            
        }
        public void Subscribe()
        {
            _viewmodelTabl.Subscribe();
        }
        public void Unsubscribe()
        {
        }
    }

    public class ViewModelTabl : UserControl, INotifyPropertyChanged
    {
        private OPC_client _opc;
        private OpcServer.OpcList _opcName;
        private long _id;

        public ViewModelTabl(OpcServer.OpcList opcName)
        {
            _opcName = opcName;
        }

        public void Subscribe()
        {
            CreateSubscription();
        }
        public void Unsubscribe()
        {
        }

        private void CreateSubscription()
        {
            _opc = OpcServer.GetInstance().GetOpc(_opcName);
            var idItem = new OpcMonitoredItem(_opc.cl.GetNode("TaskID"), OpcAttribute.Value);
            idItem.DataChangeReceived += HandleIdChanged;
            OpcServer.GetInstance().GetSubscription(_opcName).AddMonitoredItem(idItem);
        }

        private void HandleIdChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            try
            {
                _id = long.Parse(e.Item.Value.ToString());
                GetTable();
            }
            catch (Exception exception)
            {
            }
        }

        private TaskTable GetRecipe(long id)
        {
            using (DbRbu db = new DbRbu())
            {
                var task = db.TaskTables.Where(x => x.Id == id);

                return (TaskTable)task;
            }
        }

        //public Orders order { get; set; }
        private TaskTable _tasktables;
        public TaskTable TaskTables
        {
            get { return _tasktables; }
            set
            {
                _tasktables = value;
                OnPropertyChanged(nameof(TaskTables));
            }
        }
        //public Reports report { get; set; }

        #region Задано
        private float? _cementVol;
        public float? cementVol
        {
            get { return _cementVol; }
            set
            {
                _cementVol = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("cementVol"));
            }
        }
        private float? _waterVol;
        public float? waterVol
        {
            get { return _waterVol; }
            set
            {
                _waterVol = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("waterVol"));
            }
        }
        private float? _inert1Vol;
        public float? inert1Vol
        {
            get { return _inert1Vol; }
            set
            {
                _inert1Vol = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("inert1Vol"));
            }
        }
        private float? _inert2Vol;
        public float? inert2Vol
        {
            get { return _inert2Vol; }
            set
            {
                _inert2Vol = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("inert2Vol"));
            }
        }

        private float? _inert3Vol;
        public float? inert3Vol
        {
            get { return _inert3Vol; }
            set
            {
                _inert3Vol = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("inert3Vol"));
            }
        }

        private float? _additive1Vol;
        public float? additive1Vol
        {
            get { return _additive1Vol; }
            set
            {
                _additive1Vol = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("additive1Vol"));
            }
        }

        private float? _additive2Vol;
        public float? additive2Vol
        {
            get { return _additive2Vol; }
            set
            {
                _additive2Vol = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("additive2Vol"));
            }
        }

        private float? _additive3Vol;
        public float? additive3Vol
        {
            get { return _additive3Vol; }
            set
            {
                _additive3Vol = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("additive3Vol"));
            }
        }
        #endregion
        #region Сделано
        private float? _cementVol_f;
        public float? cementVol_f
        {
            get { return _cementVol_f; }
            set
            {
                _cementVol_f = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("cementVol_f"));
            }
        }
        private float? _waterVol_f;
        public float? waterVol_f
        {
            get { return _waterVol_f; }
            set
            {
                _waterVol_f = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("waterVol_f"));
            }
        }
        private float? _inert1Vol_f;
        public float? inert1Vol_f
        {
            get { return _inert1Vol_f; }
            set
            {
                _inert1Vol_f = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("inert1Vol_f"));
            }
        }
        private float? _inert2Vol_f;
        public float? inert2Vol_f
        {
            get { return _inert2Vol_f; }
            set
            {
                _inert2Vol_f = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("inert2Vol_f"));
            }
        }

        private float? _inert3Vol_f;
        public float? inert3Vol_f
        {
            get { return _inert3Vol_f; }
            set
            {
                _inert3Vol_f = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("inert3Vol_f"));
            }
        }

        private float? _additive1Vol_f;
        public float? additive1Vol_f
        {
            get { return _additive1Vol_f; }
            set
            {
                _additive1Vol_f = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("additive1Vol_f"));
            }
        }

        private float? _additive2Vol_f;
        public float? additive2Vol_f
        {
            get { return _additive2Vol_f; }
            set
            {
                _additive2Vol_f = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("additive2Vol_f"));
            }
        }

        private float? _additive3Vol_f;
        public float? additive3Vol_f
        {
            get { return _additive3Vol_f; }
            set
            {
                _additive3Vol_f = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("additive3Vol_f"));
            }
        }
        #endregion


        public void GetTable()
        {
            if (_id != 0)
            {
                try
                {
                    TaskTables = GetRecipe(_id);
                    //order = database.orders.SingleOrDefault(x => x.orderID == PLC.GetValue("PCAY_01"));
                    //receipt = database.receipts.SingleOrDefault(x => x.receiptID == order.receiptID);
                    //report = database.reports.SingleOrDefault(x => x.orderID == order.orderID && x.cycleNum == PLC.GetValue("PCAY_14"));

                    //cementVol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.cementNorma * order.oneCycleVolume), 1));
                    //waterVol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.waterNorma * order.oneCycleVolume), 1));
                    //inert1Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.inert1Norma * order.oneCycleVolume), 0));
                    //inert2Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.inert2Norma * order.oneCycleVolume), 0));
                    //inert3Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.inert3Norma * order.oneCycleVolume), 0));
                    //additive1Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.additive1Norma * order.oneCycleVolume), 2));
                    //additive2Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.additive2Norma * order.oneCycleVolume), 2));
                    //additive3Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.additive3Norma * order.oneCycleVolume), 2));

                    //cementVol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.cementWeight), 1));
                    //waterVol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.waterWeight), 1));
                    //inert1Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.inert1Weight), 0));
                    //inert2Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.inert2Weight), 0));
                    //inert3Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.inert3Weight), 0));
                    //additive1Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.additive1Weight), 2));
                    //additive2Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.additive2Weight), 2));
                    //additive3Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.additive3Weight), 2));
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText(@"Log\log.txt", DateTime.Now + " - " + ex.Message + "->" + _id);
                }
            }
            else
            {
                //order = null;
                TaskTables = null;
                //report = null;

                cementVol = null;
                waterVol = null;
                inert1Vol = null;
                inert2Vol = null;
                inert3Vol = null;
                additive1Vol = null;
                additive2Vol = null;
                additive3Vol = null;

                cementVol_f = null;
                waterVol_f = null;
                inert1Vol_f = null;
                inert2Vol_f = null;
                inert3Vol_f = null;
                additive1Vol_f = null;
                additive2Vol_f = null;
                additive3Vol_f = null;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}