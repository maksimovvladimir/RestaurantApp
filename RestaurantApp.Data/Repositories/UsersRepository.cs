using Npgsql;
using RestaurantApp.Data.Models;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantApp.Data.Repositories;

public class UsersRepository
{
    /// <summary>Простой SHA-256 хеш пароля (для учебного проекта достаточно; для продакшена нужен bcrypt/argon2).</summary>
    public static string HashPassword(string plainPassword)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainPassword));
        return Convert.ToHexString(bytes);
    }

    /// <summary>Возвращает пользователя, если логин/пароль верны, иначе null.</summary>
    public UserAccount? TryLogin(string login, string plainPassword)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            "SELECT id, login, password_hash, role, full_name, phone FROM users WHERE login = @login", conn);
        cmd.Parameters.AddWithValue("login", login);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        var storedHash = reader.GetString(2);
        if (storedHash != HashPassword(plainPassword)) return null;

        return new UserAccount
        {
            Id = reader.GetInt32(0),
            Login = reader.GetString(1),
            PasswordHash = storedHash,
            Role = reader.GetString(3),
            FullName = reader.GetString(4),
            Phone = reader.IsDBNull(5) ? null : reader.GetString(5)
        };
    }

    public void CreateUser(string login, string plainPassword, string role, string fullName, string? phone)
    {
        using var conn = DbConfig.CreateConnection();
        using var cmd = new NpgsqlCommand(
            @"INSERT INTO users (login, password_hash, role, full_name, phone)
              VALUES (@login, @hash, @role, @fullName, @phone)", conn);
        cmd.Parameters.AddWithValue("login", login);
        cmd.Parameters.AddWithValue("hash", HashPassword(plainPassword));
        cmd.Parameters.AddWithValue("role", role);
        cmd.Parameters.AddWithValue("fullName", fullName);
        cmd.Parameters.AddWithValue("phone", phone ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }
}
