using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ChatBotApi.Hubs;
using ChatBotApi.Models;

namespace ChatBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [Route("send")]                                           //path looks like this: https://localhost:5000/api/chat/send
        [HttpPost]
        public IActionResult SendRequest( MessageDto msg)
        {
            try
            {
                _hubContext.Clients.All.SendAsync("ReceiveOne", msg.user, msg.msgText);
                return Ok("ReceiveOne");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "An error occurred while sending the message.");
            }
        }
    }
}
