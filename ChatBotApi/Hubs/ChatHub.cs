using ChatBotApi.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;                                         // using this
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBotApi.Hubs
{
    public class ChatHub:Hub<IChatClient>
    {

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        public override async Task OnConnectedAsync()
        {
         
            await Clients.All.ReceiveMessage( "System", $"{Context.ConnectionId} has joined");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.ReceiveMessage("System", $"{Context.ConnectionId} has left");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
