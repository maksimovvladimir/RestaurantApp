using RestaurantApp.Data.Repositories;

namespace RestaurantApp.UI;

public class NewOrderPromptForm : Form
{
    private readonly ComboBox _cmb;
    private readonly Button _ok;
    private readonly Button _cancel;
    private readonly ReservationsRepository _reservationsRepo = new();

    public int IdReserveTable => ((ComboItem)_cmb.SelectedItem!).IdReserveTable;
    public bool HasOptions => _cmb.Items.Count > 0;

    public NewOrderPromptForm()
    {
        var lbl = new Label
        {
            Text = "Выберите гостя/бронь и столик, для которых создаём заказ:",
            AutoSize = true,
            MaximumSize = new Size(320, 0),
            Location = new Point(20, 20)
        };

        _cmb = new ComboBox
        {
            Location = new Point(20, 60),
            Width = 320,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        foreach (var (idReserveTable, description) in _reservationsRepo.GetReservationTablesForOrder())
        {
            _cmb.Items.Add(new ComboItem(idReserveTable, description));
        }
        if (_cmb.Items.Count > 0) _cmb.SelectedIndex = 0;

        var lblEmpty = new Label
        {
            Text = "Нет активных броней со столиками. Сначала создай бронь в разделе \"Брони / посещения\".",
            AutoSize = true,
            MaximumSize = new Size(320, 0),
            ForeColor = Color.DarkRed,
            Location = new Point(20, 65),
            Visible = _cmb.Items.Count == 0
        };

        _ok = new Button { Text = "Создать", Location = new Point(20, 110), DialogResult = DialogResult.OK, Enabled = _cmb.Items.Count > 0 };
        _cancel = new Button { Text = "Отмена", Location = new Point(110, 110), DialogResult = DialogResult.Cancel };

        ClientSize = new Size(360, 160);
        Text = "Новый заказ";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        AcceptButton = _ok;
        CancelButton = _cancel;

        Controls.Add(lbl);
        Controls.Add(_cmb);
        Controls.Add(lblEmpty);
        Controls.Add(_ok);
        Controls.Add(_cancel);
    }

    private class ComboItem
    {
        public int IdReserveTable { get; }
        private readonly string _description;
        public ComboItem(int idReserveTable, string description)
        {
            IdReserveTable = idReserveTable;
            _description = description;
        }
        public override string ToString() => _description;
    }
}
