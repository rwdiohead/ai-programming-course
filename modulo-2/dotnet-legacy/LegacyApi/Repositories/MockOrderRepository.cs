namespace LegacyApi.Repositories;

using LegacyApi.Domain;

public class MockOrderRepository : IOrderRepository
{
    private static List<Order> _orders = new();
    private static int _nextId = 1;

    public Task<Order> CreateAsync(Order order)
    {
        order.Id = _nextId++;
        order.CreatedAt = DateTime.Now;
        order.Status = OrderStatus.Pending;
        _orders.Add(order);
        return Task.FromResult(order);
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        return Task.FromResult(order);
    }

    public Task<IEnumerable<Order>> GetByCustomerEmailAsync(string email)
    {
        var orders = _orders.Where(o => o.CustomerEmail == email).ToList();
        return Task.FromResult((IEnumerable<Order>)orders);
    }

    public Task<bool> UpdateAsync(Order order)
    {
        var existing = _orders.FirstOrDefault(o => o.Id == order.Id);
        if (existing == null) return Task.FromResult(false);

        existing.Total = order.Total;
        existing.Status = order.Status;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null) return Task.FromResult(false);

        _orders.Remove(order);
        return Task.FromResult(true);
    }
}
