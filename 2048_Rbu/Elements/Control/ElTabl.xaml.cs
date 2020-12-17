using System;
using System.Windows.Controls;
using AS_Library.Link;
using System.Windows.Threading;
using System.ComponentModel;

namespace _2048_Rbu.Elements.Control
{
    /// <summary>
    /// Логика взаимодействия для el_AutoControl.xaml
    /// </summary>
    public partial class ElTabl : UserControl
    {
        public DispatcherTimer Timer;
        public ModbusMasterDevice PLC;
        public viewmodel_Tabl viewmodelTabl;

        public ElTabl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {

            viewmodelTabl = new viewmodel_Tabl();
            this.DataContext = viewmodelTabl;

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            //if (database != null)
            //{
            //    viewmodelTabl.Update();
            //    this.DataContext = null;
            //    this.DataContext = viewmodelTabl;
            //}
        }

    }

    public class viewmodel_Tabl : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //public Orders order { get; set; }
        //public Receipts receipt { get; set; }
        //public Reports report { get; set; }

        //#region Задано
        //private float? _cementVol;
        //public float? cementVol
        //{
        //    get { return _cementVol; }
        //    set
        //    {
        //        _cementVol = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("cementVol"));
        //    }
        //}
        //private float? _waterVol;
        //public float? waterVol
        //{
        //    get { return _waterVol; }
        //    set
        //    {
        //        _waterVol = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("waterVol"));
        //    }
        //}
        //private float? _inert1Vol;
        //public float? inert1Vol
        //{
        //    get { return _inert1Vol; }
        //    set
        //    {
        //        _inert1Vol = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("inert1Vol"));
        //    }
        //}
        //private float? _inert2Vol;
        //public float? inert2Vol
        //{
        //    get { return _inert2Vol; }
        //    set
        //    {
        //        _inert2Vol = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("inert2Vol"));
        //    }
        //}

        //private float? _inert3Vol;
        //public float? inert3Vol
        //{
        //    get { return _inert3Vol; }
        //    set
        //    {
        //        _inert3Vol = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("inert3Vol"));
        //    }
        //}

        //private float? _additive1Vol;
        //public float? additive1Vol
        //{
        //    get { return _additive1Vol; }
        //    set
        //    {
        //        _additive1Vol = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("additive1Vol"));
        //    }
        //}

        //private float? _additive2Vol;
        //public float? additive2Vol
        //{
        //    get { return _additive2Vol; }
        //    set
        //    {
        //        _additive2Vol = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("additive2Vol"));
        //    }
        //}

        //private float? _additive3Vol;
        //public float? additive3Vol
        //{
        //    get { return _additive3Vol; }
        //    set
        //    {
        //        _additive3Vol = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("additive3Vol"));
        //    }
        //}
        //#endregion
        //#region Сделано
        //private float? _cementVol_f;
        //public float? cementVol_f
        //{
        //    get { return _cementVol_f; }
        //    set
        //    {
        //        _cementVol_f = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("cementVol_f"));
        //    }
        //}
        //private float? _waterVol_f;
        //public float? waterVol_f
        //{
        //    get { return _waterVol_f; }
        //    set
        //    {
        //        _waterVol_f = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("waterVol_f"));
        //    }
        //}
        //private float? _inert1Vol_f;
        //public float? inert1Vol_f
        //{
        //    get { return _inert1Vol_f; }
        //    set
        //    {
        //        _inert1Vol_f = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("inert1Vol_f"));
        //    }
        //}
        //private float? _inert2Vol_f;
        //public float? inert2Vol_f
        //{
        //    get { return _inert2Vol_f; }
        //    set
        //    {
        //        _inert2Vol_f = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("inert2Vol_f"));
        //    }
        //}

        //private float? _inert3Vol_f;
        //public float? inert3Vol_f
        //{
        //    get { return _inert3Vol_f; }
        //    set
        //    {
        //        _inert3Vol_f = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("inert3Vol_f"));
        //    }
        //}

        //private float? _additive1Vol_f;
        //public float? additive1Vol_f
        //{
        //    get { return _additive1Vol_f; }
        //    set
        //    {
        //        _additive1Vol_f = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("additive1Vol_f"));
        //    }
        //}

        //private float? _additive2Vol_f;
        //public float? additive2Vol_f
        //{
        //    get { return _additive2Vol_f; }
        //    set
        //    {
        //        _additive2Vol_f = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("additive2Vol_f"));
        //    }
        //}

        //private float? _additive3Vol_f;
        //public float? additive3Vol_f
        //{
        //    get { return _additive3Vol_f; }
        //    set
        //    {
        //        _additive3Vol_f = value;
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs("additive3Vol_f"));
        //    }
        //}
        //#endregion
        //public Database database { get; set; }
        ModbusMasterDevice PLC;

        public viewmodel_Tabl()
        {
            //this.PLC = PLC;
            //this.database = database;
            Update();
        }

        public void Update()
        {
            //if (PLC.GetValue("PCAY_01") != 0)
            //{
            //    try
            //    {
            //        order = database.orders.SingleOrDefault(x => x.orderID == PLC.GetValue("PCAY_01"));
            //        receipt = database.receipts.SingleOrDefault(x => x.receiptID == order.receiptID);
            //        report = database.reports.SingleOrDefault(x => x.orderID == order.orderID && x.cycleNum == PLC.GetValue("PCAY_14"));

            //        cementVol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.cementNorma * order.oneCycleVolume), 1));
            //        waterVol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.waterNorma * order.oneCycleVolume), 1));
            //        inert1Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.inert1Norma * order.oneCycleVolume), 0));
            //        inert2Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.inert2Norma * order.oneCycleVolume), 0));
            //        inert3Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.inert3Norma * order.oneCycleVolume), 0));
            //        additive1Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.additive1Norma * order.oneCycleVolume), 2));
            //        additive2Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.additive2Norma * order.oneCycleVolume), 2));
            //        additive3Vol = Convert.ToSingle(Math.Round(Convert.ToDecimal(receipt.additive3Norma * order.oneCycleVolume), 2));

            //        cementVol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.cementWeight), 1));
            //        waterVol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.waterWeight), 1));
            //        inert1Vol_f =Convert.ToSingle(Math.Round(Convert.ToDecimal( report.inert1Weight), 0));
            //        inert2Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.inert2Weight), 0));
            //        inert3Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.inert3Weight), 0));
            //        additive1Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.additive1Weight), 2));
            //        additive2Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.additive2Weight), 2));
            //        additive3Vol_f = Convert.ToSingle(Math.Round(Convert.ToDecimal(report.additive3Weight), 2));
            //    }
            //    catch (Exception ex)
            //    {
            //        System.IO.File.WriteAllText(@"Log\log.txt", DateTime.Now + " - " + ex.Message + "->" + PLC.GetValue("PCAY_01"));
            //    }
            //}
            //else
            //{
            //    order = null;
            //    receipt = null;
            //    report = null;

            //    cementVol = null;
            //    waterVol = null;
            //    inert1Vol = null;
            //    inert2Vol = null;
            //    inert3Vol = null;
            //    additive1Vol = null;
            //    additive2Vol = null;
            //    additive3Vol = null;

            //    cementVol_f = null;
            //    waterVol_f = null;
            //    inert1Vol_f = null;
            //    inert2Vol_f = null;
            //    inert3Vol_f = null;
            //    additive1Vol_f = null;
            //    additive2Vol_f = null;
            //    additive3Vol_f = null;
            //}
        }
    }
}