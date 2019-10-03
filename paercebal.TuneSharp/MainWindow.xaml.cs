using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace paercebal.TuneSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Interfaces.IDebugOutputable
    {
        string currentFilename = @"D:\media\music\Dragonette - Merry Xmas (Says Your Text Message) [Explicit] - Copie\01 - Merry Xmas (Says Your Text Message) [Explicit] - Copie.mp3";

        public MainWindow()
        {
            InitializeComponent();

            this.musicPlayer.SetDebugOutputable("MusicPlayer", this);
            this.musicPlayer.Title = this.currentFilename;
        }

        #region Interfaces.IDebugOutputable

        Dictionary<string, string> logs = new Dictionary<string, string>();

        private void DoLog()
        {
            string log = "";

            foreach (var kv in this.logs)
            {
                log += string.Format("{0}: {1}\n"
                    , kv.Key
                    , kv.Value);
            }

            this.DebugTextBox.Text = log;
        }

        public void Log(string key, string text)
        {
            this.logs[key] = text;
            this.DoLog();
        }

        public void Log(string key, string format, params object[] o)
        {
            this.logs[key] = string.Format(format, o);
            this.DoLog();
        }

        #endregion // Interfaces.IDebugOutputable

        private void SetCurrentFilename(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                this.currentFilename = "";
            }
            else
            {
                this.currentFilename = filename;
            }

            this.musicPlayer.Title = this.currentFilename;
        }

        private void OnOpenFileButtonClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                this.SetCurrentFilename(openFileDialog.FileName);
            }
        }
    }
}
