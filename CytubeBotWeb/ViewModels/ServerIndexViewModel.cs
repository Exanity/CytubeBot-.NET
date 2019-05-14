using CytubeBotCore;
using CytubeBotWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CytubeBotWeb.ViewModels
{
    public class ServerIndexViewModel
    {
        public List<ServerModel> Servers { get; set; }
        public List<CytubeBot> RunningBots { get; set; }
    }
}
