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
    enum MediaPlayerState
    {
        None,
        Stopped,
        Playing,
        Paused
    }

    [ValueConversion(typeof(TimeSpan), typeof(double))]
    public class TimeSpanToMilliSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((TimeSpan)value).TotalMilliseconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TimeSpan.FromMilliseconds((double)value);
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Interfaces.IDebugOutputable
    {
        string currentFilename = @"D:\media\music\Dragonette - Merry Xmas (Says Your Text Message) [Explicit] - Copie\01 - Merry Xmas (Says Your Text Message) [Explicit] - Copie.mp3";
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private MediaPlayerState mediaPlayerState = MediaPlayerState.None;
        private TimeSpanToMilliSecondsConverter timeSpanToMilliSecondsConverter = new TimeSpanToMilliSecondsConverter();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            this.mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            this.mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            {
                Binding binding = new Binding("Volume");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this.mediaPlayer;
                this.VolumeProgress.SetBinding(CustomControls.ClickableProgressBar.ValueProperty, binding);
            }

            {
                //// Ok, problem: The Position cannot seem to call back the binding when it is modified
                //// For now, mitigation is to use a timer instead
                //
                //Binding binding = new Binding("Position");
                //binding.Mode = BindingMode.TwoWay;
                //binding.Source = this.mediaPlayer;
                //binding.Converter = this.timeSpanToMilliSecondsConverter;
                //this.MusicProgress.SetBinding(CustomControls.ClickableProgressBar.ValueProperty, binding);
            }

            this.VolumeProgress.SetDebugOutputable("VolumeProgress", this);
            this.MusicProgress.SetDebugOutputable("MusicProgress", this);
            this.MusicProgress.OnValueManuallyChanged += MusicProgress_OnValueManuallyChanged;

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(250);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (this.mediaPlayerState == MediaPlayerState.Playing)
            {
                var position = this.mediaPlayer.Position;
                var value = position.TotalMilliseconds;
                this.MusicProgress.Value = value;
                this.Log("MainWindow.Tick", "{0}s -> {1}s"
                    , position
                    , value);
            }
        }

        private void MusicProgress_OnValueManuallyChanged(double oldValue, double newValue)
        {
            this.mediaPlayer.Position = TimeSpan.FromMilliseconds(newValue);
            this.Log("MainWindow.OnValueManuallyChanged", "{0}s -> {1}s"
                , oldValue
                , newValue);
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

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            this.MusicProgress.Minimum = 0;
            this.MusicProgress.Maximum = this.mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            this.Log("MainWindow.MediaOpened", "{0}s - {1}s"
                , this.MusicProgress.Minimum
                , this.MusicProgress.Maximum);

            this.mediaPlayer.Play();
            this.mediaPlayerState = MediaPlayerState.Playing;
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            this.MusicProgress.Value = 0;
            this.Log("MainWindow.MediaEnded", "...");
        }

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

            this.TitleTextBlock.Text = this.currentFilename;
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

        private void OnPlayButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.mediaPlayerState == MediaPlayerState.None)
            {
                if (!string.IsNullOrWhiteSpace(this.currentFilename))
                {
                    this.mediaPlayer.Open(new Uri(this.currentFilename, UriKind.Absolute));
                }
            }
            else if (this.mediaPlayerState == MediaPlayerState.Stopped)
            {
                this.mediaPlayer.Play();
                this.mediaPlayerState = MediaPlayerState.Playing;
            }
            else if (this.mediaPlayerState == MediaPlayerState.Playing)
            {
                this.mediaPlayer.Pause();
                this.mediaPlayerState = MediaPlayerState.Paused;
            }
            else if (this.mediaPlayerState == MediaPlayerState.Paused)
            {
                this.mediaPlayer.Play();
                this.mediaPlayerState = MediaPlayerState.Playing;
            }
        }

        private void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            this.mediaPlayer.Stop();
            this.mediaPlayerState = MediaPlayerState.Stopped;
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            this.mediaPlayer.Stop();
            this.mediaPlayerState = MediaPlayerState.Stopped;
        }

        private void ShuffleButtonClick(object sender, RoutedEventArgs e)
        {
            this.Log("MainWindow.Shuffle", "Position: {0} [{1} > {2}|{3} > {4}]"
                , this.mediaPlayer.Position
                , this.MusicProgress.Minimum
                , this.mediaPlayer.Position.TotalMilliseconds
                , this.MusicProgress.Value, this.MusicProgress.Maximum);
        }
    }
}
