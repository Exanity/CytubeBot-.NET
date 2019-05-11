using System;

namespace CytubeBotCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var Server = Environment.GetEnvironmentVariable("server:Host");
            var Channel = Environment.GetEnvironmentVariable("server:Channel");
            var Username = Environment.GetEnvironmentVariable("server:Username");
            var Password = Environment.GetEnvironmentVariable("server:Password");


            CytubeBot cytubebot = new CytubeBot(Server, Channel, Username, Password);
            cytubebot.Connect();
            bool CytubeRunning = true;

            while (CytubeRunning)
            {
                var Input = Console.ReadLine();
                if (Input == "/exit") {
                    CytubeRunning = false;
                }
                else
                {
                    cytubebot.SendMessage(Input);
                }
            }

        }
    }
}
