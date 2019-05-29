using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CytubeBotCore.Helpers
{
    static public class Youtube
    {
        private static readonly Regex regexUrl = new Regex(@"(?<=href=\"")((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)(?=\"")");
        private static readonly Regex regexId = new Regex(@"(?:youtu.be\/|v\/|embed\/|watch\?v=)([^#&?]*)");

        public static bool YoutubeUrlInString(string text)
        {
            return regexUrl.IsMatch(text);
        }

        public static string GetTitleFromMessage(string messageString)
        {
            string name = "";
            if (YoutubeUrlInString(messageString))
            {
                foreach (Match match in regexUrl.Matches(messageString))
                {
                    string url = match.Value;
                    name = GetTitle(url);
                }
            }

            return name;
        }

        public static string GetTitle(string url)
        {
            var id = regexId.Matches(url);
            if(id.Count > 0)
            {
                foreach (Match match in id)
                {
                    var api = $"http://youtube.com/get_video_info?video_id={match.Groups[1]}";
                    return GetArgs(new WebClient().DownloadString(api), "title", '&');
                }

                return "Error something went wrong!";
            } else
            {
                return "Error something went wrong!";
            }
        }

        private static string GetArgs(string args, string key, char query)
        {
            var iqs = args.IndexOf(query);
            return iqs == -1
                ? string.Empty
                : HttpUtility.ParseQueryString(iqs < args.Length - 1
                    ? args.Substring(iqs + 1) : string.Empty)[key];
        }
    }
}
