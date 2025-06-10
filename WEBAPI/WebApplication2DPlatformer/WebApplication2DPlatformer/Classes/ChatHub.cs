using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private static Dictionary<string, string> _userConnections = new();

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext().Request.Query["userId"];
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections[Context.ConnectionId] = userId;
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        _userConnections.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string user, string message)
    {
        Console.WriteLine($"Сервер получил: {user}: {message}");
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendPrivateMessage(string targetUserId, string message)
    {
        var senderId = _userConnections[Context.ConnectionId];
        await Clients.User(targetUserId).SendAsync("ReceivePrivateMessage", $"Player {senderId}", message);
    }
}