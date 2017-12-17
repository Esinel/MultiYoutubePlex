using System;
using System.Configuration;
namespace MultiYoutubePlex.Helpers
{
    public static class PlaylistParser
    {
        public static string[] ParsePlayList(string url)
        {
            var playListParserPythonScript =
                System.IO.File.ReadAllText(ConfigurationManager.AppSettings["PlaylistParserPath"]);
            
            // host python and execute script
            var engine = IronPython.Hosting.Python.CreateEngine();
            var scope = engine.CreateScope();
            engine.Execute(playListParserPythonScript, scope);
            
            var crawl = scope.GetVariable<Func<string, string>>("crawl");
            var listWithNewLineDelimiter = crawl(url);

            var youtubeUrList = listWithNewLineDelimiter.Split('\n');

            return youtubeUrList;
        }
    }
}
