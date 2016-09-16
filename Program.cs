using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPServer
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      //Server server = new Server("192.168.2.22", 4023);

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      FormMain formMain = new FormMain();
      //server.formMain = formMain;
      //formMain.server = server;
      Application.Run(formMain);
    }
  }
}
