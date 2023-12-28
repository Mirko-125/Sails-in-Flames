using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;

namespace SailsServer.Controllers
{
    public class GameController : ControllerBase
    {
        private readonly IHubContext _hubContext;

        public GameController()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();
        }

        // Your existing API methods

        // Example of sending game updates to clients
        private void SendGameUpdateToClients(string message)
        {
            _hubContext.Clients.All.updateGame(message);
        }
    }
}
