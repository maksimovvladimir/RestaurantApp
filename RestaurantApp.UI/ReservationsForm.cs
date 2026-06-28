using RestaurantApp.Data.Repositories;
using Npgsql;

namespace RestaurantApp.UI;

/// <summary>
/// Создание записи о посещении (бронь либо "живой" гость без предзаказа — оба случая
/// заносит администратор/официант) и привязка столика к ней.
/// TODO следующая итерация: список активных броней с возможностью отмены статуса.
/// </summary>
public partial class ReservationsForm : Form
{
    private readonly ReservationsRepository _reservationsRepo = new();
    private readonly TablesRepository _tablesRepo = new();

    public ReservationsForm()
    {
        InitializeComponent();
        LoadTables();
        RefreshList();
    }

    private void LoadTables()
    {
        cmbTable.Items.Clear();
        foreach (var t in _tablesRepo.GetAll())
            cmbTable.Items.Add(new TableComboItem(t));
        if (cmbTable.Items.Count > 0) cmbTable.SelectedIndex = 0;
    }

    private void RefreshList()
    {
        lstReservations.Items.Clear();
        foreach (var r in _reservationsRepo.GetActive())
        {
            lstReservations.Items.Add(
                $"#{r.IdReserve}  {r.ClientName}  {r.ResDate:dd.MM}  {r.TimeStart:hh\\:mm}-{r.TimeEnd:hh\\:mm}  гостей: {r.GuestsCount}");
        }
    }

    private void btnCreate_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtClientName.Text))
        {
            MessageBox.Show("Укажите имя гостя.", "Не заполнено", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbTable.SelectedItem is not TableComboItem tableItem)
        {
            MessageBox.Show("Выберите столик.", "Не заполнено", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var idReserve = _reservationsRepo.CreateReservation(
                txtClientName.Text.Trim(),
                string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                dtDate.Value.Date,
                dtTimeStart.Value.TimeOfDay,
                dtTimeEnd.Value.TimeOfDay,
                (int)numGuests.Value);

            // Триггер trg_check_reservation_capacity сам проверит вместимость столика
            _reservationsRepo.AttachTable(idReserve, tableItem.Table.IdTable);

            MessageBox.Show($"Создана бронь №{idReserve}, столик закреплён.", "Готово",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshList();
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(ex.MessageText, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private class TableComboItem
    {
        public Data.Models.TableInfo Table { get; }
        public TableComboItem(Data.Models.TableInfo t) => Table = t;
        public override string ToString() => $"Столик №{Table.Number} (до {Table.Capacity} чел., {Table.Status})";
    }
}
