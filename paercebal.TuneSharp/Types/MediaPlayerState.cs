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

namespace paercebal.TuneSharp.Types
{
    public abstract class MediaPlayerState
    {
        public static readonly MediaPlayerState None = new MediaPlayerStateNone();
        public static readonly MediaPlayerState Stopped = new MediaPlayerStateStopped();
        public static readonly MediaPlayerState Playing = new MediaPlayerStatePlaying();
        public static readonly MediaPlayerState Paused = new MediaPlayerStatePaused();

        public bool CanChangeTo(MediaPlayerState mediaPlayerState)
        {
            return CanChangeTo(mediaPlayerState.internalState);
        }

        public abstract void Update(MediaPlayer mediaPlayer, CustomControls.ClickableProgressBar timeProgressBar, out string title, Button playButton, Button pauseButton);

        // ========================================================================================
        //
        // PRIVATE: Base
        //
        // ========================================================================================

        private enum InternalState { None, Stopped, Playing, Paused }
        private readonly InternalState internalState;

        private MediaPlayerState(InternalState internalState)
        {
            this.internalState = internalState;
        }

        private bool CanChangeTo(InternalState newInternalState)
        {
            if (newInternalState == this.internalState)
            {
                return false;
            }

            switch (newInternalState)
            {
                case InternalState.None:
                    {
                        return true;
                    }
                case InternalState.Stopped:
                    {
                        return (this.internalState == InternalState.None) || (this.internalState == InternalState.Playing) || (this.internalState == InternalState.Paused);
                    }
                case InternalState.Playing:
                    {
                        return (this.internalState == InternalState.Stopped) || (this.internalState == InternalState.Paused);
                    }
                case InternalState.Paused:
                    {
                        return this.internalState == InternalState.Playing;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        // ========================================================================================
        //
        // PRIVATE: None
        //
        // ========================================================================================
        private sealed class MediaPlayerStateNone : MediaPlayerState
        {
            public MediaPlayerStateNone()
                : base(InternalState.None)
            {
            }

            public override sealed void Update(MediaPlayer mediaPlayer, CustomControls.ClickableProgressBar timeProgressBar, out string title, Button playButton, Button pauseButton)
            {
                title = string.Empty;

                if (mediaPlayer != null)
                {
                    mediaPlayer.Stop();
                    mediaPlayer.Close();
                }

                if (timeProgressBar != null)
                {
                    timeProgressBar.Value = 0;
                    timeProgressBar.Minimum = 0;
                    timeProgressBar.Maximum = 1;
                }

                if (playButton != null)
                {
                    playButton.IsEnabled = false;
                    playButton.Visibility = Visibility.Visible;
                }

                if (pauseButton != null)
                {
                    pauseButton.IsEnabled = false;
                    pauseButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        // ========================================================================================
        //
        // PRIVATE: Stopped
        //
        // ========================================================================================
        private sealed class MediaPlayerStateStopped : MediaPlayerState
        {
            public MediaPlayerStateStopped()
                : base(InternalState.Stopped)
            {
            }

            public override sealed void Update(MediaPlayer mediaPlayer, CustomControls.ClickableProgressBar timeProgressBar, out string title, Button playButton, Button pauseButton)
            {
                title = null;

                if (mediaPlayer != null)
                {
                    mediaPlayer.Stop();
                }

                if (timeProgressBar != null)
                {
                    timeProgressBar.Value = 0;
                }

                if (playButton != null)
                {
                    playButton.IsEnabled = true;
                    playButton.Visibility = Visibility.Visible;
                }

                if (pauseButton != null)
                {
                    pauseButton.IsEnabled = false;
                    pauseButton.Visibility = Visibility.Collapsed;
                }
            }
        }
        // ========================================================================================
        //
        // PRIVATE: Playing
        //
        // ========================================================================================
        private sealed class MediaPlayerStatePlaying : MediaPlayerState
        {
            public MediaPlayerStatePlaying()
                : base(InternalState.Playing)
            {
            }

            public override sealed void Update(MediaPlayer mediaPlayer, CustomControls.ClickableProgressBar timeProgressBar, out string title, Button playButton, Button pauseButton)
            {
                title = null;

                if (mediaPlayer != null)
                {
                    mediaPlayer.Play();
                }

                if (playButton != null)
                {
                    playButton.IsEnabled = false;
                    playButton.Visibility = Visibility.Collapsed;
                }

                if (pauseButton != null)
                {
                    pauseButton.IsEnabled = true;
                    pauseButton.Visibility = Visibility.Visible;
                }
            }
        }

        // ========================================================================================
        //
        // PRIVATE: Paused
        //
        // ========================================================================================
        private sealed class MediaPlayerStatePaused : MediaPlayerState
        {
            public MediaPlayerStatePaused()
                : base(InternalState.Paused)
            {
            }

            public override sealed void Update(MediaPlayer mediaPlayer, CustomControls.ClickableProgressBar timeProgressBar, out string title, Button playButton, Button pauseButton)
            {
                title = null;

                if (mediaPlayer != null)
                {
                    mediaPlayer.Pause();
                }

                if (playButton != null)
                {
                    playButton.IsEnabled = true;
                    playButton.Visibility = Visibility.Visible;
                }

                if (pauseButton != null)
                {
                    pauseButton.IsEnabled = false;
                    pauseButton.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
