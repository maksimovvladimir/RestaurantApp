namespace RestaurantApp.UI;

public class NewOrderPromptForm : Form
{
    private readonly Label _lbl;
    private readonly NumericUpDown _num;
    private readonly Button _ok;
    private readonly Button _cancel;

    public int IdReserveTable => (int)_num.Value;

    public NewOrderPromptForm()
    {
        _lbl = new Label
        {
            Text = "Номер записи Бронь_столики (id_reserve_table), для которой создаём заказ:",
            AutoSize = true,
            MaximumSize = new Size(280, 0),
            Location = new Point(20, 20)
        };
        _num = new NumericUpDown { Location = new Point(20, 70), Width = 100, Minimum = 1, Maximum = 999999 };
        _ok = new Button { Text = "Создать", Location = new Point(20, 110), DialogResult = DialogResult.OK };
        _cancel = new Button { Text = "Отмена", Location = new Point(110, 110), DialogResult = DialogResult.Cancel };

        ClientSize = new Size(320, 160);
        Text = "Новый заказ";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        AcceptButton = _ok;
        CancelButton = _cancel;

        Controls.Add(_lbl);
        Controls.Add(_num);
        Controls.Add(_ok);
        Controls.Add(_cancel);
    }
}
