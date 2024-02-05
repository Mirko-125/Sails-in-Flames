using Microsoft.AspNetCore.SignalR;
using SailsServer.Database;

namespace SailsServer
{
    public class GameHub : Hub
    {
        // Add methods for game-related actions

        DBManager dbManager;

        public GameHub()
        {
            dbManager = DBManager.Instance;
        }

        public async Task ConnectAs(string user)
        {
            Console.WriteLine("First time client connecting with username " + user);
            await dbManager.ConnectWithUser(user, Clients.Caller);
        }

        public async Task ConnectId(string id)
        {
            Console.WriteLine("Existing client connecting with id " + id);
            await dbManager.ConnectExisting(id, Clients.Caller);
        }

        public async Task LookForGame(string id)
        {
            Console.WriteLine("Client " + id + " trying to find game...");
            await dbManager.FindGame(id);
        }

        public async Task AcceptWeapons(string id, string weapons)
        {
            Console.WriteLine("Client " + id + " submitted weapon list: " + weapons);
            await dbManager.AcceptWeapon(id, weapons);
        }

        public async Task AcceptPlacement(string id, string boardState)
        {
            Console.WriteLine("Client " + id + " submitted board state: " + boardState);
            await dbManager.AcceptBoard(id, boardState);
        }

        public async Task PerformMove(string id, int weapon, int row, int col)
        {
            Console.WriteLine("Client " + id + " submitted move: " + row.ToString() + ", " + col.ToString() + " - " + weapon.ToString());
            await dbManager.PlayMove(id, row, col, weapon);
        }


    }
}
