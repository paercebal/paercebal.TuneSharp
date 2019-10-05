using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace paercebal.TuneSharp.Dialogs
{
    /// <summary>
    /// Interaction logic for MusicDirectoriesWindow.xaml
    /// </summary>
    public partial class MusicDirectoriesWindow : Window
    {
        public readonly SortedSet<string> TemporaryDirectories;

        public MusicDirectoriesWindow(SortedSet<string> temporaryDirectories)
        {
            InitializeComponent();

            this.TemporaryDirectories = temporaryDirectories;
            this.MusicDirectoriesListBox.ItemsSource = this.TemporaryDirectories;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if(this.DialogResult == null)
            {
                this.DialogResult = false;
            }

            base.OnClosing(e);
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private string OpenFolderDialog(string selectedPath)
        {
            var openDirectoryDialog = new System.Windows.Forms.FolderBrowserDialog();
            openDirectoryDialog.SelectedPath = selectedPath;

            if (openDirectoryDialog.ShowDialog(new Dialogs.ParentWpfWrapperForChildWinForm(this)) == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(openDirectoryDialog.SelectedPath))
                {
                    return openDirectoryDialog.SelectedPath;
                }
            }

            return null;
        }


        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            var path = this.OpenFolderDialog(@"D:\");

            if(path != null)
            {
                this.TemporaryDirectories.Add(path);

                this.MusicDirectoriesListBox.ItemsSource = null;
                this.MusicDirectoriesListBox.ItemsSource = this.TemporaryDirectories;
            }
        }


        private string GetTextContentAssociatedWithButton(object sender)
        {
            var button = sender as Button;
            if (button != null)
            {
                var grid = button.Parent as Grid;
                if (grid != null)
                {
                    foreach(var o in grid.Children)
                    {
                        var textBlock = o as TextBlock;

                        if(textBlock !=  null)
                        {
                            return textBlock.Text;
                        }
                    }
                }
            }

            return null;
        }

        private void ListItem_ModifyButtonClick(object sender, RoutedEventArgs e)
        {
            var text = this.GetTextContentAssociatedWithButton(sender);
            if(text != null)
            {
                var path = this.OpenFolderDialog(text);

                if ((path != text) && (path != null))
                {
                    this.TemporaryDirectories.Remove(text);
                    this.TemporaryDirectories.Add(path);

                    this.MusicDirectoriesListBox.ItemsSource = null;
                    this.MusicDirectoriesListBox.ItemsSource = this.TemporaryDirectories;
                }
            }
        }

        private void ListItem_RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            var text = this.GetTextContentAssociatedWithButton(sender);
            if (text != null)
            {
                this.TemporaryDirectories.Remove(text);

                this.MusicDirectoriesListBox.ItemsSource = null;
                this.MusicDirectoriesListBox.ItemsSource = this.TemporaryDirectories;
            }

        }
    }
}
