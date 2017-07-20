using HCPACS4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace ACS4Ingest
{
  static class Program
  {
      private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      static void Main(string[] args)
      {
          IngestionJob.Run();
      }




      // TO RUN GUI COMMENT OUT ABOVE AND UNCOMMENT BELOW 
    
      /// <summary>
    /// The main entry point for the application.
    /// </summary>
      //[STAThread]
      //static void Main()
      //{
      //    Application.EnableVisualStyles();
      //    Application.SetCompatibleTextRenderingDefault(false);
      //    // run the main window
      //    Application.Run(new MainWindow());
      //}




  }
}
