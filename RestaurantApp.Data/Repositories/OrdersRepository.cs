using Npgsql;
using RestaurantApp.Data.Models;

namespace RestaurantApp.Data.Repositories;

public class OrdersRepository
{
    public int CreateDraftOrder(int idReserveTable)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            "INSERT INTO orders (id_reserve_table) VALUES (@rt) RETURNING id", conn);
        cmd.Parameters.AddWithValue("rt", idReserveTable);
        return (int)cmd.ExecuteScalar()!;
    }

    /// <summary>
    /// Добавляет блюдо в заказ через sp_add_dish_to_order.
    /// Возвращает текстовое сообщение из БД — его и нужно показать в MessageBox,
    /// будь то успех или "недостаточно порций на складе".
    /// </summary>
    public string AddDishToOrder(int orderId, int dishId, int qty)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand("SELECT sp_add_dish_to_order(@o, @d, @q)", conn);
        cmd.Parameters.AddWithValue("o", orderId);
        cmd.Parameters.AddWithValue("d", dishId);
        cmd.Parameters.AddWithValue("q", qty);
        return (string)cmd.ExecuteScalar()!;
    }

    public string RemoveDishFromOrder(int orderId, int dishId)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand("SELECT sp_remove_dish_from_order(@o, @d)", conn);
        cmd.Parameters.AddWithValue("o", orderId);
        cmd.Parameters.AddWithValue("d", dishId);
        return (string)cmd.ExecuteScalar()!;
    }

    /// <summary>
    /// Меняет статус заказа через sp_change_order_status.
    /// Бросает NpgsqlException с понятным текстом, если переход запрещён
    /// (например, "оформлен" -> "составление") — это нужно поймать в UI и показать пользователю.
    /// </summary>
    public void ChangeStatus(int orderId, string newStatus)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand("SELECT sp_change_order_status(@o, @s)", conn);
        cmd.Parameters.AddWithValue("o", orderId);
        cmd.Parameters.AddWithValue("s", newStatus);
        cmd.ExecuteNonQuery();
    }

    public List<Order> GetOrdersByStatus(string status)
    {
        var result = new List<Order>();
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            @"SELECT id, id_reserve_table, status, created_at, confirmed_at, total_amount
              FROM orders WHERE status = @status ORDER BY created_at", conn);
        cmd.Parameters.AddWithValue("status", status);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Order
            {
                Id = reader.GetInt32(0),
                IdReserveTable = reader.GetInt32(1),
                Status = reader.GetString(2),
                CreatedAt = reader.GetDateTime(3),
                ConfirmedAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                TotalAmount = reader.GetDecimal(5)
            });
        }
        return result;
    }

    public List<(OrderItem Item, string DishName)> GetOrderItems(int orderId)
    {
        var result = new List<(OrderItem, string)>();
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            @"SELECT oi.id, oi.order_id, oi.dish_id, oi.quantity, d.name
              FROM order_items oi JOIN dishes d ON d.id = oi.dish_id
              WHERE oi.order_id = @o", conn);
        cmd.Parameters.AddWithValue("o", orderId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add((new OrderItem
            {
                Id = reader.GetInt32(0),
                OrderId = reader.GetInt32(1),
                DishId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3)
            }, reader.GetString(4)));
        }
        return result;
    }

    public void CreateReceipt(int orderId, string paymentMethoc)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            "INSERT INTO receipts (order_id, payment_methoc) VALUES (@o, @m)", conn);
        cmd.Parameters.AddWithValue("o", orderId);
        cmd.Parameters.AddWithValue("m", paymentMethoc);
        cmd.ExecuteNonQuery();
    }
}
