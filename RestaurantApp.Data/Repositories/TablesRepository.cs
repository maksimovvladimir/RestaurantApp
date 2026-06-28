using Npgsql;
using RestaurantApp.Data.Models;

namespace RestaurantApp.Data.Repositories;

public class TablesRepository
{
    public List<TableInfo> GetAll()
    {
        var result = new List<TableInfo>();
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand("SELECT id_table, number, capacity, status FROM tables ORDER BY number", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new TableInfo
            {
                IdTable = reader.GetInt32(0),
                Number = reader.GetInt32(1),
                Capacity = reader.GetInt32(2),
                Status = reader.GetString(3)
            });
        }
        return result;
    }

    public void SetStatus(int idTable, string status)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand("UPDATE tables SET status = @status WHERE id_table = @id", conn);
        cmd.Parameters.AddWithValue("status", status);
        cmd.Parameters.AddWithValue("id", idTable);
        cmd.ExecuteNonQuery();
    }
}
