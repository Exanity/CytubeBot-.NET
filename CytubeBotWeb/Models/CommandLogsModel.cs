using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CytubeBotWeb.Models
{
    public class CommandLogsModel
    {
        public int Id { get; set; }
        public string Command { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
        public DateTime MessageTime { get; set; }

        public int ChannelModelId { get; set; }

        [ForeignKey("ChannelModelId")]
        public virtual ChannelModel Channel { get; set; }
    }
}
