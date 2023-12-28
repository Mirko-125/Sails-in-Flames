using Microsoft.Data.Sqlite;
using Dapper;

namespace SailsServer.Database
{
    public interface IIgraciRepository
    {
        Task Create(Igraci igrac);
        Task Update(Igraci igrac);
    }
    public class IgraciRepository : IIgraciRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public IgraciRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task Create(Igraci igrac)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            await connection.ExecuteAsync("INSERT INTO Igraci (UserID, DisplayName, CurrentGame)" +
                "VALUES (@UserID, @DisplayName, @CurrentGame);", igrac);
        }

        public async Task Update(Igraci igrac)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            await connection.ExecuteAsync("UPDATE Igraci SET DisplayName = @DisplayName, CurrentGame = @CurrentGame " +
                "WHERE UserID = @UserID;", igrac);
        }
    }
}
