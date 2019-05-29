using CytubeBotCore.Helpers;
using CytubeBotCore;
using CytubeBotCore.Commands;
using CytubeBotWeb.Data;
using CytubeBotWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocket4Net;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CytubeBotWeb.Helper
{
    public class CytubeBotCoreHelper
    {
        private ChannelModel _channel { get; set; }
        private CytubeBot _bot { get; set; }
        public string _connectionString = Startup.StaticConfig.GetConnectionString("DefaultConnection");

        public CytubeBotCoreHelper(ChannelModel channel, CytubeBot cytubebot)
        {
            _channel = channel;
            _bot = cytubebot;
        }

        public void websocket_MessageReceivedLog(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message.Contains($"42[\"chatMsg\""))
            {
                var ServerMessage = e.Message.Replace("42[\"chatMsg\",", "");
                ServerMessage = ServerMessage.Substring(0, ServerMessage.Length - 1);
                dynamic MessageObj = JsonConvert.DeserializeObject(ServerMessage);
                string messageString = MessageObj.msg;
                DateTime messageTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                double time = MessageObj.time;
                messageTime = messageTime.AddSeconds(time / 1000).ToLocalTime();
                string username = MessageObj.username;


                if (messageString.StartsWith("!"))
                {
                    var commandName = messageString.Split(" ")[0].Substring(1);
                    commandName = char.ToUpper(commandName.First()) + commandName.Substring(1).ToLower();
                    var objectType = Type.GetType($"CytubeBotCore.Commands.{commandName}, CytubeBotCore");

                    if (objectType != null && _bot.Connected)
                    {
                        SaveCommandLogToDb(commandName, username, ServerMessage, messageTime);
                    }
                }
                else if (Youtube.YoutubeUrlInString(messageString))
                {
                    if (_bot.Connected)
                        SaveCommandLogToDb("Youtube", username, ServerMessage, messageTime);
                }
            }
        }

        public async void SaveCommandLogToDb(string command, string user, string message, DateTime messageTime)
        {
            var _context = new ServerDbContext(new DbContextOptionsBuilder<ServerDbContext>().UseSqlServer(_connectionString).Options);

            var existingMessage = await _context.CommandLogs.Where(c => (c.Message == message) && (c.ChannelModelId == _channel.Id)).ToListAsync();
            if (existingMessage.Count == 0)
            {
                var commandLog = new CommandLogsModel
                {
                    Command = command,
                    User = user,
                    Message = message,
                    MessageTime = messageTime,
                    ChannelModelId = _channel.Id
                };
                _context.CommandLogs.Add(commandLog);
                _context.SaveChanges();
            }

            _context.Dispose();
        }
    }
}
