using Npgsql;
using RestaurantApp.Data.Models;

namespace RestaurantApp.Data.Repositories;

public class ReservationsRepository
{
    /// <summary>
    /// Создаёт запись о посещении (бронь либо "живой" гость без предзаказа — оба случая
    /// заносит администратор/официант, в системе нет отдельного клиентского входа).
    /// </summary>
    public int CreateReservation(string clientName, string? clientPhone, DateTime resDate,
        TimeSpan timeStart, TimeSpan timeEnd, int guestsCount)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            @"INSERT INTO reservations (client_name, client_phone, res_date, time_start, time_end, guests_count)
              VALUES (@name, @phone, @date, @ts, @te, @guests) RETURNING id_reserve", conn);
        cmd.Parameters.AddWithValue("name", clientName);
        cmd.Parameters.AddWithValue("phone", clientPhone ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("date", resDate.Date);
        cmd.Parameters.AddWithValue("ts", timeStart);
        cmd.Parameters.AddWithValue("te", timeEnd);
        cmd.Parameters.AddWithValue("guests", guestsCount);
        return (int)cmd.ExecuteScalar()!;
    }

    /// <summary>Привязывает столик к брони. Триггер в БД сам проверит вместимость.</summary>
    public int AttachTable(int idReserve, int idTable)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            @"INSERT INTO reservation_tables (id_reserve, id_table)
              VALUES (@r, @t) RETURNING id_reserve_table", conn);
        cmd.Parameters.AddWithValue("r", idReserve);
        cmd.Parameters.AddWithValue("t", idTable);
        return (int)cmd.ExecuteScalar()!;
    }

    /// <summary>
    /// Список пар "бронь+столик", по которым ещё можно создать заказ — с человекочитаемым
    /// описанием (имя гостя, номер столика), чтобы не вводить id_reserve_table руками.
    /// </summary>
    public List<(int IdReserveTable, string Description)> GetReservationTablesForOrder()
    {
        var result = new List<(int, string)>();
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            @"SELECT rt.id_reserve_table, r.client_name, t.number, r.res_date, r.time_start
              FROM reservation_tables rt
              JOIN reservations r ON r.id_reserve = rt.id_reserve
              JOIN tables t ON t.id_table = rt.id_table
              WHERE r.status = 'active'
              ORDER BY r.res_date DESC, r.time_start DESC", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var idReserveTable = reader.GetInt32(0);
            var clientName = reader.GetString(1);
            var tableNumber = reader.GetInt32(2);
            var date = reader.GetDateTime(3);
            var time = reader.GetTimeSpan(4);
            result.Add((idReserveTable,
                $"{clientName} — столик №{tableNumber} ({date:dd.MM} {time:hh\\:mm})"));
        }
        return result;
    }

    public List<Reservation> GetActive()
    {
        var result = new List<Reservation>();
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            @"SELECT id_reserve, client_name, client_phone, res_date, time_start, time_end, guests_count, status
              FROM reservations WHERE status = 'active' ORDER BY res_date, time_start", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Reservation
            {
                IdReserve = reader.GetInt32(0),
                ClientName = reader.GetString(1),
                ClientPhone = reader.IsDBNull(2) ? null : reader.GetString(2),
                ResDate = reader.GetDateTime(3),
                TimeStart = reader.GetTimeSpan(4),
                TimeEnd = reader.GetTimeSpan(5),
                GuestsCount = reader.GetInt32(6),
                Status = reader.GetString(7)
            });
        }
        return result;
    }
}
