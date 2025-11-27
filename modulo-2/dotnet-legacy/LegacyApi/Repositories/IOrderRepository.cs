namespace LegacyApi.Repositories;

using LegacyApi.Domain;

public interface IOrderRepository
{
    /// <summary>
    /// Crea una nueva orden en el repositorio
    /// </summary>
    Task<Order> CreateAsync(Order order);

    /// <summary>
    /// Obtiene una orden por su ID
    /// </summary>
    Task<Order?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene todas las órdenes de un cliente
    /// </summary>
    Task<IEnumerable<Order>> GetByCustomerEmailAsync(string email);

    /// <summary>
    /// Actualiza una orden existente
    /// </summary>
    Task<bool> UpdateAsync(Order order);

    /// <summary>
    /// Elimina una orden
    /// </summary>
    Task<bool> DeleteAsync(int id);
}
