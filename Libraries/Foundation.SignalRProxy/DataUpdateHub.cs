using Microsoft.AspNetCore.SignalR;

namespace Foundation.SignalRRelay;

public class ObjectUpdateHub : Hub
{
    public async Task SendMessage(string type, string json)
    {
        await Clients.All.SendAsync("ObjectUpdate", type, json);
    }

    public async Task SendMessge(string connectionId, string type, string json)
    {
        await Clients.Client(connectionId).SendAsync("ReceiveMessage", type, json);
    }

    //public override async Task OnConnectedAsync()
    //{
    //    await base.OnConnectedAsync();
    //    await Clients.Caller.SendAsync("Notify", "You are connected to the ForwardingHub.");
    //}

    //public override async Task OnDisconnectedAsync(System.Exception exception)
    //{
    //    await base.OnDisconnectedAsync(exception);
    //}
}
