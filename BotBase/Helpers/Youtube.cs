using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CytubeBotCore.Helpers
{
    class Youtube
    {
        private Regex regexString = new Regex(@"(?<=href=\"")((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)(?=\"")");

        public bool YoutubeUrlInString(string text)
        {
            return regexString.IsMatch(text);
        }

        public string GetTitleFromMessage(string messageString)
        {
            string name = "";
            if (YoutubeUrlInString(messageString))
            {
                foreach (Match match in regexString.Matches(messageString))
                {
                    string url = match.Value;
                    name = GetTitle(url);
                }
            }

            return name;
        }

        public static string GetTitle(string url)
        {
            var api = $"http://youtube.com/get_video_info?video_id={GetArgs(url, "v", '?')}";
            return GetArgs(new WebClient().DownloadString(api), "title", '&');
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
