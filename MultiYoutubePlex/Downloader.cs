using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MultiYoutubePlex.Enums;
using MultiYoutubePlex.Helpers;
using YoutubeExtractor;

namespace MultiYoutubePlex
{
    public static class Downloader
    {
        public static ProgressBarWrapper CurrentDownloadProgress = new ProgressBarWrapper();
        public static ProgressBarWrapper CurrentAudioConversionProgress = new ProgressBarWrapper();

        public static void Download(string videoLink, string saveLocation, Resolution resolution, DownloadType downloadType = DownloadType.Video, UrlType urlType = UrlType.Video)
        {

            Task task;
            switch (downloadType)
            {
                case DownloadType.Audio:
                    //task = new Task(() => DownloadAudio(videoLink, saveLocation), TaskCreationOptions.LongRunning);
                    //task.Start();
                    DownloadAudio(videoLink, saveLocation);
                    break;
                case DownloadType.Video:
                    //task = new Task(() => DownloadVideo(videoLink, saveLocation, resolution), TaskCreationOptions.LongRunning);
                    //task.Start();
                    DownloadVideo(videoLink, saveLocation, resolution);
                    break;
            }
        }

        #region Downloaders
        public static void DownloadFromPlaylist(string playlistUrl,
            string saveLocation,
            Resolution resolution,
            DownloadType downloadType = DownloadType.Video)
        {
            var videoUrls = PlaylistParser.ParsePlayList(playlistUrl);
            Task task;
            foreach (var url in videoUrls)
            {
                switch (downloadType)
                {
                    case DownloadType.Audio:
                        //task = new Task(() => DownloadAudio(videoLink, saveLocation), TaskCreationOptions.LongRunning);
                        //task.Start();
                        DownloadAudio(url, saveLocation);
                        break;
                    case DownloadType.Video:
                        //task = new Task(() => DownloadVideo(videoLink, saveLocation, resolution), TaskCreationOptions.LongRunning);
                        //task.Start();
                        DownloadVideo(url, saveLocation, resolution);
                        break;
                }
            }
        }

        public static void DownloadVideo(string videoLink, string saveLocation, Resolution resolution)
        {


            var video = GetVideo(videoLink, resolution);

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }


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
            videoDownloader.DownloadProgressChanged += (sender, args) => CurrentDownloadProgress.CurrentProgress = args.ProgressPercentage;

            /*
             * Execute the video downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            videoDownloader.Execute();
        }

        public static void DownloadAudio(string videoLink, string saveLocation)
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
            var video = videoInfos
                .FirstOrDefault(info => info.Resolution == videoResolution);

            if (video != null && video.RequiresDecryption)
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
