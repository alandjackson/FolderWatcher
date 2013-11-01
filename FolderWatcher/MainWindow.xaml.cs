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

namespace FolderWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FolderWatcher.Common.FolderWatcher FolderWatcherInstance = new Common.FolderWatcher();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            FolderWatcherInstance.NewFile += (s, e) => AppendStatus("New File: " + e.Param);
        }

        private void AppendStatus(string message)
        {
            StatusMessagesTextBlock.Text += message + Environment.NewLine;
            StatusMessageScrollViewer.ScrollToEnd();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FolderTextBox.Text = FolderWatcher.Properties.Settings.Default.Folder;
        }

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FolderTextBox.Text = dlg.SelectedPath;
            }
        }

        private void FolderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FolderWatcher.Properties.Settings.Default.Folder = FolderTextBox.Text;
            FolderWatcher.Properties.Settings.Default.Save();
        }

        private void HotfolderControlButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)HotfolderStateLabel.Content == "Stopped")
            {
                FolderWatcherInstance.Join();

                FolderWatcherInstance.FolderToWatch = FolderTextBox.Text;
                FolderWatcherInstance.IgnoreStartingFiles = true;
                FolderWatcherInstance.RunAsync();

                AppendStatus("Started watching: " + FolderWatcherInstance.FolderToWatch);
                HotfolderStateLabel.Content = "Running...";
                HotfolderControlButton.Content = "Stop";
            }
            else
            {
                FolderWatcherInstance.End();
                AppendStatus("Stopped watching folder");
                HotfolderStateLabel.Content = "Stopped";
                HotfolderControlButton.Content = "Start";
            }
        }

        
    }
}
