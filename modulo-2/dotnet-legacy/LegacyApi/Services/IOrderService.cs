namespace LegacyApi.Services;

using LegacyApi.Domain;
using LegacyApi.Core;

public interface IOrderService
{
    /// <summary>
    /// Crea una nueva orden con validaciones y notificaciones
    /// </summary>
    Task<Result<Order>> CreateOrderAsync(string customerEmail, decimal total);

    /// <summary>
    /// Obtiene una orden por su ID
    /// </summary>
    Task<Result<Order>> GetOrderAsync(int id);

    /// <summary>
    /// Obtiene todas las órdenes de un cliente
    /// </summary>
    Task<Result<IEnumerable<Order>>> GetCustomerOrdersAsync(string email);
}
