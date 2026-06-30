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

    /// <summary>
    /// Заказы с человекочитаемой информацией о столике и госте — то, что нужно видеть
    /// и официанту, и кухне, чтобы понимать, какой заказ к какому столику относится.
    /// </summary>
    public List<(Order Order, int TableNumber, string ClientName)> GetOrdersByStatusWithTable(string status)
    {
        var result = new List<(Order, int, string)>();
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            @"SELECT o.id, o.id_reserve_table, o.status, o.created_at, o.confirmed_at, o.total_amount,
                     t.number, r.client_name
              FROM orders o
              JOIN reservation_tables rt ON rt.id_reserve_table = o.id_reserve_table
              JOIN tables t ON t.id_table = rt.id_table
              JOIN reservations r ON r.id_reserve = rt.id_reserve
              WHERE o.status = @status
              ORDER BY o.created_at", conn);
        cmd.Parameters.AddWithValue("status", status);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var order = new Order
            {
                Id = reader.GetInt32(0),
                IdReserveTable = reader.GetInt32(1),
                Status = reader.GetString(2),
                CreatedAt = reader.GetDateTime(3),
                ConfirmedAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                TotalAmount = reader.GetDecimal(5)
            };
            result.Add((order, reader.GetInt32(6), reader.GetString(7)));
        }
        return result;
    }

    /// <summary>Есть ли уже чек по этому заказу (чтобы не пытаться оплатить дважды).</summary>
    public bool HasReceipt(int orderId)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand("SELECT 1 FROM receipts WHERE order_id = @o", conn);
        cmd.Parameters.AddWithValue("o", orderId);
        return cmd.ExecuteScalar() != null;
    }

    public Receipt? GetReceipt(int orderId)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            "SELECT id, order_id, paid_at, amount, payment_methoc FROM receipts WHERE order_id = @o", conn);
        cmd.Parameters.AddWithValue("o", orderId);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        return new Receipt
        {
            Id = reader.GetInt32(0),
            OrderId = reader.GetInt32(1),
            PaidAt = reader.GetDateTime(2),
            Amount = reader.GetDecimal(3),
            PaymentMethoc = reader.GetString(4)
        };
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
