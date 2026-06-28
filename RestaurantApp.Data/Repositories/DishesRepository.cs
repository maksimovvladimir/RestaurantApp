using Npgsql;
using RestaurantApp.Data.Models;

namespace RestaurantApp.Data.Repositories;

public class DishesRepository
{
    public List<DishCurrentPrice> GetMenuWithCurrentPrices()
    {
        var result = new List<DishCurrentPrice>();
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            "SELECT dish_id, name, base_price, discount_percent, current_price FROM v_dish_current_price ORDER BY name", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new DishCurrentPrice
            {
                DishId = reader.GetInt32(0),
                Name = reader.GetString(1),
                BasePrice = reader.GetDecimal(2),
                DiscountPercent = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                CurrentPrice = reader.GetDecimal(4)
            });
        }
        return result;
    }

    public List<StockItem> GetStock()
    {
        var result = new List<StockItem>();
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand("SELECT id, dish_id, quantity_avalible FROM stock ORDER BY dish_id", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new StockItem
            {
                Id = reader.GetInt32(0),
                DishId = reader.GetInt32(1),
                QuantityAvalible = reader.GetInt32(2)
            });
        }
        return result;
    }

    public void AddDiscount(int dishId, string name, decimal percent, DateTime start, DateTime end)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            @"INSERT INTO discounts (dish_id, name, discount_percent, start_date, end_date)
              VALUES (@dish, @name, @pct, @start, @end)
              ON CONFLICT (dish_id) DO UPDATE SET
                name = EXCLUDED.name, discount_percent = EXCLUDED.discount_percent,
                start_date = EXCLUDED.start_date, end_date = EXCLUDED.end_date", conn);
        cmd.Parameters.AddWithValue("dish", dishId);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("pct", percent);
        cmd.Parameters.AddWithValue("start", start.Date);
        cmd.Parameters.AddWithValue("end", end.Date);
        cmd.ExecuteNonQuery();
    }
}
