namespace RestaurantApp.UI;

partial class OrdersForm
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblOrder = null!;
    private ComboBox cmbOrders = null!;
    private Button btnNewOrder = null!;
    private Label lblStatus = null!;
    private ListBox lstItems = null!;
    private Label lblDish = null!;
    private ComboBox cmbDish = null!;
    private Label lblQty = null!;
    private NumericUpDown numQuantity = null!;
    private Button btnAddDish = null!;
    private Label lblTotal = null!;
    private Button btnPlaceOrder = null!;
    private Button btnStartCooking = null!;
    private Button btnMarkReady = null!;
    private Button btnMarkServed = null!;
    private Button btnPay = null!;

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
        lblStatus = new Label();
        lstItems = new ListBox();
        lblDish = new Label();
        cmbDish = new ComboBox();
        lblQty = new Label();
        numQuantity = new NumericUpDown();
        btnAddDish = new Button();
        lblTotal = new Label();
        btnPlaceOrder = new Button();
        btnStartCooking = new Button();
        btnMarkReady = new Button();
        btnMarkServed = new Button();
        btnPay = new Button();

        lblOrder.Text = "Заказ (столик / гость / статус):";
        lblOrder.AutoSize = true;
        lblOrder.Location = new Point(20, 15);

        cmbOrders.Location = new Point(20, 38);
        cmbOrders.Width = 360;
        cmbOrders.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbOrders.SelectedIndexChanged += cmbOrders_SelectedIndexChanged;

        btnNewOrder.Text = "Новый заказ...";
        btnNewOrder.Location = new Point(390, 37);
        btnNewOrder.Width = 130;
        btnNewOrder.Click += btnNewOrder_Click;

        lblStatus.Text = "Статус: —";
        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.Location = new Point(20, 68);

        lstItems.Location = new Point(20, 95);
        lstItems.Size = new Size(500, 120);

        lblDish.Text = "Блюдо:";
        lblDish.AutoSize = true;
        lblDish.Location = new Point(20, 225);

        cmbDish.Location = new Point(20, 248);
        cmbDish.Width = 280;
        cmbDish.DropDownStyle = ComboBoxStyle.DropDownList;

        lblQty.Text = "Кол-во:";
        lblQty.AutoSize = true;
        lblQty.Location = new Point(310, 230);

        numQuantity.Location = new Point(310, 248);
        numQuantity.Width = 60;
        numQuantity.Minimum = 1;
        numQuantity.Maximum = 99;
        numQuantity.Value = 1;

        btnAddDish.Text = "Добавить в заказ";
        btnAddDish.Location = new Point(380, 247);
        btnAddDish.Width = 140;
        btnAddDish.Click += btnAddDish_Click;

        lblTotal.Text = "Сумма: —";
        lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblTotal.AutoSize = true;
        lblTotal.Location = new Point(20, 285);

        // Жизненный цикл заказа: официант оформляет, кухня готовит/выдаёт, официант принимает оплату.
        // Все кнопки видны всем ролям, но реальное разрешение перехода всё равно проверяет БД.
        btnPlaceOrder.Text = "Оформить";
        btnPlaceOrder.Location = new Point(20, 320);
        btnPlaceOrder.Width = 110;
        btnPlaceOrder.Click += btnPlaceOrder_Click;

        btnStartCooking.Text = "Взять в готовку";
        btnStartCooking.Location = new Point(140, 320);
        btnStartCooking.Width = 120;
        btnStartCooking.Click += btnStartCooking_Click;

        btnMarkReady.Text = "Готово к выдаче";
        btnMarkReady.Location = new Point(270, 320);
        btnMarkReady.Width = 130;
        btnMarkReady.Click += btnMarkReady_Click;

        btnMarkServed.Text = "Выдано";
        btnMarkServed.Location = new Point(410, 320);
        btnMarkServed.Width = 110;
        btnMarkServed.Click += btnMarkServed_Click;

        btnPay.Text = "Оплатить / показать чек";
        btnPay.Location = new Point(20, 355);
        btnPay.Width = 220;
        btnPay.Enabled = false;
        btnPay.Click += btnPay_Click;

        ClientSize = new Size(540, 400);
        Text = "Заказы";
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(lblOrder);
        Controls.Add(cmbOrders);
        Controls.Add(btnNewOrder);
        Controls.Add(lblStatus);
        Controls.Add(lstItems);
        Controls.Add(lblDish);
        Controls.Add(cmbDish);
        Controls.Add(lblQty);
        Controls.Add(numQuantity);
        Controls.Add(btnAddDish);
        Controls.Add(lblTotal);
        Controls.Add(btnPlaceOrder);
        Controls.Add(btnStartCooking);
        Controls.Add(btnMarkReady);
        Controls.Add(btnMarkServed);
        Controls.Add(btnPay);
    }
}
