using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MultiYoutubePlex.Enums;
using MultiYoutubePlex.Helpers;
using YoutubeExtractor;

namespace MultiYoutubePlex
{
    internal static class Downloader
    {
        public static ProgressBarWrapper CurrentDownloadProgress = new ProgressBarWrapper();
        public static ProgressBarWrapper CurrentAudioConversionProgress = new ProgressBarWrapper();

        public static void Download(string videoLink, string saveLocation,Resolution resolution, DownloadType downloadType = DownloadType.Video)
        {

            Task task; 
            switch (downloadType)
            {
                case DownloadType.Audio:
                    task = new Task(() => DownloadVideo(videoLink, saveLocation,resolution),TaskCreationOptions.LongRunning);
                    task.Start();
                    break;
                case DownloadType.Video:
                    task = new Task(() =>  DownloadVideo(videoLink, saveLocation,resolution), TaskCreationOptions.LongRunning);
                    task.Start();
                    break;
            }
        }
        
        #region Downloaders
        private static void DownloadVideo(string videoLink, string saveLocation, Resolution resolution)
        {


            VideoInfo video = GetVideo(videoLink, resolution);

            /*
             * If the video has a decrypted signature, decipher it
             */


            /*
             * Create the video downloader.
             * The first argument is the video to download.
             * The second argument is the path to save the video file.
             */

            var path = saveLocation != ""
                ? saveLocation
                : Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

            var videoDownloader = new VideoDownloader(video,
                Path.Combine(path,
                    RemoveIllegalPathCharacters(video.Title) + video.VideoExtension));

            // Register the ProgressChanged event and print the current progress
            videoDownloader.DownloadProgressChanged += (sender, args) => CurrentDownloadProgress.CurrentProgress += args.ProgressPercentage;

            /*
             * Execute the video downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            videoDownloader.Execute();
        }

        private static void DownloadAudio(string videoLink, string saveLocation)
        { 

            var videoInfos = DownloadUrlResolver.GetDownloadUrls(videoLink, false);
            VideoInfo video = videoInfos
                .Where(info => info.CanExtractAudio)
                .OrderByDescending(info => info.AudioBitrate)
                .First();

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            /*
             * Create the audio downloader.
             * The first argument is the video where the audio should be extracted from.
             * The second argument is the path to save the audio file.
             */
            var path = saveLocation != ""
                ? saveLocation
                : Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            var audioDownloader = new AudioDownloader(video,
               Path.Combine(path,
                   RemoveIllegalPathCharacters(video.Title) + video.AudioExtension));

            // Register the progress events. We treat the download progress as 85% of the progress
            // and the extraction progress only as 15% of the progress, because the download will
            // take much longer than the audio extraction.
            audioDownloader.DownloadProgressChanged += (sender, args) => CurrentAudioConversionProgress.CurrentProgress = args.ProgressPercentage * 0.85;
            audioDownloader.AudioExtractionProgressChanged += (sender, args) => CurrentAudioConversionProgress.CurrentProgress = args.ProgressPercentage * 0.15;

            /*
             * Execute the audio downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            audioDownloader.Execute();
        } 
        #endregion
        #region Helpers

        public static VideoInfo GetVideo(string videoLink, Resolution resolution)
        {
            var videoInfos = DownloadUrlResolver.GetDownloadUrls(videoLink, false);

            var videoResolution = (int)resolution;
            VideoInfo video = videoInfos
                .FirstOrDefault(info => info.Resolution == videoResolution);

            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            return video;
        }

        private static string RemoveIllegalPathCharacters(string path)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex($"[{Regex.Escape(regexSearch)}]");
            return r.Replace(path, "");
        } 
        #endregion
    }
}
