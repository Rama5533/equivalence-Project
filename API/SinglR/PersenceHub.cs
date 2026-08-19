using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

//real time communication(الـ Backend يقدر يبعث التحديث مباشرة للـ Frontend لحظة حدوثه.)

namespace API.SinglR;

[Authorize]
public class PersenceHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SendAsync("UserOnline", Context.User?.FindFirstValue(ClaimTypes.Email));//the client is going to listen for to receive the notifications
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.SendAsync("UserOffline",
            Context.User?.FindFirstValue(ClaimTypes.Email));


        await base.OnDisconnectedAsync(exception);
    }
}
