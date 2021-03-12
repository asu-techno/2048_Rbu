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
using ServiceLibCore.Classes;
using ServiceLibCore.Windows;
using System.Threading.Tasks;
using Opc.UaFx;
using AsLibraryCore.Events.Classes;

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
        private WindowSplash _windowSplash;

        public MainWindow()
        {
            ServiceData.Init(@"Data/Service.xlsx");

            _windowSplash = new WindowSplash();
            _windowSplash.Show();

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
            AsLibraryCore.LibService.GetInstance().SetEventsDbConnectionString(Service.GetInstance().GetOpcDict()["EventsDbConnectionString"]);
            NewOpcServer.Init(_logger);
            NewOpcServer.GetInstance().InitOpc(NewOpcServer.OpcList.Rbu);
            NewOpcServer.GetInstance().ConnectOpc(NewOpcServer.OpcList.Rbu);

            var reportHelper = new ReportHelper(_logger);
            reportHelper.SubscribeReportSaving();

            OpcServer.Init(@"Data/Service.xlsx");
            OpcServer.GetInstance();

            EventsBase.GetInstance().CreateControlEvents(OpcServer.OpcList.Rbu);
            #endregion

            EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent("Программа управления открыта", SystemEventType.Message);
            Title += Release.GetInstance().GetVersion() + " " + Release.GetInstance().GetCopyright();

            Login();

            #region Масштаб экрана (comment)

            //var rect = SystemParameters.WorkArea;
            //if (rect.Width != 1920)
            //    MainGrid.LayoutTransform =
            //        new ScaleTransform(Math.Min(rect.Width, 1920) / 1920, Math.Min(rect.Height, 1080) / 1080);

            #endregion
        }

        private void Login()
        {
            WindowLogin login = new WindowLogin(ServiceData.GetInstance().GetTitle(), Release.GetInstance().GetVersion());
            login.LoginViewModel.PasswordCorrect += LoginPasswordCorrect;
            login.LoginViewModel.CloseMainWindow += CloseMainWindow;
            this.Hide();
            _windowSplash?.Close();
            login.LoginViewModel.CheckVersion(Release.GetInstance().GetBuildDateTime(), Release.GetInstance().GetVersion());
            login.LoginViewModel.Create();
        }

        private void User()
        {
            try
            {
                WindowUser window = new WindowUser(EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu));
                window.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Нет связи с базой данных", "Предупреждение");
            }
        }

        private void LoginPasswordCorrect(User user)
        {
            SelUser.GetInstance().SetSelUser(user);

            if (!_isLoad)
            {
                _elScreen.Initialize(_logger);
                ScreenGrid.Children.Add(_elScreen);
                _elScreen.Visibility = Visibility.Visible;
                CreateAutoEventAsync();
                _elScreen.LoginClick += Login;
                _elScreen.UserClick += User;
                _isLoad = true;
                EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent("Пользователь " + user.UserName + " вошел в программу", SystemEventType.Message);
            }
            _elScreen.ViewModelScreenRbu.GetUserPermit();
            this.Show();
        }

        private async void CreateAutoEventAsync()
        {
            await Task.Run(() =>
            {
                var events = EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu);
                #region Alarms
                events.AddAutoEvent("M_15.gb_AL_Feedback", SystemEventType.Alarm, "Насос M-15 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_15.gb_AL_External", SystemEventType.Alarm, "Насос M-15 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_15.gb_AL_DKS", SystemEventType.Alarm, "Насос M-15 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_16.gb_AL_Feedback", SystemEventType.Alarm, "Насос M-16 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_16.gb_AL_External", SystemEventType.Alarm, "Насос M-16 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_16.gb_AL_DKS", SystemEventType.Alarm, "Насос M-16 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_17.gb_AL_Feedback", SystemEventType.Alarm, "Насос M-17 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_17.gb_AL_External", SystemEventType.Alarm, "Насос M-17 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_17.gb_AL_DKS", SystemEventType.Alarm, "Насос M-17 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_18.gb_AL_Feedback", SystemEventType.Alarm, "Бетоносмеситель M-18 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_18.gb_AL_External", SystemEventType.Alarm, "Бетоносмеситель M-18 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_18.gb_AL_Oil", SystemEventType.Alarm, "Бетоносмеситель M-18 - авария системы смазки", ResponseType.Rising);
                events.AddAutoEvent("M_9.gb_AL_Feedback", SystemEventType.Alarm, "Конвейер M-9 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_9.gb_AL_External", SystemEventType.Alarm, "Конвейер M-9 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_9.gb_AL_DKS", SystemEventType.Alarm, "Конвейер M-9 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_1.gb_AL_Feedback", SystemEventType.Alarm, "Конвейер M-1 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_1.gb_AL_External", SystemEventType.Alarm, "Конвейер M-1 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_1.gb_AL_DKS", SystemEventType.Alarm, "Конвейер M-1 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_11.gb_AL_Feedback", SystemEventType.Alarm, "Шнек M-11 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_11.gb_AL_External", SystemEventType.Alarm, "Шнек M-11 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_11.gb_AL_DKS", SystemEventType.Alarm, "Шнек M-11 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("М_12.gb_AL_Feedback", SystemEventType.Alarm, "Шнек M-12 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("М_12.gb_AL_External", SystemEventType.Alarm, "Шнек M-12 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("М_12.gb_AL_DKS", SystemEventType.Alarm, "Шнек M-12 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_13.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-13 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_13.gb_AL_External", SystemEventType.Alarm, "Вибратор M-13 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_13.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-13 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_14.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-14 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_14.gb_AL_External", SystemEventType.Alarm, "Вибратор M-14 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_14.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-14 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_10.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-10 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_10.gb_AL_External", SystemEventType.Alarm, "Вибратор M-10 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_10.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-10 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_2.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-2 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_2.gb_AL_External", SystemEventType.Alarm, "Вибратор M-2 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_2.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-2 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_3.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-3 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_3.gb_AL_External", SystemEventType.Alarm, "Вибратор M-3 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_3.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-3 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_4.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-4 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_4.gb_AL_External", SystemEventType.Alarm, "Вибратор M-4 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_4.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-4 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_5.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-5 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_5.gb_AL_External", SystemEventType.Alarm, "Вибратор M-5 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_5.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-5 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_6.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-6 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_6.gb_AL_External", SystemEventType.Alarm, "Вибратор M-6 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_6.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-6 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("M_7.gb_AL_Feedback", SystemEventType.Alarm, "Вибратор M-7 - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_7.gb_AL_External", SystemEventType.Alarm, "Вибратор M-7 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_7.gb_AL_DKS", SystemEventType.Alarm, "Вибратор M-7 - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("V_1.gb_AL_Feedback_Open", SystemEventType.Alarm, "Задвижка V-1 - авария ОС-Open", ResponseType.Rising);
                events.AddAutoEvent("V_1.gb_AL_Feedback_Close", SystemEventType.Alarm, "Задвижка V-1 - авария ОС-Close", ResponseType.Rising);
                events.AddAutoEvent("V_1.gb_AL_BothSensor", SystemEventType.Alarm, "Задвижка V-1 - авария датчиков", ResponseType.Rising);
                events.AddAutoEvent("V_1.gb_AL_External", SystemEventType.Alarm, "Задвижка V-1 - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("V_2.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-2 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_2.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-2 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_3.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-3 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_3.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-3 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_4.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-4 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_4.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-4 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_5.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-5 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_5.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-5 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_6.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-6 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_6.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-6 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_7.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-7 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_7.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-7 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_9_1.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-9-1 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_9_1.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-9-1 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_9_2.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-9-2 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_9_2.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-9-2 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_10_1.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-10-1 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_10_1.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-10-1 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_10_2.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-10-2 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_10_2.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-10-2 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_11_1.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-11-1 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_11_1.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-11-1 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_11_2.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-11-2 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_11_2.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-11-2 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_12_1.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-12-1 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_12_1.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-12-1 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("V_12_2.gb_AL_Open", SystemEventType.Alarm, "Задвижка V-12-2 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("V_12_2.gb_AL_Close", SystemEventType.Alarm, "Задвижка V-12-2 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("M_19.gb_AL_Feedback", SystemEventType.Alarm, "Насос M-19 - Вода - авария ОС", ResponseType.Rising);
                events.AddAutoEvent("M_19.gb_AL_External", SystemEventType.Alarm, "Насос M-19 - Вода - внешняя авария", ResponseType.Rising);
                events.AddAutoEvent("M_19.gb_AL_DKS", SystemEventType.Alarm, "Насос M-19 - Вода - авария ДКС", ResponseType.Rising);
                events.AddAutoEvent("Air_Cement1.gb_AL_Open", SystemEventType.Alarm, "Аэратор силоса цемента 1 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("Air_Cement1.gb_AL_Close", SystemEventType.Alarm, "Аэратор силоса цемента 1 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("Air_Cement2.gb_AL_Open", SystemEventType.Alarm, "Аэратор силоса цемента 2 - авария открытия", ResponseType.Rising);
                events.AddAutoEvent("Air_Cement2.gb_AL_Close", SystemEventType.Alarm, "Аэратор силоса цемента 2 - авария закрытия", ResponseType.Rising);
                events.AddAutoEvent("gb_AL_Hydro_Feedback", SystemEventType.Alarm, "Насос гидростанции -не сработал пускатель", ResponseType.Rising);
                events.AddAutoEvent("DI_NotStop_BTN_PLC", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления контроллера", ResponseType.Faling);
                events.AddAutoEvent("DI_NotStop_BTN_Inert", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления инертными фракциями", ResponseType.Faling);
                events.AddAutoEvent("DI_NotStop_BTN_Doz", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления дозированием", ResponseType.Faling);
                events.AddAutoEvent("DI_NotStop_BTN_Cem", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления цементом", ResponseType.Faling);
                events.AddAutoEvent("DI_NotStop_BTN_Water", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления жидкими компонентами", ResponseType.Faling);
                events.AddAutoEvent("DI_NotStop_BTN_BS", SystemEventType.Alarm, "Нажата стоповая кнопка в шкафу управления бетоносмесителем", ResponseType.Faling);
                events.AddAutoEvent("cmd_Stop", SystemEventType.Alarm, "Общий стоп", ResponseType.Rising);
                events.AddAutoEvent("gb_LinkERR_WeightCement", SystemEventType.Alarm, "Нет связи с весовым индикатором цемента", ResponseType.Rising);
                events.AddAutoEvent("gb_LinkERR_WeightWater", SystemEventType.Alarm, "Нет связи с весовым индикатором воды", ResponseType.Rising);
                events.AddAutoEvent("gb_LinkERR_WeightInert", SystemEventType.Alarm, "Нет связи с весовым индикатором инертных фракций", ResponseType.Rising);
                events.AddAutoEvent("gb_LinkERR_WeightAdditive", SystemEventType.Alarm, "Нет связи с весовым индикатором химических добавок", ResponseType.Rising);
                #endregion

                #region Warnings
                events.AddAutoEvent("M_18.gb_Warning_Oil", SystemEventType.Warning, "Бетоносмеситель M-18 - нет давления в системе смазки", ResponseType.Rising);
                events.AddAutoEvent("DI_Gates_M18_Closed", SystemEventType.Warning, "Крышка открыта", ResponseType.Faling);
                events.AddAutoEvent("DI_M9_sw_AutomatMode", SystemEventType.Warning, "Бетоносмеситель M-18 - автоматический режим", ResponseType.Rising);
                events.AddAutoEvent("DI_M18_sw_AutomatMode", SystemEventType.Warning, "Конвейер M-9 - автоматический", ResponseType.Rising);
                events.AddAutoEvent("DI_M9_sw_AutomatMode", SystemEventType.Warning, "Бетоносмеситель M-18 - ручной режим", ResponseType.Faling);
                events.AddAutoEvent("DI_M18_sw_AutomatMode", SystemEventType.Warning, "Конвейер M-9 - ручной режим", ResponseType.Faling);
                events.AddAutoEvent("gb_AL_Hydro_Pressure", SystemEventType.Warning, "Низкое давление в гидравлической магистрали", ResponseType.Rising);
                events.AddAutoEvent("gb_ArchiverERROR", SystemEventType.Warning, "Нет связи с программой архивации", ResponseType.Rising);
                #endregion
            });
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
            EventsBase.GetInstance().GetControlEvents(OpcServer.OpcList.Rbu).AddEvent("Программа управления закрыта", SystemEventType.Message);
        }
    }
}
