namespace LegacyApi.Services;

using LegacyApi.Domain;
using LegacyApi.Repositories;
using LegacyApi.Core;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Order>> CreateOrderAsync(string customerEmail, decimal total)
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(customerEmail))
            return Result<Order>.Fail("El email del cliente es requerido");

        if (total < 0)
            return Result<Order>.Fail("El total no puede ser negativo");

        try
        {
            // Crear la orden
            var order = new Order
            {
                CustomerEmail = customerEmail,
                Total = total
            };

            var createdOrder = await _repository.CreateAsync(order);

            // Notificación (lógica de dominio)
            NotifyCustomer(customerEmail, createdOrder.Id);

            return Result<Order>.Ok(createdOrder, "Orden creada exitosamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear orden: {ex.Message}");
            return Result<Order>.Fail($"Error al crear la orden: {ex.Message}");
        }
    }

    public async Task<Result<Order>> GetOrderAsync(int id)
    {
        try
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
                return Result<Order>.Fail($"Orden con ID {id} no encontrada");

            return Result<Order>.Ok(order, "Orden encontrada");
        }
        catch (Exception ex)
        {
            return Result<Order>.Fail($"Error al obtener la orden: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Order>>> GetCustomerOrdersAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<IEnumerable<Order>>.Fail("El email es requerido");

            var orders = await _repository.GetByCustomerEmailAsync(email);

            return Result<IEnumerable<Order>>.Ok(orders, "Órdenes obtenidas exitosamente");
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Order>>.Fail($"Error al obtener órdenes: {ex.Message}");
        }
    }

    /// <summary>
    /// Lógica privada de notificación al cliente
    /// </summary>
    private void NotifyCustomer(string email, int orderId)
    {
        // Simulación de envío de email
        Console.WriteLine($"📧 Enviando email de confirmación a {email}");
        Console.WriteLine($"   Orden ID: {orderId}");
        Console.WriteLine($"   [Email enviado exitosamente]");
    }
}
