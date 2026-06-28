namespace RestaurantApp.UI;

partial class MenuStockForm
{
    private System.ComponentModel.IContainer components = null!;
    private DataGridView dgvMenu = null!;
    private Button btnAddDiscount = null!;
    private Button btnRefresh = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        dgvMenu = new DataGridView();
        btnAddDiscount = new Button();
        btnRefresh = new Button();

        dgvMenu.Location = new Point(20, 20);
        dgvMenu.Size = new Size(560, 300);
        dgvMenu.ReadOnly = true;
        dgvMenu.AllowUserToAddRows = false;
        dgvMenu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvMenu.MultiSelect = false;
        dgvMenu.Columns.Add("DishId", "ID");
        dgvMenu.Columns.Add("Name", "Блюдо");
        dgvMenu.Columns.Add("BasePrice", "Базовая цена");
        dgvMenu.Columns.Add("DiscountPercent", "Скидка %");
        dgvMenu.Columns.Add("CurrentPrice", "Текущая цена");
        dgvMenu.Columns.Add("Stock", "Остаток");

        btnAddDiscount.Text = "Добавить скидку на выбранное";
        btnAddDiscount.Location = new Point(20, 335);
        btnAddDiscount.Width = 220;
        btnAddDiscount.Click += btnAddDiscount_Click;

        btnRefresh.Text = "Обновить";
        btnRefresh.Location = new Point(250, 335);
        btnRefresh.Width = 100;
        btnRefresh.Click += btnRefresh_Click;

        ClientSize = new Size(600, 390);
        Text = "Меню и склад";
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(dgvMenu);
        Controls.Add(btnAddDiscount);
        Controls.Add(btnRefresh);
    }
}
