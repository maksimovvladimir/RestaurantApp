using RestaurantApp.Data.Models;
using RestaurantApp.Data.Repositories;
using Npgsql;

namespace RestaurantApp.UI;

public partial class OrdersForm : Form
{
    private readonly OrdersRepository _ordersRepo = new();
    private readonly DishesRepository _dishesRepo = new();
    private readonly UserAccount _currentUser;

    private int? _currentOrderId;

    public OrdersForm(UserAccount currentUser)
    {
        _currentUser = currentUser;
        InitializeComponent();
        LoadOrders();
        LoadMenu();
    }

    /// <summary>
    /// Список заказов в работе — не только "draft", но и переданные на кухню,
    /// чтобы официант и кухня видели один и тот же список с привязкой к столику.
    /// </summary>
    private void LoadOrders()
    {
        cmbOrders.Items.Clear();
        foreach (var status in new[] { "draft", "placed", "cooking", "ready" })
        {
            foreach (var (order, tableNumber, clientName) in _ordersRepo.GetOrdersByStatusWithTable(status))
            {
                cmbOrders.Items.Add(new ComboBoxOrderItem(order, tableNumber, clientName));
            }
        }
        if (cmbOrders.Items.Count > 0) cmbOrders.SelectedIndex = 0;
        else RefreshOrderItems();
    }

    private void LoadMenu()
    {
        cmbDish.Items.Clear();
        foreach (var dish in _dishesRepo.GetMenuWithCurrentPrices())
        {
            cmbDish.Items.Add(new ComboBoxDishItem(dish));
        }
        if (cmbDish.Items.Count > 0) cmbDish.SelectedIndex = 0;
    }

    private void RefreshOrderItems()
    {
        lstItems.Items.Clear();
        if (_currentOrderId is null)
        {
            lblTotal.Text = "Сумма: —";
            lblStatus.Text = "Статус: —";
            btnPay.Enabled = false;
            return;
        }

        foreach (var (item, dishName) in _ordersRepo.GetOrderItems(_currentOrderId.Value))
        {
            lstItems.Items.Add($"{dishName}  ×{item.Quantity}");
        }

        var selected = cmbOrders.SelectedItem as ComboBoxOrderItem;
        if (selected is not null)
        {
            lblTotal.Text = $"Сумма: {selected.Order.TotalAmount:0.00} ₽";
            lblStatus.Text = $"Статус: {StatusToRussian(selected.Order.Status)}";
        }

        var hasReceipt = _ordersRepo.HasReceipt(_currentOrderId.Value);
        btnPay.Enabled = selected?.Order.Status == "served" && !hasReceipt;

        if (hasReceipt)
        {
            var receipt = _ordersRepo.GetReceipt(_currentOrderId.Value);
            if (receipt is not null)
            {
                lblTotal.Text += $"   (оплачено: {receipt.Amount:0.00} ₽, {PaymentToRussian(receipt.PaymentMethoc)}, {receipt.PaidAt:dd.MM HH:mm})";
            }
        }
    }

    private static string StatusToRussian(string status) => status switch
    {
        "draft" => "составление",
        "placed" => "оформлен",
        "cooking" => "готовится",
        "ready" => "готов к выдаче",
        "served" => "выдан",
        "cancelled" => "отменён",
        _ => status
    };

    private static string PaymentToRussian(string method) => method switch
    {
        "cash" => "наличные",
        "card" => "карта",
        "online" => "онлайн",
        _ => method
    };

    private void cmbOrders_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbOrders.SelectedItem is ComboBoxOrderItem selected)
        {
            _currentOrderId = selected.Order.Id;
            RefreshOrderItems();
        }
    }

    private void btnNewOrder_Click(object sender, EventArgs e)
    {
        using var promptForm = new NewOrderPromptForm();
        if (!promptForm.HasOptions)
        {
            MessageBox.Show("Нет активных броней со столиками. Сначала создай бронь в разделе \"Брони / посещения\".",
                "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (promptForm.ShowDialog() != DialogResult.OK) return;

        try
        {
            var newId = _ordersRepo.CreateDraftOrder(promptForm.IdReserveTable);
            LoadOrders();
            for (int i = 0; i < cmbOrders.Items.Count; i++)
            {
                if (cmbOrders.Items[i] is ComboBoxOrderItem item && item.Order.Id == newId)
                {
                    cmbOrders.SelectedIndex = i;
                    break;
                }
            }
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(ex.MessageText, "Ошибка создания заказа", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnAddDish_Click(object sender, EventArgs e)
    {
        if (_currentOrderId is null)
        {
            MessageBox.Show("Сначала выберите или создайте заказ.", "Нет заказа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbDish.SelectedItem is not ComboBoxDishItem dishItem) return;

        var qty = (int)numQuantity.Value;

        try
        {
            var message = _ordersRepo.AddDishToOrder(_currentOrderId.Value, dishItem.Dish.DishId, qty);
            MessageBox.Show(message, "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadOrders();
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(ex.MessageText, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnPlaceOrder_Click(object sender, EventArgs e)
    {
        if (_currentOrderId is null) return;
        ChangeStatusSafe("placed", "Заказ оформлен и передан на кухню.");
    }

    private void btnStartCooking_Click(object sender, EventArgs e)
    {
        if (_currentOrderId is null) return;
        ChangeStatusSafe("cooking", "Заказ взят в готовку.");
    }

    private void btnMarkReady_Click(object sender, EventArgs e)
    {
        if (_currentOrderId is null) return;
        ChangeStatusSafe("ready", "Заказ готов к выдаче.");
    }

    private void btnMarkServed_Click(object sender, EventArgs e)
    {
        if (_currentOrderId is null) return;
        ChangeStatusSafe("served", "Заказ выдан гостю. Теперь можно принять оплату.");
    }

    private void ChangeStatusSafe(string newStatus, string successMessage)
    {
        try
        {
            _ordersRepo.ChangeStatus(_currentOrderId!.Value, newStatus);
            MessageBox.Show(successMessage, "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadOrders();
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(ex.MessageText, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnPay_Click(object sender, EventArgs e)
    {
        if (_currentOrderId is null) return;

        using var promptForm = new PaymentPromptForm();
        if (promptForm.ShowDialog() != DialogResult.OK) return;

        try
        {
            _ordersRepo.CreateReceipt(_currentOrderId.Value, promptForm.PaymentMethod);
            MessageBox.Show("Оплата принята, чек сформирован.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshOrderItems();
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(ex.MessageText, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private class ComboBoxOrderItem
    {
        public Order Order { get; }
        private readonly int _tableNumber;
        private readonly string _clientName;
        public ComboBoxOrderItem(Order order, int tableNumber, string clientName)
        {
            Order = order;
            _tableNumber = tableNumber;
            _clientName = clientName;
        }
        public override string ToString() =>
            $"Заказ №{Order.Id} — столик №{_tableNumber} ({_clientName}) — {StatusToRussian(Order.Status)}";
    }

    private class ComboBoxDishItem
    {
        public DishCurrentPrice Dish { get; }
        public ComboBoxDishItem(DishCurrentPrice dish) => Dish = dish;
        public override string ToString() => Dish.DiscountPercent is null
            ? $"{Dish.Name} — {Dish.CurrentPrice:0.00} ₽"
            : $"{Dish.Name} — {Dish.CurrentPrice:0.00} ₽ (скидка {Dish.DiscountPercent}%)";
    }
}
