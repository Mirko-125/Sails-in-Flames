using Microsoft.AspNet.SignalR;
namespace SailsServer
{
    public class GameHub : Hub
    {
        // Add methods for game-related actions

        public void SendGameUpdate(string message)
        {
            Clients.All.updateGame(message);
        }
    }
}
