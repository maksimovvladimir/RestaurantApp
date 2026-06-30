namespace RestaurantApp.UI;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblWelcome = null!;
    private Button btnOrders = null!;
    private Button btnReservations = null!;
    private Button btnMenuStock = null!;
    private Button btnStatistics = null!;
    private Button btnLogout = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblWelcome = new Label();
        btnOrders = new Button();
        btnReservations = new Button();
        btnMenuStock = new Button();
        btnStatistics = new Button();
        btnLogout = new Button();

        lblWelcome.AutoSize = true;
        lblWelcome.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblWelcome.Location = new Point(30, 25);

        btnOrders.Text = "Заказы";
        btnOrders.Location = new Point(30, 80);
        btnOrders.Size = new Size(180, 45);
        btnOrders.Click += btnOrders_Click;

        btnReservations.Text = "Брони / посещения";
        btnReservations.Location = new Point(30, 135);
        btnReservations.Size = new Size(180, 45);
        btnReservations.Click += btnReservations_Click;

        btnMenuStock.Text = "Меню и склад";
        btnMenuStock.Location = new Point(30, 190);
        btnMenuStock.Size = new Size(180, 45);
        btnMenuStock.Click += btnMenuStock_Click;

        btnStatistics.Text = "Статистика";
        btnStatistics.Location = new Point(30, 245);
        btnStatistics.Size = new Size(180, 45);
        btnStatistics.Click += btnStatistics_Click;

        btnLogout.Text = "Выйти из аккаунта";
        btnLogout.Location = new Point(30, 300);
        btnLogout.Size = new Size(180, 35);
        btnLogout.Click += btnLogout_Click;

        ClientSize = new Size(280, 365);
        Text = "Ресторан — главное меню";
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(lblWelcome);
        Controls.Add(btnOrders);
        Controls.Add(btnReservations);
        Controls.Add(btnMenuStock);
        Controls.Add(btnStatistics);
        Controls.Add(btnLogout);
    }
}
