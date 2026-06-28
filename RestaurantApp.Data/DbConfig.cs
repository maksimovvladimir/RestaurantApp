using Npgsql;

namespace RestaurantApp.Data;

/// <summary>
/// Центральная точка подключения к PostgreSQL.
/// Строку подключения меняешь здесь либо через переменную окружения RESTAURANT_DB_CONNECTION.
/// </summary>
public static class DbConfig
{
    public static string ConnectionString { get; set; } =
        Environment.GetEnvironmentVariable("RESTAURANT_DB_CONNECTION")
        ?? "Host=localhost;Port=5432;Database=restaurant_v2;Username=postgres;Password=postgres";

    public static NpgsqlConnection CreateConnection()
    {
        var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}
