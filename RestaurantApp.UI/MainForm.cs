using RestaurantApp.Data.Models;

namespace RestaurantApp.UI;

public partial class MainForm : Form
{
    private readonly UserAccount _currentUser;

    public MainForm(UserAccount currentUser)
    {
        _currentUser = currentUser;
        InitializeComponent();

        lblWelcome.Text = $"{_currentUser.FullName}  ({RoleToRussian(_currentUser.Role)})";

        // Доступ по ролям: проверка прав всё равно остаётся на стороне БД
        // (sp_* функции и триггеры), здесь — только скрытие лишних кнопок в UI.
        btnOrders.Visible = _currentUser.Role is "waiter" or "admin" or "kitchen";
        btnReservations.Visible = _currentUser.Role is "waiter" or "admin";
        btnMenuStock.Visible = _currentUser.Role is "admin";
        btnStatistics.Visible = _currentUser.Role is "admin";
    }

    private static string RoleToRussian(string role) => role switch
    {
        "waiter" => "Официант",
        "kitchen" => "Кухня",
        "admin" => "Администратор",
        _ => role
    };

    private void btnOrders_Click(object sender, EventArgs e) => new OrdersForm(_currentUser).Show();
    private void btnReservations_Click(object sender, EventArgs e) => new ReservationsForm().Show();
    private void btnMenuStock_Click(object sender, EventArgs e) => new MenuStockForm().Show();
    private void btnStatistics_Click(object sender, EventArgs e) => new StatisticsForm().Show();

    private void btnLogout_Click(object sender, EventArgs e)
    {
        var login = new LoginForm();
        this.Hide();
        login.FormClosed += (_, _) => this.Close();
        login.Show();
    }
}
