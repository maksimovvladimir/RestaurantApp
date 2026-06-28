namespace RestaurantApp.UI;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null!;
    private Label lblTitle = null!;
    private Label lblLogin = null!;
    private Label lblPassword = null!;
    private TextBox txtLogin = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblLogin = new Label();
        lblPassword = new Label();
        txtLogin = new TextBox();
        txtPassword = new TextBox();
        btnLogin = new Button();

        // lblTitle
        lblTitle.Text = "Вход в систему ресторана";
        lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(40, 30);

        // lblLogin
        lblLogin.Text = "Логин:";
        lblLogin.AutoSize = true;
        lblLogin.Location = new Point(40, 90);

        // txtLogin
        txtLogin.Location = new Point(120, 87);
        txtLogin.Width = 200;

        // lblPassword
        lblPassword.Text = "Пароль:";
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(40, 130);

        // txtPassword
        txtPassword.Location = new Point(120, 127);
        txtPassword.Width = 200;
        txtPassword.UseSystemPasswordChar = true;

        // btnLogin
        btnLogin.Text = "Войти";
        btnLogin.Location = new Point(120, 170);
        btnLogin.Width = 100;
        btnLogin.Click += btnLogin_Click;

        // LoginForm
        ClientSize = new Size(380, 240);
        Text = "Ресторан — вход";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(lblTitle);
        Controls.Add(lblLogin);
        Controls.Add(txtLogin);
        Controls.Add(lblPassword);
        Controls.Add(txtPassword);
        Controls.Add(btnLogin);
    }
}
