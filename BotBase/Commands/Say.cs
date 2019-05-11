using System;
using System.Collections.Generic;
using System.Text;

namespace CytubeBotCore.Commands
{
    class Say : Command
    {

        public override void Execute()
        {
            var stringArr = MessageObject.msg.ToString().Split(" ");
            var SendString = string.Join(" ", stringArr, 1, stringArr.Length - 1);
            CytubeBotObject.SendMessage(SendString);
        }
    }
}
