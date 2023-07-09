using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Game
{
    public class Server
    {
        public readonly Map CurrentMap;
        public  ClientWebSocket Client;
        public readonly byte[] Buffer;
        public Server(Map map)
        {
            Buffer = new byte[65535];
            CurrentMap = map;
        }

        public async Task<string> Initialize(InitializeInfo info)
        {
            Client = new ClientWebSocket();
           
                await Client?.ConnectAsync(new Uri("ws://192.168.0.109:78"), CancellationToken.None);
                var data = JsonConvert.SerializeObject(info);
                var bytes = Encoding.UTF8.GetBytes(data);
                //await Client?.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                //var response = await Client.ReceiveAsync(new ArraySegment<byte>(Buffer), CancellationToken.None);
                //var text = Encoding.UTF8.GetString(Buffer, 0, response.Count);
                //InitializeInfo fromServer = JsonConvert.DeserializeObject<InitializeInfo>(text);
                //if (info.Initialized)
                //{
                //    CurrentMap.MultiplyMap(fromServer.Map);
                //    foreach (var player in fromServer.Players)
                //    {
                //        CurrentMap.AddNewPlayer(player);
                //    }
                //}

                //else
                //{
                //    CurrentMap.IsOnline = true;
                //}
                //info.Players[0].IsOnline = true;
                ReceiveInfo();
                return "Succesfully connected";
            return "Problems with server";
        }

        public async Task SendInfo(Info info)
        {
            if(info is MoveInfo)
            {
                var temp = (MoveInfo)info;
                var data = JsonConvert.SerializeObject(temp);
                var bytes = Encoding.UTF8.GetBytes(data);
                await Client?.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if(info is HpInfo)
            {
                var temp = (HpInfo)info;
                var data = JsonConvert.SerializeObject(temp);
                var bytes = Encoding.UTF8.GetBytes(data);
                await Client?.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        public async Task ReceiveInfo()
        {
            while(true)
            {
                var result = await Client?.ReceiveAsync(new ArraySegment<byte>(Buffer), CancellationToken.None);
                var text = Encoding.UTF8.GetString(Buffer, 0, result.Count).ToLower();
                if(text.IndexOf("moveinfo") != -1)
                {
                    var data = JsonConvert.DeserializeObject<MoveInfo>(text);
                    CurrentMap.SetNewLocationFromNet(data);
                }
                else if(text.IndexOf("hpinfo") != -1)
                {
                    var data = JsonConvert.DeserializeObject<HpInfo>(text);
                   CurrentMap.SetNewHpFromNet(data);
                }
            }
        }
    }
}
