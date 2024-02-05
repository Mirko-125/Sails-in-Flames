using Microsoft.Data.Sqlite;
using Dapper;

namespace SailsServer.Database
{
    public interface IIgreRepository
    {
        Task Create(Igre igra);
        Task Update(Igre igra);
    }
    public class IgreRepository : IIgreRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public IgreRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task Create(Igre igra)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            await connection.ExecuteAsync("INSERT INTO Igre (GameID, Weapon1, Weapon2, BoardState1, BoardState2, GameState, Player1, Player2)" +
                "VALUES (@GameID, @Weapon1, @Weapon2, @BoardState1, @BoardState2, @GameState, @Player1, @Player2);", igra);
        }

        public async Task Update(Igre igra)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            await connection.ExecuteAsync("UPDATE Igre SET Weapon1 = @Weapon1, Weapon2 = @Weapon2, BoardState1 = @BoardState1, BoardState2 = @BoardState2, GameState = @GameState, Player1 = @Player1, Player2 = @Player2 " +
                "WHERE GameID = @GameID;", igra);
        }
    }
}