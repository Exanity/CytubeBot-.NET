using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using WebSocket4Net;
using System.Threading.Tasks;
using System.Timers;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CytubeBotCore.Helpers;

namespace CytubeBotCore
{
    public class CytubeBot
    {
        public string Server;
        public int Port;
        public string Channel;
        public string Username;
        private string Password;
        public WebSocket websocket;
        private Timer KeepAliveTimer;
        private Timer ResetConnectionTimer;
        public bool Connected = false;

        public CytubeBot(string _server, string _channel, string _username, string _password)
        {
            Server = _server;
            Channel = _channel;
            Username = _username;
            Password = _password;

            BuildWebsocketObject();
        }

        public void BuildWebsocketObject()
        {
            websocket = new WebSocket(getSocketURL(new Uri(Server)));
            websocket.Opened += websocket_Opened;
            websocket.Error += websocket_Error;
            websocket.Closed += websocket_Closed;
            websocket.MessageReceived += websocket_MessageReceived;
            websocket.EnableAutoSendPing = true;
            websocket.AutoSendPingInterval = 25;
        }

        public void Connect()
        {
            Console.WriteLine("Starting Connection to Socket.io server...");

            websocket.Open();
        }

        public void Disconnect()
        {
            websocket.Close();
            websocket.Dispose();
        }

        private void keep_open(object sender, ElapsedEventArgs e)
        {
            this.Emit("", null);
        }

        private void try_reconnect(object sender, ElapsedEventArgs e)
        {
            websocket.Open();
        }

        public string getSocketURL(Uri uri) {

            /* 
             * TODO
             * Make it so you fetch the correct url based on if its http or https
             */

            string returnString = "";
            string scheme = uri.Scheme;
            string wssScheme = (scheme == "http") ? "ws" : "wss";
            string socketIOPath = "socket.io/?EIO=3&transport=websocket";

            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync($"{scheme}://{uri.Authority}/socketconfig/{Channel}.json").Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;


                    dynamic dynJson = JsonConvert.DeserializeObject(responseString);
                    Uri socketUrl = null;
                    foreach (var item in dynJson.servers)
                    {
                        if (socketUrl != null) continue;

                        string itemUrl = item.url;

                        if (scheme == "https" && item.secure == "true")
                        {
                            socketUrl = new Uri(itemUrl);
                        }
                        else
                        {
                            socketUrl = new Uri(item.url);
                        }
                    }

                    returnString = $"{wssScheme}://{socketUrl.Authority}/{socketIOPath}";
                }

            }

            return returnString;
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Connected!");
            this.Emit("initChannelCallbacks", null);
            this.Emit("joinChannel", "{ \"name\": \""+Channel+"\"}");
            this.Emit("login", $"{{ \"name\": \"{Username}\", \"pw\": \"{Password}\"}}");

            if(ResetConnectionTimer != null) ResetConnectionTimer.Stop();
            KeepAliveTimer = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds); // Set the time (5 mins in this case)
            KeepAliveTimer.AutoReset = true;
            KeepAliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(keep_open);
            KeepAliveTimer.Start();
        }
        private void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine("WebSocket Error");
            Console.WriteLine(e.Exception.Message);
            KeepAliveTimer.Stop();

            ResetConnectionTimer = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds); // Set the time (5 mins in this case)
            ResetConnectionTimer.AutoReset = true;
            ResetConnectionTimer.Elapsed += new System.Timers.ElapsedEventHandler(try_reconnect);
            ResetConnectionTimer.Start();
        }
        private void websocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection Closed...");
            Console.WriteLine(e);
            // Add Reconnect logic... this.Start()
        }
        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message.Contains($"42[\"setUserRank\",{{\"name\":\"{Username}\""))
            {
                this.Connected = true;
            }
            // Remove this if you don't want the bot to reconnect on kick
            else if (e.Message.Contains($"42[\"kick\""))
            {
                this.Disconnect();
                BuildWebsocketObject();
                this.Connect();
            }

            if (this.Connected)
            {
                if (e.Message.Contains($"42[\"chatMsg\""))
                {
                    var ServerMessage = e.Message.Replace("42[\"chatMsg\",", "");
                    ServerMessage = ServerMessage.Substring(0, ServerMessage.Length - 1);
                    dynamic MessageObj = JsonConvert.DeserializeObject(ServerMessage);
                    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    double time = MessageObj.time;
                    origin = origin.AddSeconds(time / 1000).ToLocalTime();
                    string messageString = MessageObj.msg;

                    if (messageString.StartsWith("!"))
                    {
                        var commandName = messageString.Split(" ")[0].Substring(1);
                        commandName = char.ToUpper(commandName.First()) + commandName.Substring(1).ToLower();
                        var objectType = Type.GetType($"CytubeBotCore.Commands.{commandName}");

                        if (objectType != null)
                        {
                            dynamic command = Activator.CreateInstance(objectType);
                            command.MessageString = e.Message;
                            command.MessageObject = MessageObj;
                            command.Username = MessageObj.username.ToString();
                            command.CytubeBotObject = this;
                            command.Execute();
                        }
                    }
                    else if (Youtube.YoutubeUrlInString(messageString))
                    {
                        string title = Youtube.GetTitleFromMessage(messageString);
                        SendMessage("Youtube video: " + title);
                    }

                    var ConsoleMessage = $"[{origin.ToString("HH:mm:ss")}]{MessageObj.username}: {MessageObj.msg}";
                    Console.WriteLine(ConsoleMessage);
                }
            }
            //Console.WriteLine(e.Message);
        }

        private void Emit(string eventName, string jsonObj)
        {
            if (jsonObj != null) jsonObj = "," + jsonObj;
            websocket.Send($"42[\"{eventName}\"{jsonObj}]");
        }
        
        public void SendMessage(string msg)
        {
            var test = HttpUtility.JavaScriptStringEncode(msg);
            this.Emit("chatMsg", "{ \"msg\": \""+ test+ "\", \"meta\": {}}");
        }
    }
}
