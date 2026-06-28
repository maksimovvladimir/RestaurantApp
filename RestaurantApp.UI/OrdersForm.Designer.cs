namespace RestaurantApp.UI;

partial class OrdersForm
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblOrder = null!;
    private ComboBox cmbOrders = null!;
    private Button btnNewOrder = null!;
    private ListBox lstItems = null!;
    private Label lblDish = null!;
    private ComboBox cmbDish = null!;
    private Label lblQty = null!;
    private NumericUpDown numQuantity = null!;
    private Button btnAddDish = null!;
    private Label lblTotal = null!;
    private Button btnPlaceOrder = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblOrder = new Label();
        cmbOrders = new ComboBox();
        btnNewOrder = new Button();
        lstItems = new ListBox();
        lblDish = new Label();
        cmbDish = new ComboBox();
        lblQty = new Label();
        numQuantity = new NumericUpDown();
        btnAddDish = new Button();
        lblTotal = new Label();
        btnPlaceOrder = new Button();

        lblOrder.Text = "Заказ (в составлении):";
        lblOrder.AutoSize = true;
        lblOrder.Location = new Point(20, 20);

        cmbOrders.Location = new Point(20, 45);
        cmbOrders.Width = 280;
        cmbOrders.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbOrders.SelectedIndexChanged += cmbOrders_SelectedIndexChanged;

        btnNewOrder.Text = "Новый заказ...";
        btnNewOrder.Location = new Point(310, 44);
        btnNewOrder.Width = 140;
        btnNewOrder.Click += btnNewOrder_Click;

        lstItems.Location = new Point(20, 80);
        lstItems.Size = new Size(430, 150);

        lblDish.Text = "Блюдо:";
        lblDish.AutoSize = true;
        lblDish.Location = new Point(20, 245);

        cmbDish.Location = new Point(20, 268);
        cmbDish.Width = 280;
        cmbDish.DropDownStyle = ComboBoxStyle.DropDownList;

        lblQty.Text = "Кол-во:";
        lblQty.AutoSize = true;
        lblQty.Location = new Point(310, 250);

        numQuantity.Location = new Point(310, 268);
        numQuantity.Width = 60;
        numQuantity.Minimum = 1;
        numQuantity.Maximum = 99;
        numQuantity.Value = 1;

        btnAddDish.Text = "Добавить в заказ";
        btnAddDish.Location = new Point(20, 300);
        btnAddDish.Width = 200;
        btnAddDish.Click += btnAddDish_Click;

        lblTotal.Text = "Сумма: —";
        lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblTotal.AutoSize = true;
        lblTotal.Location = new Point(20, 340);

        btnPlaceOrder.Text = "Оформить заказ";
        btnPlaceOrder.Location = new Point(20, 370);
        btnPlaceOrder.Width = 200;
        btnPlaceOrder.Click += btnPlaceOrder_Click;

        ClientSize = new Size(470, 420);
        Text = "Заказы";
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(lblOrder);
        Controls.Add(cmbOrders);
        Controls.Add(btnNewOrder);
        Controls.Add(lstItems);
        Controls.Add(lblDish);
        Controls.Add(cmbDish);
        Controls.Add(lblQty);
        Controls.Add(numQuantity);
        Controls.Add(btnAddDish);
        Controls.Add(lblTotal);
        Controls.Add(btnPlaceOrder);
    }
}
