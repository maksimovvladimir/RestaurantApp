namespace RestaurantApp.UI;

partial class StatisticsForm
{
    private System.ComponentModel.IContainer components = null!;
    private Button btnSalesByCategory = null!;
    private Button btnReservationsByTable = null!;
    private Button btnTableOccupancy = null!;
    private DataGridView dgvReport = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        btnSalesByCategory = new Button();
        btnReservationsByTable = new Button();
        btnTableOccupancy = new Button();
        dgvReport = new DataGridView();

        btnSalesByCategory.Text = "Продажи по категориям";
        btnSalesByCategory.Location = new Point(20, 20);
        btnSalesByCategory.Width = 180;
        btnSalesByCategory.Click += btnSalesByCategory_Click;

        btnReservationsByTable.Text = "Брони по столикам";
        btnReservationsByTable.Location = new Point(210, 20);
        btnReservationsByTable.Width = 180;
        btnReservationsByTable.Click += btnReservationsByTable_Click;

        btnTableOccupancy.Text = "Занятость по часам";
        btnTableOccupancy.Location = new Point(400, 20);
        btnTableOccupancy.Width = 180;
        btnTableOccupancy.Click += btnTableOccupancy_Click;

        dgvReport.Location = new Point(20, 60);
        dgvReport.Size = new Size(560, 350);
        dgvReport.ReadOnly = true;
        dgvReport.AllowUserToAddRows = false;

        ClientSize = new Size(600, 430);
        Text = "Статистика";
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(btnSalesByCategory);
        Controls.Add(btnReservationsByTable);
        Controls.Add(btnTableOccupancy);
        Controls.Add(dgvReport);
    }
}
