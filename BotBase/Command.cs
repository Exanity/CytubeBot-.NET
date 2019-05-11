using System;
using System.Collections.Generic;
using System.Text;

namespace CytubeBotCore
{
    abstract class Command
    {
        public string Username { get; set; }
        public string MessageString { get; set; }
        public dynamic MessageObject { get; set; }
        public CytubeBot CytubeBotObject { get; set; }
        public dynamic Parameters;

        public abstract void Execute();
    }
}
