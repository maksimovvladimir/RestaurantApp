using RestaurantApp.Data.Repositories;
using Npgsql;

namespace RestaurantApp.UI;

/// <summary>
/// Просмотр меню с текущими ценами (с учётом скидок) и остатков склада, добавление скидки.
/// TODO следующая итерация: добавление/редактирование самих блюд и категорий через UI
/// (сейчас это делается напрямую в БД администратором при заполнении меню).
/// </summary>
public partial class MenuStockForm : Form
{
    private readonly DishesRepository _dishesRepo = new();

    public MenuStockForm()
    {
        InitializeComponent();
        RefreshGrid();
    }

    private void RefreshGrid()
    {
        var menu = _dishesRepo.GetMenuWithCurrentPrices();
        var stock = _dishesRepo.GetStock().ToDictionary(s => s.DishId, s => s.QuantityAvalible);

        dgvMenu.Rows.Clear();
        foreach (var dish in menu)
        {
            var qty = stock.TryGetValue(dish.DishId, out var q) ? q.ToString() : "—";
            dgvMenu.Rows.Add(dish.DishId, dish.Name, dish.BasePrice, dish.DiscountPercent, dish.CurrentPrice, qty);
        }
    }

    private void btnAddDiscount_Click(object sender, EventArgs e)
    {
        if (dgvMenu.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите блюдо в таблице.", "Не выбрано", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var dishId = (int)dgvMenu.SelectedRows[0].Cells[0].Value!;

        using var promptForm = new DiscountPromptForm();
        if (promptForm.ShowDialog() != DialogResult.OK) return;

        try
        {
            _dishesRepo.AddDiscount(dishId, promptForm.DiscountName, promptForm.Percent,
                promptForm.StartDate, promptForm.EndDate);
            MessageBox.Show("Скидка применена.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshGrid();
        }
        catch (PostgresException ex)
        {
            MessageBox.Show(ex.MessageText, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnRefresh_Click(object sender, EventArgs e) => RefreshGrid();
}
