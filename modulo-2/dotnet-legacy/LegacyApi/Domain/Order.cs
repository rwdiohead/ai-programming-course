namespace LegacyApi.Domain;

public class Order
{
    public int Id { get; set; }
    public required string CustomerEmail { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Failed = 2
}
