namespace RestaurantApp.UI;

public class PaymentPromptForm : Form
{
    private readonly ComboBox _cmb;

    public string PaymentMethod => ((Option)_cmb.SelectedItem!).Value;

    public PaymentPromptForm()
    {
        var lbl = new Label { Text = "Способ оплаты:", AutoSize = true, Location = new Point(20, 20) };

        _cmb = new ComboBox { Location = new Point(20, 45), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmb.Items.Add(new Option("cash", "Наличные"));
        _cmb.Items.Add(new Option("card", "Карта"));
        _cmb.Items.Add(new Option("online", "Онлайн"));
        _cmb.SelectedIndex = 0;

        var ok = new Button { Text = "Принять оплату", Location = new Point(20, 80), Width = 130, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Отмена", Location = new Point(160, 80), DialogResult = DialogResult.Cancel };

        ClientSize = new Size(260, 120);
        Text = "Оплата заказа";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        AcceptButton = ok;
        CancelButton = cancel;

        Controls.Add(lbl);
        Controls.Add(_cmb);
        Controls.Add(ok);
        Controls.Add(cancel);
    }

    private class Option
    {
        public string Value { get; }
        private readonly string _label;
        public Option(string value, string label) { Value = value; _label = label; }
        public override string ToString() => _label;
    }
}
