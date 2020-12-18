using System;
using System.Windows.Controls;
using System.Windows.Threading;
using AS_Library.Link;
using System.ComponentModel;


namespace _2048_Rbu.Elements.Control
{
    /// <summary>
    /// Логика взаимодействия для el_Zames.xaml
    /// </summary>
       public partial class ElZamesDozing : UserControl
    {
        public DispatcherTimer Timer;
        public ModbusMasterDevice PLC;
        public viewmodel_Dozing viewmodelDozing;

        public ElZamesDozing()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            viewmodelDozing = new viewmodel_Dozing();
            this.DataContext = viewmodelDozing;

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            //if (database != null)
            //    viewmodelDozing.Update();
        }

        public int WordToInt(int val)
        {
            if (val <= 32767)
                return val;
            else
                return (val - 65536);
        }
    }

       public class viewmodel_Dozing : INotifyPropertyChanged
       {
           public event PropertyChangedEventHandler PropertyChanged;

           private string _receiptName;
           public string receiptName
           {
               get { return _receiptName; }
               set
               {
                   _receiptName = value;
                   if (PropertyChanged != null)
                       PropertyChanged(this, new PropertyChangedEventArgs("receiptName"));
               }
           }
           private int? _orderCycles;
           public int? orderCycles
           {
               get { return _orderCycles; }
               set
               {
                   _orderCycles = value;
                   if (PropertyChanged != null)
                       PropertyChanged(this, new PropertyChangedEventArgs("orderCycles"));
               }
           }
           private int? _orderActCycle;
           public int? orderActCycle
           {
               get { return _orderActCycle; }
               set
               {
                   _orderActCycle = value;
                   if (PropertyChanged != null)
                       PropertyChanged(this, new PropertyChangedEventArgs("orderActCycle"));
               }
           }
           private int? _dozingProcess;
           public int? dozingProcess
           {
               get { return _dozingProcess; }
               set
               {
                   _dozingProcess = value;
                   if (PropertyChanged != null)
                       PropertyChanged(this, new PropertyChangedEventArgs("dozingProcess"));
               }
           }

           //Database database;
           //ModbusMasterDevice PLC;

           public viewmodel_Dozing()
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
               //        Orders tmp_order = database.orders.SingleOrDefault(x => x.orderID == PLC.GetValue("PCAY_01"));
               //        Receipts tmp_receipt = database.receipts.SingleOrDefault(x => x.receiptID == tmp_order.receiptID);

               //        receiptName = tmp_receipt.receiptName;
               //        orderCycles = tmp_order.orderCycles;
               //        orderActCycle = PLC.GetValue("PCAY_14");
               //        dozingProcess = PLC.GetValue("PCAY_11");
               //    }
               //    catch (Exception ex)
               //    {
               //        System.IO.File.WriteAllText(@"Log\log.txt", DateTime.Now + " - " + ex.Message + "->" + PLC.GetValue("PCAY_01"));
               //    }
               //}
               //else
               //{
               //    receiptName = null;
               //    orderCycles = null;
               //    orderActCycle = null;
               //    dozingProcess = null;
               //}
           }
       }
}