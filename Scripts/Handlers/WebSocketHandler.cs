using Godot;
using Levonin.Scripts;
using Levonin.Scripts.Models.WS.Client;
using Levonin.Scripts.Models.WS.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public partial class WebSocketHandler: Node
{
    public static WebSocketHandler Instance { get; set; }
    private ClientWebSocket _socket = new ClientWebSocket();

    public async Task ConnectAsync(string uri)
    {
        await _socket.ConnectAsync(new Uri(uri), CancellationToken.None);
        await ReceiveLoop();
        
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[1024];
        while (_socket.State == WebSocketState.Open)
        {
            var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
            GD.Print("recieved something");
            try
            {
                var baseObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
                if (baseObj != null && baseObj.ContainsKey("header"))
                {
                    string header = baseObj["header"].ToString();

                    switch (header)
                    {
                        case "InitialConnectionResponse":
                            var initResp = JsonConvert.DeserializeObject<InitialConnectionResponse>(msg);
                            GD.Print("Init success: " + initResp.success);
                            break;

                        case "ChannelReRender":
                            var channelMsg = JsonConvert.DeserializeObject<ChannelReRender>(msg);
                            GD.Print("Re-render channel: " + channelMsg.channel);
                            EventHandler.Instance.CallEvent("reRenderChannel", null);
                            break;

                        case "MessageInChannelResponse":
                            var msgResp = JsonConvert.DeserializeObject<MessageInChannelResponse>(msg);
                            GD.Print("Message sent ok: " + msgResp.success);
                            break;

                        default:
                            GD.Print("Unknown header: " + header);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr("JSON parse error: " + ex.Message);
            }
        }
    }


    public async Task Send(string message)
    {
        GD.Print($"Sending... {message} to server");
        var bytes = Encoding.UTF8.GetBytes(message);
        await _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public override void _Ready()
    {
        if(Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;
        _ = ConnectAsync("wss://levonin-websocket.aleksandermilisenko23.thkit.ee");
    }
}
