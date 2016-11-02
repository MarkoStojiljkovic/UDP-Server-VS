using System;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPServer
{
    static class Program
    {
        //[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        //[FlagsAttribute]
        //public enum EXECUTION_STATE : uint
        //{
        //    ES_AWAYMODE_REQUIRED = 0x00000040,
        //    ES_CONTINUOUS = 0x80000000,
        //    ES_DISPLAY_REQUIRED = 0x00000002,
        //    ES_SYSTEM_REQUIRED = 0x00000001
        //    // Legacy flag, should not be used.
        //    // ES_USER_PRESENT = 0x00000004
        //}



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Server server = new Server("192.168.2.22", 4023);

            // Prevent sleep mode
            //AllowMonitorPowerdown();
            //PreventMonitorPowerdown();
            //PreventSleep();
            //KeepSystemAwake();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormMain formMain = new FormMain();
            //server.formMain = formMain;
            //formMain.server = server;
            Storage.StorageInit();
            Application.Run(formMain);
        }


        //static void PreventMonitorPowerdown()
        //{
        //    SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        //}

        //static void AllowMonitorPowerdown()
        //{
        //    SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        //}

        //static void PreventSleep()
        //{
        //    // Prevent Idle-to-Sleep (monitor not affected) (see note above)
        //    SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
        //}

        //static void KeepSystemAwake()
        //{
        //    SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED);


        //}
    }
}
