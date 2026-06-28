using RestaurantApp.Data.Models;
using RestaurantApp.Data.Repositories;

namespace RestaurantApp.UI;

public partial class LoginForm : Form
{
    private readonly UsersRepository _usersRepo = new();

    public LoginForm()
    {
        InitializeComponent();
    }

    private void btnLogin_Click(object sender, EventArgs e)
    {
        var login = txtLogin.Text.Trim();
        var password = txtPassword.Text;

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UserAccount? user;
        try
        {
            user = _usersRepo.TryLogin(login, password);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось подключиться к базе данных:\n{ex.Message}",
                "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (user is null)
        {
            MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var main = new MainForm(user);
        this.Hide();
        main.FormClosed += (_, _) => this.Close();
        main.Show();
    }
}
