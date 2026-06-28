using Npgsql;

namespace RestaurantApp.UI;

/// <summary>
/// Раздел статистики из задания: продажи по категориям, брони по столикам,
/// занятость по часам. Использует прямые SQL-запросы (см. БД_3НФ_и_логика.md, раздел 7),
/// адаптированные под текущую схему (без price_at_order — цена считается через v_dish_current_price).
/// TODO следующая итерация: отчёт по официантам — требует join через смены/Зарезервированные_столики,
/// т.к. в этой версии схемы Заказ не хранит waiter_id напрямую.
/// </summary>
public partial class StatisticsForm : Form
{
    public StatisticsForm()
    {
        InitializeComponent();
    }

    private void btnSalesByCategory_Click(object sender, EventArgs e)
    {
        const string sql = @"
            SELECT c.name AS category, d.name AS dish_name, SUM(oi.quantity) AS qty_sold
            FROM order_items oi
            JOIN dishes d ON d.id = oi.dish_id
            JOIN dish_categories c ON c.id = d.category_id
            JOIN orders o ON o.id = oi.order_id
            WHERE o.created_at >= date_trunc('month', CURRENT_DATE) - INTERVAL '1 month'
            GROUP BY category, dish_name
            ORDER BY category, dish_name";
        RunReport(sql, new[] { "Категория", "Блюдо", "Продано, шт." });
    }

    private void btnReservationsByTable_Click(object sender, EventArgs e)
    {
        const string sql = @"
            SELECT t.number AS table_number, COUNT(*) AS reservations_count
            FROM reservation_tables rt
            JOIN tables t ON t.id_table = rt.id_table
            JOIN reservations r ON r.id_reserve = rt.id_reserve
            WHERE date_trunc('month', r.res_date) = date_trunc('month', CURRENT_DATE)
            GROUP BY t.number
            ORDER BY reservations_count DESC";
        RunReport(sql, new[] { "Столик №", "Кол-во броней" });
    }

    private void btnTableOccupancy_Click(object sender, EventArgs e)
    {
        const string sql = @"
            SELECT t.number AS table_number, gen.hour,
                CASE WHEN EXISTS (
                    SELECT 1 FROM reservation_tables rt
                    JOIN reservations r ON r.id_reserve = rt.id_reserve
                    WHERE rt.id_table = t.id_table
                      AND r.res_date = CURRENT_DATE
                      AND gen.hour::time >= r.time_start AND gen.hour::time < r.time_end
                ) THEN 'занят' ELSE 'свободен' END AS status
            FROM tables t
            CROSS JOIN generate_series(9, 22) AS gen(hour)
            ORDER BY t.number, gen.hour";
        RunReport(sql, new[] { "Столик №", "Час", "Статус" });
    }

    private void RunReport(string sql, string[] columnNames)
    {
        dgvReport.Columns.Clear();
        dgvReport.Rows.Clear();
        foreach (var name in columnNames) dgvReport.Columns.Add(name, name);

        try
        {
            using var conn = Data.DbConfig.CreateConnection();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                dgvReport.Rows.Add(values);
            }
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(ex.MessageText, "Ошибка запроса", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
