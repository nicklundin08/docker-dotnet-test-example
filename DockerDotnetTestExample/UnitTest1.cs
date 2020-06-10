using System;
using Xunit;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DockerDotnetTestExample
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            await ContainerFactory.StartContainer();

            await DatabaseFactory.CreateDb(TestProperties.SqlConnectionStringBuilder);

            using(var sqlConn = new SqlConnection(TestProperties.SqlConnectionStringBuilder.ConnectionString))
            {
                await sqlConn.OpenAsync();

                using(var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = $"CREATE TABLE Foo (bar int, baz int)";

                    await cmd.ExecuteNonQueryAsync();
                }

            }
        }
    }
}