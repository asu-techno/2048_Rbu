using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using AS_Library.Link;
using System.ComponentModel;

namespace _2048_Rbu.Elements.Control
{
    /// <summary>
    /// Логика взаимодействия для el_Zames.xaml
    /// </summary>
    public partial class ElZamesMixer : UserControl
     {
        public DispatcherTimer Timer;
        public ModbusMasterDevice PLC;
        public viewmodel_Mixer viewmodelMixer;

        public ElZamesMixer()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            this.PLC = PLC;

            viewmodelMixer = new viewmodel_Mixer();
            this.DataContext = viewmodelMixer;

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            //if (database != null)
            //    viewmodelMixer.Update();
        }

        private void lbl_t_razgr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Window_setParameter window = new Window_setParameter(PLC, "Длительность разгрузки, c", 0, 100, "PCAX_15", "PCX_15", "PCAY_15", 0, controlEvents);
            //window.Show();
        }
    }

       public class viewmodel_Mixer : INotifyPropertyChanged
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
           private int? _mixingProcess;
           public int? mixingProcess
           {
               get { return _mixingProcess; }
               set
               {
                   _mixingProcess = value;
                   if (PropertyChanged != null)
                       PropertyChanged(this, new PropertyChangedEventArgs("mixingProcess"));
               }
           }
           private string _timeProcess;
           public string timeProcess
           {
               get { return _timeProcess; }
               set
               {
                   _timeProcess = value;
                   if (PropertyChanged != null)
                       PropertyChanged(this, new PropertyChangedEventArgs("timeProcess"));
               }
           }
           private int? _razgruzkaProcess;
           public int? razgruzkaProcess
           {
               get { return _razgruzkaProcess; }
               set
               {
                   _razgruzkaProcess = value;
                   if (PropertyChanged != null)
                       PropertyChanged(this, new PropertyChangedEventArgs("razgruzkaProcess"));
               }
           }
           private string _timeRazgruzka;
           public string timeRazgruzka
           {
               get { return _timeRazgruzka; }
               set
               {
                   _timeRazgruzka = value;
                   if (PropertyChanged != null)
                       PropertyChanged(this, new PropertyChangedEventArgs("timeRazgruzka"));
               }
           }


           //Database database;
           //ModbusMasterDevice PLC;

           public viewmodel_Mixer(/*ModbusMasterDevice PLC, Database database*/)
           {
               //this.PLC = PLC;
               //this.database = database;
               //Update();
           }

           //public void Update()
           //{
           //    if (PLC.GetValue("PCAY_08") != 0)
           //    {
           //        try
           //        {
           //            Orders tmp_order = database.orders.SingleOrDefault(x => x.orderID == PLC.GetValue("PCAY_08"));
           //            Receipts tmp_receipt = database.receipts.SingleOrDefault(x => x.receiptID == tmp_order.receiptID);

           //            receiptName = tmp_receipt.receiptName;
           //            orderCycles = tmp_order.orderCycles;
           //            orderActCycle = PLC.GetValue("PCAY_101");
           //            mixingProcess = PLC.GetValue("PCAY_12");
           //            timeProcess = WordToInt(PLC.GetValue("PCAY_09")).ToString() + " с";
           //            razgruzkaProcess = PLC.GetValue("PCAY_13");
           //            timeRazgruzka = WordToInt(PLC.GetValue("PCAY_10")).ToString() + " с";
           //        }
           //        catch (Exception ex)
           //        {
           //             System.IO.File.WriteAllText(@"Log\log.txt", DateTime.Now + " - " + ex.Message + "->" + PLC.GetValue("PCAY_08"));
           //        }
           //    }
           //    else
           //    {
           //        receiptName = null;
           //        orderCycles = null;
           //        orderActCycle = null;
           //        mixingProcess = null;
           //        timeProcess = null;
           //        razgruzkaProcess = null;
           //        timeRazgruzka = null;
           //    }
           //}

           //public int WordToInt(int val)
           //{
           //    if (val <= 32767)
           //        return val;
           //    else
           //        return (val - 65536);
           //}
       }
}
 