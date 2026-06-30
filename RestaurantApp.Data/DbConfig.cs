using Npgsql;

namespace RestaurantApp.Data;

/// <summary>

/// </summary>
public static class DbConfig
{
    public static string ConnectionString { get; set; } =
        Environment.GetEnvironmentVariable("RESTAURANT_DB_CONNECTION")
        ?? "Host=localhost;Port=5432;Database=restraunt db;Username=postgres;Password=123";

    public static NpgsqlConnection CreateConnection()
    {
        var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}
