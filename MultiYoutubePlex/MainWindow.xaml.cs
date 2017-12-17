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
using MultiYoutubePlex.Enums;
using MultiYoutubePlex.Helpers;

namespace MultiYoutubePlex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ProgressBarWrapper Progress;

        public MainWindow()
        {
            InitializeComponent();
            Progress = Downloader.CurrentDownloadProgress;
            DataContext = Progress;

        }

        private void DownloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (VideoLinkTextBox.Text == "")
            {
                MessageBox.Show("NO LINK BRO!!!");
                return;
            }

            var downloadType = VideoRadioButton.IsEnabled ? DownloadType.Video : DownloadType.Audio;

            try
            {
                if (PlaylistRadioButton.IsEnabled)
                {
                    Downloader.DownloadFromPlaylist(VideoLinkTextBox.Text, SaveLocationTextBox.Text, Resolution.HD,
                        downloadType);
                }
                else
                {
                    Downloader.Download(VideoLinkTextBox.Text, SaveLocationTextBox.Text, Resolution.HD, downloadType);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }


        }
    }
}
