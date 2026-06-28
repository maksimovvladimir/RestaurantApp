namespace RestaurantApp.UI;

partial class ReservationsForm
{
    private System.ComponentModel.IContainer components = null!;
    private TextBox txtClientName = null!;
    private TextBox txtPhone = null!;
    private DateTimePicker dtDate = null!;
    private DateTimePicker dtTimeStart = null!;
    private DateTimePicker dtTimeEnd = null!;
    private NumericUpDown numGuests = null!;
    private ComboBox cmbTable = null!;
    private Button btnCreate = null!;
    private ListBox lstReservations = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        txtClientName = new TextBox();
        txtPhone = new TextBox();
        dtDate = new DateTimePicker { Format = DateTimePickerFormat.Short };
        dtTimeStart = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true };
        dtTimeEnd = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true };
        numGuests = new NumericUpDown { Minimum = 1, Maximum = 20, Value = 2 };
        cmbTable = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        btnCreate = new Button();
        lstReservations = new ListBox();

        var lblName = new Label { Text = "Имя гостя:", AutoSize = true, Location = new Point(20, 20) };
        txtClientName.Location = new Point(140, 17); txtClientName.Width = 200;

        var lblPhone = new Label { Text = "Телефон:", AutoSize = true, Location = new Point(20, 50) };
        txtPhone.Location = new Point(140, 47); txtPhone.Width = 200;

        var lblDate = new Label { Text = "Дата:", AutoSize = true, Location = new Point(20, 80) };
        dtDate.Location = new Point(140, 77); dtDate.Width = 120;

        var lblTime = new Label { Text = "Время с / до:", AutoSize = true, Location = new Point(20, 110) };
        dtTimeStart.Location = new Point(140, 107); dtTimeStart.Width = 90;
        dtTimeEnd.Location = new Point(240, 107); dtTimeEnd.Width = 90;

        var lblGuests = new Label { Text = "Гостей:", AutoSize = true, Location = new Point(20, 140) };
        numGuests.Location = new Point(140, 137); numGuests.Width = 60;

        var lblTable = new Label { Text = "Столик:", AutoSize = true, Location = new Point(20, 170) };
        cmbTable.Location = new Point(140, 167); cmbTable.Width = 200;

        btnCreate.Text = "Создать бронь";
        btnCreate.Location = new Point(140, 205);
        btnCreate.Width = 150;
        btnCreate.Click += btnCreate_Click;

        lstReservations.Location = new Point(20, 245);
        lstReservations.Size = new Size(440, 160);

        ClientSize = new Size(480, 420);
        Text = "Брони / посещения";
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(lblName); Controls.Add(txtClientName);
        Controls.Add(lblPhone); Controls.Add(txtPhone);
        Controls.Add(lblDate); Controls.Add(dtDate);
        Controls.Add(lblTime); Controls.Add(dtTimeStart); Controls.Add(dtTimeEnd);
        Controls.Add(lblGuests); Controls.Add(numGuests);
        Controls.Add(lblTable); Controls.Add(cmbTable);
        Controls.Add(btnCreate);
        Controls.Add(lstReservations);
    }
}
