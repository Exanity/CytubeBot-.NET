using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CytubeBotWeb.Models
{
    public class ChannelModel
    {
        public int Id { get; set; }
        public string ChannelName { get; set; }
        public int ServerModelId { get; set; }

        [ForeignKey("ServerModelId")]
        public ServerModel Server { get; set; }
    }
}
