﻿using Dapper;
using Microsoft.Data.Sqlite;

namespace SailsServer.Database
{
    public interface IIgraciProvider
    {
        Task<IEnumerable<Igraci>> Get(string UserID);
    }
    public class IgraciProvider : IIgraciProvider
    {
        private readonly DatabaseConfig databaseConfig;

        public IgraciProvider(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<IEnumerable<Igraci>> Get(string UserID)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            var param = new { userId = UserID };

            return await connection.QueryAsync<Igraci>("SELECT UserID AS UserID, DisplayName, CurrentGame FROM Igraci WHERE UserID = @userId;", param); //todo jednu osobu zgrabi
        }
    }
}

