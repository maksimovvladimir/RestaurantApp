namespace RestaurantApp.UI;

public class DiscountPromptForm : Form
{
    private readonly TextBox _txtName;
    private readonly NumericUpDown _numPercent;
    private readonly DateTimePicker _dtStart;
    private readonly DateTimePicker _dtEnd;

    public string DiscountName => _txtName.Text.Trim();
    public decimal Percent => _numPercent.Value;
    public DateTime StartDate => _dtStart.Value.Date;
    public DateTime EndDate => _dtEnd.Value.Date;

    public DiscountPromptForm()
    {
        _txtName = new TextBox { Location = new Point(140, 17), Width = 180, Text = "Акция" };
        _numPercent = new NumericUpDown { Location = new Point(140, 47), Width = 80, Minimum = 1, Maximum = 90, Value = 10 };
        _dtStart = new DateTimePicker { Location = new Point(140, 77), Width = 150, Format = DateTimePickerFormat.Short };
        _dtEnd = new DateTimePicker { Location = new Point(140, 107), Width = 150, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(7) };

        var ok = new Button { Text = "ОК", Location = new Point(140, 145), DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Отмена", Location = new Point(230, 145), DialogResult = DialogResult.Cancel };

        ClientSize = new Size(360, 190);
        Text = "Новая скидка";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        AcceptButton = ok;
        CancelButton = cancel;

        Controls.Add(new Label { Text = "Название:", AutoSize = true, Location = new Point(20, 20) });
        Controls.Add(new Label { Text = "Процент:", AutoSize = true, Location = new Point(20, 50) });
        Controls.Add(new Label { Text = "Дата начала:", AutoSize = true, Location = new Point(20, 80) });
        Controls.Add(new Label { Text = "Дата конца:", AutoSize = true, Location = new Point(20, 110) });
        Controls.Add(_txtName);
        Controls.Add(_numPercent);
        Controls.Add(_dtStart);
        Controls.Add(_dtEnd);
        Controls.Add(ok);
        Controls.Add(cancel);
    }
}
