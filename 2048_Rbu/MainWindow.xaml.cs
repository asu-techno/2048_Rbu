using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using AsuBetonLibrary;
using AsuBetonLibrary.Elements;
using AsuBetonLibrary.Services;
using AsuBetonLibrary.Windows;
using _2048_Rbu.Classes;
using _2048_Rbu.Elements;
using _2048_Rbu.Handlers;
using _2048_Rbu.Helpers;
using NLog;
using _2048_Rbu.Windows;
using AS_Library.Events.Classes;
using ServiceLib.Classes;
using ServiceLib.Windows;
using System.Threading.Tasks;
using Opc.UaFx;

namespace _2048_Rbu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ElScreenRbu _elScreen = new ElScreenRbu();
        private Logger _logger;
        private bool _isLoad;

        public MainWindow()
        {
            ServiceData.Init(@"Data/Service.xlsx");

            Opc.UaFx.Client.Licenser.LicenseKey =
                "AALOERR5OO7EKFNQCABINGCH6TYOVHPLFC2QCUYAV3IYL7FGRNBJ4TYJX2GM6HKSCKZLBJWGHUWWXQ5HKWI7OFVYYMERDPQDC7ZW7ZTLPTM" +
                "LWLMY3RMLD2UQ6OXKUKC2YBBPBRGK6SBRI4DBXF4NGVKZUATMW3VI7EALG5FQNCETII7JG7OTOCL2EPO55TO5D4GPJROX5FHSUSALQX56E6" +
                "NCBRCDX35VJBWLQDD4QANXIWUKO7D3Q7SWDDL55ZZCSN7NLHKB3W5O524VIXFPLVJIYKM5U4OPRXPQ2IM5HNR5GRY4ZODEIIKTRJS2N2MBD" +
                "LYUUDDNUH5Y7X2MERUMPKZZHP3WWLGR2XUCXTZTA6MWSB6KXNJ3DMIA6ZU54UQVFC3FGTOU5YP2CHBVZOFZDZTDD2OX3XD5ZMPQTS4DU7C5" +
                "44BBNUXMNCNQ52DQSVBF3YYGVTPHHD7LMGCLZQA2EYXQBMNUIHQT3RZPFXRNUA3VKSXBN4WRK2QCN2MPWQL4RBNK7KEJVM2GK7QEOMX7FAN" +
                "SQGOIYLIQQCXGJB27SZEL6ZF4C4T6EUFOPKW7NSMEQWBAMCO2BSCP2YD4P4O3KHTUB7HXIFKUJNI5AVEM2YTDTLACIIOKI3BM7HXYJZBTHF" +
                "ME2O7ONQLRMFIQQF72YOWHVCRLLWGZOJUTWDWDPN532SOOV2AW2X4SFCVZHJGCG7OR5RYFI46NAA7Y3W52CHSMJKNGQEE7IDJPS76QFCOWE" +
                "TDJ6M2LXC4YMOO3CZYEZ6NVHWZQX2GJKU4CIYII5DTMAPACKWASLFUIHBMA47DBZYV5VBRUL6K7IGDTTZANTSFR4I2FGTFLVAXJOUR2WVEM" +
                "6L6AS6V5M3MU2KCRLLJGJ2QYOGYYCO4KYRVMRVK2324DAFHVMQJBC6YKDDSLPFHJLI6IHVNLHKMWWYFQC2O3KZTANJYC37J7DEV7XBBZIHY" +
                "7VBGTEKBW73KBCKZ3CQJDUW267N4QRJKSE4Z5YEYJDVESXC2JBHWPYXZV2VOU64D55EFWPQBN4KSZM3PLPELUQDLAE6LMCUC2QFODQI3E4D" +
                "Q7FI2GKTHYAO2KKWNI2STU7ABZ3VROFRTYOOEOLVI7FH2ZL35SDTWD2FPFWCR4SDXIRRFVXOGZJLF6KEUHOYGBRQR3CYA62YF4K2HYX46TX" +
                "ZQ3KUPWZTOBY3M7ZUJQFQ2KTXXIB75LVC7EHAZ2AF3OELXR2YP2AVTIGKBEPXAJIFQLB6CKHQQEHHSSPWO3HQHTYVZYGMVM4KUTDRHHSGUA";

            Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHlo0BLi03H5eu1wfyYtghdwJHRIUDASSNqz5wYiN/0REuF5rx" +
                                             "l0PuYej+g+9PW3zj9rf4q2Hr3Rp+4siFan2+bLDIWecyw4cso2kClUxmGiOPMf8e9yf/4LU06O2KwITH9htGzZ5H8f" +
                                             "WkYXqiSRovW8fvjzPHMKsHri+ywu+vv1sGfAwA0zzPsF2OEmeNC34NoNvq0aHta9JYfhfq9bkKZym8x02csbJJuI2R" +
                                             "c3V1rG5B/jATAQfCMOVMRttnnmQ6VnSEiSRStLhvBtVE0bC1kq4Z4RNpUVrGVcBn6w41aSyD8uOc78dBIu7a3ykZ45" +
                                             "1A/Q3ww3T1GtnmfAkGLK2b/srd675k7fpZrVlyiLH7BfEyo+hrBj51dYG0IaJqzseeuSh7GFjJZCihujJNgugyR4Id" +
                                             "N4KjrZg2sAuO4JeNlvzZvyQ7wNzu92lFHGoZGLnnKIAUQUk8FECgamOxYIYzP4UzLturdpFL6MBW6c9E0SdRy+Ydnb" +
                                             "SH40eSD+WGmkWgDhi8JDzJK2aIjAcTpjguEE";

            InitializeComponent();

            #region Init
            _logger = Service.GetInstance().GetLogger();
            LibService.Init(_logger);
            LibService.GetInstance().SetDbConnectionString(Service.GetInstance().GetOpcDict()["DbConnectionString"]);
            NewOpcServer.Init(_logger);
            NewOpcServer.GetInstance().InitOpc(NewOpcServer.OpcList.Rbu);
            NewOpcServer.GetInstance().ConnectOpc(NewOpcServer.OpcList.Rbu);

            var reportHelper = new ReportHelper(_logger);
            reportHelper.SubscribeReportSaving();

            OpcServer.Init(@"Data/Service.xlsx");
            OpcServer.GetInstance();
            #endregion

            GetVersion();
            Login();

            Title += Static.Version + " " + Static.Copyright;

            #region Масштаб экрана (comment)

            //var rect = SystemParameters.WorkArea;
            //if (rect.Width != 1920)
            //    MainGrid.LayoutTransform =
            //        new ScaleTransform(Math.Min(rect.Width, 1920) / 1920, Math.Min(rect.Height, 1080) / 1080);

            #endregion

            #region AutoEvent

            //var events = EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu);

            //events.AddAutoEvent("6_1_1.ServiceMode", SystemEventType.Warning, "Задвижка 6.1.1 - включен сервисный режим", ResponseType.Rising);

            #endregion
        }

        private void GetVersion()
        {
            var myVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var compileTime = new DateTime(2000, 1, 1).AddDays(myVersion.Build).AddSeconds(myVersion.Revision * 2);
            Static.Version = " v." + myVersion.Major + "." + myVersion.Minor + "." + compileTime.Day.ToString("D2") + compileTime.Month.ToString("D2") + "." + compileTime.Hour.ToString("D2") + compileTime.Minute.ToString("D2");
            Static.Copyright = "© ООО «‎АСУ-Техно»‎, " + compileTime.Year;
        }

        private void Login()
        {
            WindowLogin login = new WindowLogin(ServiceData.GetInstance().GetTitle(), Static.Version);
            login.LoginViewModel.PasswordCorrect += LoginPasswordCorrect;
            login.LoginViewModel.CloseMainWindow += CloseMainWindow;
            this.Hide();
            login.LoginViewModel.Create();
        }

        private void User()
        {
            WindowUser window = new WindowUser(null);
            window.ShowDialog();
        }

        private void LoginPasswordCorrect(User user)
        {
            //EventsBase.GetInstance().CreateControlEvents(OpcList.System);

            SelUser.GetInstance().SetSelUser(user);

            if (!_isLoad)
            {
                _elScreen.Initialize(_logger);
                ScreenGrid.Children.Add(_elScreen);
                _elScreen.Visibility = Visibility.Visible;
                _elScreen.LoginClick += Login;
                _elScreen.UserClick += User;
                _isLoad = true;
                //EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent("Программа управления открыта", SystemEventType.Message);
            }
            _elScreen.ViewModelScreenRbu.GetUserPermit();
            this.Show();
        }

        private void CloseMainWindow()
        {
            Close();
        }

        private void Program_Closed(object sender, EventArgs e)
        {
            ElScreenRbu.LinkTimer?.Stop();
            foreach (Window w in App.Current.Windows)
                w.Close();
        }

        void Program_Closing(object sender, CancelEventArgs e)
        {
            //EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu)?.AddEvent("Программа управления закрыта", SystemEventType.Message);
        }
    }
}
