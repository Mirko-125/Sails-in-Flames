using Microsoft.Data.Sqlite;
using Dapper;

namespace SailsServer.Database
{
    public interface IDatabaseBootstrap
    {
        void Setup();
    }

    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig databaseConfig;

        public DatabaseBootstrap(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public void Setup()
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            var table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'Igraci';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "Igraci")
                return;

            connection.Execute("Create Table Igraci (" +
                "UserID VARCHAR(100) PRIMARY KEY," +
                "DisplayName VARCHAR(100) NOT NULL," + 
                "CurrentGame VARCHAR(100) NULL);");

            connection.Execute("Create Table Igre (" +
                "GameID VARCHAR(100) PRIMARY KEY," +
                "Weapon1 VARCHAR(100) NOT NULL," +
                "Weapon2 VARCHAR(100) NOT NULL," +
                "BoardState1 VARCHAR(500) NOT NULL," +
                "BoardState2 VARCHAR(500) NOT NULL," +
                "GameState VARCHAR(5) NOT NULL," +
                "Player1 VARCHAR(100) NOT NULL," +
                "Player2 VARCHAR(100) NOT NULL);");
        }
    }
}
