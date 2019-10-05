using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace paercebal.TuneSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Globals Globals = new Globals();

        protected override void OnStartup(StartupEventArgs e)
        {
            //this.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative);

            this.Globals.ApplicationArguments.DataDirectory = this.GetApplicationDataDirectory(e);
            var mainWindow = new MainWindow(this.Globals);
            mainWindow.Show();
        }


        private string GetApplicationDataDirectory(StartupEventArgs e)
        {
            var s = Path.Combine(this.GetWorkingDataDirectory(e), "paercebal.TuneSharp");
            Directory.CreateDirectory(s);
            return s;
        }

        private string GetWorkingDataDirectory(StartupEventArgs e)
        {
            foreach(var arg in e.Args)
            {
                if (arg == "--portable")
                {
                    return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
    }
}
