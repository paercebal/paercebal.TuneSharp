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

namespace paercebal.TuneSharp.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:paercebal.TuneSharp.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:paercebal.TuneSharp.CustomControls;assembly=paercebal.TuneSharp.CustomControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:MusicPlayer/>
    ///
    /// </summary>
    public class MusicPlayer : Grid
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // PUBLIC
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title",
            typeof(string),
            typeof(MusicPlayer),
            new PropertyMetadata(string.Empty));

        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
                var isEmpty = string.IsNullOrWhiteSpace(value);
                var oldTitle = this.Title;
                var newTitle = isEmpty ? string.Empty : value;

                if(newTitle != oldTitle)
                {
                    this.SetValue(TitleProperty, newTitle);
                    this.titleTextBlock.Text = newTitle;

                    if (isEmpty)
                    {
                        this.mediaPlayer.Close();
                        this.State = Types.MediaPlayerState.None;
                    }
                    else
                    {
                        this.mediaPlayer.Open(new Uri(value, UriKind.Absolute));
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State",
            typeof(Types.MediaPlayerState),
            typeof(MusicPlayer),
            new PropertyMetadata(Types.MediaPlayerState.None));

        public Types.MediaPlayerState State
        {
            get
            {
                return (Types.MediaPlayerState)this.GetValue(StateProperty);
            }
            set
            {
                var oldState = this.State;
                var newState = value;

                if ((oldState != newState) && this.State.CanChangeTo(newState))
                {
                    this.SetValue(StateProperty, value);
                    newState.Update(this.mediaPlayer, this.timeProgressBar, out string title, this.playingPlayButton, this.playingPauseButton);
                    if(title != null)
                    {
                        this.Title = title;
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public MusicPlayer()
        {
            this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });
            this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });

            // <custom:ClickableProgressBar />
            this.timeProgressBar = new CustomControls.ClickableProgressBar
            {
                Minimum = 0,
                Maximum = 1,
                Value = .75,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(1, 2, 1, 2)
            };
            this.timeProgressBar.OnValueManuallyChanged += TimeProgressBar_OnValueManuallyChanged;
            Grid.SetRow(this.timeProgressBar, 0);
            this.Children.Add(this.timeProgressBar);

            // <Grid Grid.Row="1">
            this.controlsGrid = new Grid();
            this.controlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25) });
            this.controlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(75) });
            this.controlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25) });
            this.controlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            this.controlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25) });
            this.controlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25) });
            this.controlsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(this.controlsGrid, 1);
            this.Children.Add(this.controlsGrid);

            // <Button x:Name="PreviousButton">
            this.previousButton = new Button
            {
                Margin = new Thickness(1),
                Content = "N",
                ToolTip = new ToolTip { Content = "None" },
            };
            this.previousButton.Click += this.PreviousButtonClick;
            Grid.SetColumn(this.previousButton, 0);
            this.controlsGrid.Children.Add(this.previousButton);

            // <Button x:Name="PlayingPlayButton">
            this.playingPlayButton = new Button
            {
                Margin = new Thickness(1),
                Content = "Play",
                ToolTip = new ToolTip { Content = "Play" },
            };
            this.playingPlayButton.Click += this.PlayingPlayButtonClick;
            Grid.SetColumn(this.playingPlayButton, 1);
            this.controlsGrid.Children.Add(this.playingPlayButton);

            // <Button x:Name="PlayingPauseButton">
            this.playingPauseButton = new Button
            {
                Margin = new Thickness(1),
                Content = "Pause",
                ToolTip = new ToolTip { Content = "Pause" },
            };
            this.playingPauseButton.Click += this.PlayingPauseButtonClick;
            Grid.SetColumn(this.playingPauseButton, 1);
            this.controlsGrid.Children.Add(this.playingPauseButton);
            this.playingPauseButton.Visibility = Visibility.Collapsed;

            // <Button x:Name="NextButton">
            this.nextButton = new Button
            {
                Margin = new Thickness(1),
                Content = "S",
                ToolTip = new ToolTip { Content = "Stop" },
            };
            this.nextButton.Click += this.NextButtonClick;
            Grid.SetColumn(this.previousButton, 2);
            this.controlsGrid.Children.Add(this.nextButton);

            // <custom:ClickableProgressBar />
            this.volumeProgressBar = new CustomControls.ClickableProgressBar
            {
                Minimum = 0,
                Maximum = 1,
                Value = .75,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(1, 8, 1, 8)
            };
            //this.volumeProgressBar.OnValueManuallyChanged += VolumeProgressBar_OnValueManuallyChanged;
            Grid.SetColumn(this.volumeProgressBar, 3);
            this.controlsGrid.Children.Add(this.volumeProgressBar);

            // <Button x:Name="shuffleOnButton">
            this.shuffleOnButton = new Button
            {
                Margin = new Thickness(1),
                Content = "L",
                ToolTip = new ToolTip { Content = "Log Debug Information" },
            };
            this.shuffleOnButton.Click += this.ShuffleOnButtonClick;
            Grid.SetColumn(this.shuffleOnButton, 4);
            this.controlsGrid.Children.Add(this.shuffleOnButton);

            // <Button x:Name="shuffleOffButton">
            this.shuffleOffButton = new Button
            {
                Margin = new Thickness(1),
                Content = "L",
                ToolTip = new ToolTip { Content = "Log Debug Information" },
            };
            this.shuffleOffButton.Click += this.ShuffleOffButtonClick;
            Grid.SetColumn(this.shuffleOffButton, 4);
            this.controlsGrid.Children.Add(this.shuffleOffButton);
            this.shuffleOffButton.Visibility = Visibility.Collapsed;

            // <Button x:Name="repeatNoneButton">
            this.repeatNoneButton = new Button
            {
                Margin = new Thickness(1),
                Content = "O",
                ToolTip = new ToolTip { Content = "Open File" },
            };
            this.repeatNoneButton.Click += this.RepeatNoneButtonClick;
            Grid.SetColumn(this.repeatNoneButton, 5);
            this.controlsGrid.Children.Add(this.repeatNoneButton);

            // <Button x:Name="repeatAllButton">
            this.repeatAllButton = new Button
            {
                Margin = new Thickness(1),
                Content = "O",
                ToolTip = new ToolTip { Content = "Open File" },
            };
            this.repeatAllButton.Click += this.RepeatAllButtonClick;
            Grid.SetColumn(this.repeatAllButton, 5);
            this.controlsGrid.Children.Add(this.repeatAllButton);
            this.repeatAllButton.Visibility = Visibility.Collapsed;

            // <Button x:Name="repeatOneButton">
            this.repeatOneButton = new Button
            {
                Margin = new Thickness(1),
                Content = "O",
                ToolTip = new ToolTip { Content = "Open File" },
            };
            this.repeatOneButton.Click += this.RepeatOneButtonClick;
            Grid.SetColumn(this.repeatOneButton, 5);
            this.controlsGrid.Children.Add(this.repeatOneButton);
            this.repeatOneButton.Visibility = Visibility.Collapsed;

            //private TextBox titleTextBlock;
            this.titleTextBlock = new TextBlock
            {
                Margin = new Thickness(1),
            };
            Grid.SetColumn(this.titleTextBlock, 6);
            this.controlsGrid.Children.Add(this.titleTextBlock);

            this.dispatcherTimer.Interval = TimeSpan.FromMilliseconds(250);
            this.dispatcherTimer.Tick += DispatcherTimer_Tick;
            this.dispatcherTimer.Start();

            this.mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            this.mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            this.mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;

            {
                Binding binding = new Binding("Volume") { Mode = BindingMode.TwoWay, Source = this.mediaPlayer };
                this.volumeProgressBar.SetBinding(CustomControls.ClickableProgressBar.ValueProperty, binding);
            }

            {
                //// Ok, problem: The Position cannot seem to call back the binding when it is modified
                //// For now, mitigation is to use a timer instead
                //
                //Binding binding = new Binding("Position");
                //binding.Mode = BindingMode.TwoWay;
                //binding.Source = this.mediaPlayer;
                //binding.Converter = this.timeSpanToMilliSecondsConverter;
                //this.timeProgressBar.SetBinding(CustomControls.ClickableProgressBar.ValueProperty, binding);
            }
        }

        public void SetDebugOutputable(string debugKey, Interfaces.IDebugOutputable debugOutputable)
        {
            this.debugKey = debugKey;
            this.debugOutputable = debugOutputable;

            if(this.timeProgressBar != null)
            {
                this.timeProgressBar.SetDebugOutputable(debugKey, debugOutputable);
            }

            if (this.volumeProgressBar != null)
            {
                this.volumeProgressBar.SetDebugOutputable(debugKey, debugOutputable);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // PRIVATE
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static MusicPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MusicPlayer), new FrameworkPropertyMetadata(typeof(MusicPlayer)));
        }

        private readonly CustomControls.ClickableProgressBar timeProgressBar;
        private readonly Grid controlsGrid;
        private readonly Button previousButton;
        private readonly Button playingPlayButton;
        private readonly Button playingPauseButton;
        private readonly Button nextButton;
        private readonly CustomControls.ClickableProgressBar volumeProgressBar;
        private readonly Button shuffleOnButton;
        private readonly Button shuffleOffButton;
        private readonly Button repeatNoneButton;
        private readonly Button repeatAllButton;
        private readonly Button repeatOneButton;
        private readonly TextBlock titleTextBlock;

        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        //private Types.MediaPlayerState mediaPlayerState = Types.MediaPlayerState.None;
        private MediaPlayer mediaPlayer = new MediaPlayer();

        private Interfaces.IDebugOutputable debugOutputable = null;
        private string debugKey = "MusicPlayer";

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (this.State == Types.MediaPlayerState.Playing)
            {
                var position = this.mediaPlayer.Position;
                var value = position.TotalMilliseconds;
                this.timeProgressBar.Value = value;

                this.debugOutputable?.Log(this.debugKey + ".Tick", "{0}s -> {1}s"
                    , position
                    , value);
            }
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionEventArgs e)
        {
            MessageBox.Show(string.Format("Media failure for reason {0}", e), "Media Failure", MessageBoxButton.OK);
            this.timeProgressBar.Minimum = 0;
            this.timeProgressBar.Maximum = 1;
            this.timeProgressBar.Value = 0;
            this.State = Types.MediaPlayerState.None;
            this.debugOutputable?.Log(this.debugKey + ".MediaState", this.State.ToString());
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            this.timeProgressBar.Value = 0;
            this.State = Types.MediaPlayerState.Stopped;
            this.debugOutputable?.Log(this.debugKey + ".MediaState", this.State.ToString());
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            this.timeProgressBar.Minimum = 0;
            this.timeProgressBar.Maximum = this.mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            this.State = Types.MediaPlayerState.Stopped;

            this.debugOutputable?.Log(this.debugKey + ".MediaOpened", "{0}s - {1}s"
                , this.timeProgressBar.Minimum
                , this.timeProgressBar.Maximum);
            this.debugOutputable?.Log(this.debugKey + ".MediaState", this.State.ToString());
        }

        //private void VolumeProgressBar_OnValueManuallyChanged(double oldValue, double newValue)
        //{
        //    newValue = Types.Utils.Clamp(newValue, 0.0, 1.0);

        //    if (oldValue != newValue)
        //    {
        //        this.mediaPlayer.Volume = newValue;
        //        this.debugOutputable?.Log(this.debugKey + ".OnVolumeValueManuallyChanged", "{0}s -> {1}s"
        //            , oldValue
        //            , newValue);
        //    }
        //}

        private void TimeProgressBar_OnValueManuallyChanged(double oldValue, double newValue)
        {
            newValue = Types.Utils.Clamp(newValue, 0.0, this.mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds);

            if (oldValue != newValue)
            {
                this.mediaPlayer.Position = TimeSpan.FromMilliseconds(newValue);
                this.debugOutputable?.Log(this.debugKey + ".OnTiimeValueManuallyChanged", "{0}s -> {1}s"
                    , oldValue
                    , newValue);
            }
        }

        private void PreviousButtonClick(object sender, RoutedEventArgs e)
        {
            this.State = Types.MediaPlayerState.None;
            //this.mediaPlayer.Stop();
            //this.State = MediaPlayerState.Stopped;
        }

        private void PlayingPlayButtonClick(object sender, RoutedEventArgs e)
        {
            this.State = Types.MediaPlayerState.Playing;
        }

        private void PlayingPauseButtonClick(object sender, RoutedEventArgs e)
        {
            this.State = Types.MediaPlayerState.Paused;
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            this.State = Types.MediaPlayerState.Stopped;
        }

        private void ShuffleOnButtonClick(object sender, RoutedEventArgs e)
        {
            this.debugOutputable?.Log(this.debugKey + ".Shuffle", "Position: {0} [{1} > {2}|{3} > {4}]"
                , this.mediaPlayer.Position
                , this.timeProgressBar.Minimum
                , this.mediaPlayer.Position.TotalMilliseconds
                , this.timeProgressBar.Value
                , this.timeProgressBar.Maximum);
        }

        private void ShuffleOffButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private void RepeatNoneButtonClick(object sender, RoutedEventArgs e)
        {
            this.Title = @"D:\media\music\Dragonette - Merry Xmas (Says Your Text Message) [Explicit] - Copie\01 - Merry Xmas (Says Your Text Message) [Explicit] - Copie.mp3";
        }

        private void RepeatOneButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private void RepeatAllButtonClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
