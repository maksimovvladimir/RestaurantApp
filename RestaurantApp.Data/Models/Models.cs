namespace RestaurantApp.Data.Models;

public class UserAccount
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = ""; // waiter | kitchen | admin
    public string FullName { get; set; } = "";
    public string? Phone { get; set; }
}

public class TableInfo
{
    public int IdTable { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = "free";
}

public class Shift
{
    public int Id { get; set; }
    public int WaiterId { get; set; }
    public DateTime ShiftDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string Status { get; set; } = "open";
}

public class Reservation
{
    public int IdReserve { get; set; }
    public string ClientName { get; set; } = "";
    public string? ClientPhone { get; set; }
    public DateTime ResDate { get; set; }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public int GuestsCount { get; set; }
    public string Status { get; set; } = "active";
}

public class ReservationTable
{
    public int IdReserveTable { get; set; }
    public int IdReserve { get; set; }
    public int IdTable { get; set; }
}

public class DishCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Dish
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string? Description { get; set; }
}

public class DishCurrentPrice
{
    public int DishId { get; set; }
    public string Name { get; set; } = "";
    public decimal BasePrice { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal CurrentPrice { get; set; }
}

public class StockItem
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public int QuantityAvalible { get; set; } // имя поля сохранено как в БД (опечатка по диаграмме)
}

public class Discount
{
    public int DishId { get; set; }
    public string Name { get; set; } = "";
    public decimal DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int IdReserveTable { get; set; }
    public string Status { get; set; } = "draft";
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public decimal TotalAmount { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int DishId { get; set; }
    public int Quantity { get; set; }
}

public class Receipt
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public DateTime PaidAt { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethoc { get; set; } = "cash"; // имя поля сохранено как в БД
}
