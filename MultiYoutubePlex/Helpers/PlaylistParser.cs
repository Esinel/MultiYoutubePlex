using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace MultiYoutubePlex.Helpers
{
    /// <summary>
    /// Static class for parser methods. 
    /// TODO: These should be revorked into agent/factory model(together with Downloader).
    /// TODO: Make parsiong of the links c# only solution (Make a python version as well? hmm..)
    /// </summary>
    public static class PlaylistParser
    {
        
        public static string[] ParsePlayList(string url)
        {
            var playListParserPythonScript =
                File.ReadAllText(ConfigurationManager.AppSettings["PlaylistParserPath"]);
            
            // host python and execute script
            var engine = IronPython.Hosting.Python.CreateEngine();
            var scope = engine.CreateScope();
            engine.Execute(playListParserPythonScript, scope);
            
            var crawl = scope. GetVariable<Func<string, string, string>>("crawl");
            var html = GetHtml(url);
            var listWithNewLineDelimiter = crawl(url, html);

            var youtubeUrList = listWithNewLineDelimiter.Split('\n');

            return youtubeUrList;
        }

        public static string GetHtml(string input)
        {
            string html;
            var url = $@"{input}";

            if (!url.Contains("http"))
            {
                url = @"http://" + input;
            }
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            return html;
        }
    }
}
