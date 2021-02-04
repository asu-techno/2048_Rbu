using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace _2048_Rbu
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.IO.File.AppendAllText("log.txt", DateTime.Now + " - " + e.ExceptionObject);
            MessageBox.Show("Что-то пошло не так");
            MainWindow?.Close();
        }

        protected override void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            var currentProcess = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(currentProcess.ProcessName).Length > 1)
            {
                MessageBox.Show("Запущено более одной копии программного обеспечения");
                currentProcess.Kill();
            }

            //if (!InstanceCheck())
            //{
            //    MessageBox.Show("Запущено более одной копии программного обеспечения");
            //    Process.GetCurrentProcess().Kill();
            //    //foreach (Process proc in Process.GetProcessesByName("ServerArchivator"))
            //    //{
            //    //    proc.Kill();
            //    //}
            //}
        }

        //// держим в переменной, чтобы сохранить владение им до конца пробега программы
        //static Mutex InstanceCheckMutex;
        //static bool InstanceCheck()
        //{
        //    bool isNew;
        //    InstanceCheckMutex = new Mutex(true, "<ServerArchivator>", out isNew);
        //    return isNew;
        //}
    }
}
