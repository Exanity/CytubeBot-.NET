using CytubeBotWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CytubeBotWeb.ViewModels
{
    public class ServerChannelViewModel
    {
        public ServerModel Server { get; set; }
        public ICollection<ChannelModel> Channels { get; set; }
        public ChannelModel Channel { get; set; }
    }
}
