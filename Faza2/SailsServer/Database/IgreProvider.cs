using Dapper;
using Microsoft.Data.Sqlite;

namespace SailsServer.Database
{
    public interface IIgreProvider
    {
        Task<IEnumerable<Igre>> Get(string GameID);
        Task<IEnumerable<Igre>> GetActiveUser(string UserID);
    }
    public class IgreProvider : IIgreProvider
    {
        private readonly DatabaseConfig databaseConfig;

        public IgreProvider(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<IEnumerable<Igre>> Get(string GameID)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            var param = new { gameId = GameID };

            return await connection.QueryAsync<Igre>("SELECT GameID AS GameID, Weapon1, Weapon2, BoardState1, BoardState2, GameState, Player1, Player2 FROM Igre WHERE GameID = @gameId;", param);
        }

        public async Task<IEnumerable<Igre>> GetActiveUser(string UserID)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            var param = new { userId = UserID };

            return await connection.QueryAsync<Igre>("SELECT GameID AS GameID, Weapon1, Weapon2, BoardState1, BoardState2, GameState, Player1, Player2 FROM Igre WHERE (Player1 = @userId AND GameState <> '0') OR (Player2 = @userId AND GameState <> '0');", param);
        }
    }
}

