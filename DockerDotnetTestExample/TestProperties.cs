using System;
using System.Data.SqlClient;

namespace DockerDotnetTestExample
{
    public class TestProperties
    {
        public const string Port = "1450";

        public const string DBName = "DockerDotnet";

        public const string Password = "Development!";

        public const string ContainerName = "MyTestContainer";

        public static readonly string ConnectionString = 
            $"Server=tcp:localhost,{Port};" + 
            $"Initial Catalog={DBName};" + 
            $"User ID=sa;" + 
            $"Password={Password};" + 
            //$"Persist Security Info=False;" + 
            //$"MultipleActiveResultSets=False;" + 
            //$"Encrypt=True;" + 
            $"TrustServerCertificate=True;" + 
            $"Connection Timeout=30";

        public static readonly SqlConnectionStringBuilder SqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);  
    }
}