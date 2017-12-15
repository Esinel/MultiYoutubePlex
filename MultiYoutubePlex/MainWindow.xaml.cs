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
        public static ProgressBarWrapper CurrentProgress = Downloader.CurrentDownloadProgress;

        public MainWindow()
        {
            InitializeComponent();
            CurrentProgress.PropertyChanged += (sender, args) => DownloadProgress.Value += CurrentProgress.CurrentProgress;
        }

        private void DownloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (VideoLinkTextBox.Text == "")
            {
                MessageBox.Show("NO LINK BRO!!!");
            }

            var downloadType = VideoRadioButton.IsEnabled ? DownloadType.Video : DownloadType.Audio;

            Downloader.Download(VideoLinkTextBox.Text, SaveLocationTextBox.Text, Resolution.FullHD, downloadType);


        }






    }
}
