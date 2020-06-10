using System;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DockerDotnetTestExample
{
    public static class DatabaseFactory
    {
        public static async Task CreateDb(SqlConnectionStringBuilder connectionStringBuilder)
        {
            var intermediateConnectionStringBuilder = new SqlConnectionStringBuilder(connectionStringBuilder.ConnectionString);

            intermediateConnectionStringBuilder.InitialCatalog = "master";

            var conn = intermediateConnectionStringBuilder.ConnectionString;

            //throw new Exception(conn);

            using(var sqlConn = new SqlConnection(conn))
            {
                await sqlConn.OpenAsync();

                using(var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = $"CREATE DATABASE {connectionStringBuilder.InitialCatalog}";

                    await cmd.ExecuteNonQueryAsync();
                }

            }
        }
    }
}