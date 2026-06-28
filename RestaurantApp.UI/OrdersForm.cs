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
        LoadDraftOrders();
        LoadMenu();
    }

    /// <summary>Список заказов в статусе "draft" — это и есть "составление".</summary>
    private void LoadDraftOrders()
    {
        cmbOrders.Items.Clear();
        foreach (var order in _ordersRepo.GetOrdersByStatus("draft"))
        {
            cmbOrders.Items.Add(new ComboBoxOrderItem(order));
        }
        if (cmbOrders.Items.Count > 0) cmbOrders.SelectedIndex = 0;
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
        if (_currentOrderId is null) return;

        decimal total = 0;
        foreach (var (item, dishName) in _ordersRepo.GetOrderItems(_currentOrderId.Value))
        {
            lstItems.Items.Add($"{dishName}  ×{item.Quantity}");
        }

        var order = _ordersRepo.GetOrdersByStatus("draft").FirstOrDefault(o => o.Id == _currentOrderId);
        lblTotal.Text = order is null ? "Сумма: —" : $"Сумма: {order.TotalAmount:0.00} ₽";
    }

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
        if (promptForm.ShowDialog() != DialogResult.OK) return;

        try
        {
            var newId = _ordersRepo.CreateDraftOrder(promptForm.IdReserveTable);
            LoadDraftOrders();
            // выбираем только что созданный заказ
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
            // Сообщение приходит прямо из БД (sp_add_dish_to_order) — и про успех,
            // и про нехватку порций на складе, текст ровно как в задании.
            var message = _ordersRepo.AddDishToOrder(_currentOrderId.Value, dishItem.Dish.DishId, qty);
            MessageBox.Show(message, "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshOrderItems();
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(ex.MessageText, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnPlaceOrder_Click(object sender, EventArgs e)
    {
        if (_currentOrderId is null) return;

        try
        {
            _ordersRepo.ChangeStatus(_currentOrderId.Value, "placed");
            MessageBox.Show("Заказ оформлен и передан на кухню.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadDraftOrders();
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(ex.MessageText, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private class ComboBoxOrderItem
    {
        public Order Order { get; }
        public ComboBoxOrderItem(Order order) => Order = order;
        public override string ToString() => $"Заказ №{Order.Id} (бронь_столик #{Order.IdReserveTable})";
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
