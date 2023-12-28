using Dapper;
using Microsoft.Data.Sqlite;

namespace SailsServer.Database
{
    public interface IIgreProvider
    {
        Task<IEnumerable<Igre>> Get(string GameID);
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

            return await connection.QueryAsync<Igre>("SELECT GameID AS GameID, BoardState1, BoardState2, GameState, Player1, Player2 FROM Igre WHERE GameID = @gameId;", param);
        }
    }
}

