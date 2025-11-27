using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace LegacyApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderRequest request)
    {
        if (request.Total < 0) return BadRequest("Total negativo");
        if (string.IsNullOrEmpty(request.CustomerEmail)) return BadRequest("Falta email");

        try 
        {
            using (var conn = new SqlConnection("Server=.;Database=ProdDB;Trusted_Connection=True;"))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Orders (Customer, Total, Date) VALUES (@p1, @p2, @p3)", conn);
                cmd.Parameters.AddWithValue("@p1", request.CustomerEmail);
                cmd.Parameters.AddWithValue("@p2", request.Total);
                cmd.Parameters.AddWithValue("@p3", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error DB: " + ex.Message);
            return StatusCode(500, "Error simulado para demo");
        }

        Console.WriteLine($"Enviando email de confirmación a {request.CustomerEmail}...");

        return Ok(new { Message = "Orden creada", Status = "Pending" });
    }
}

public class OrderRequest {
    public string CustomerEmail { get; set; }
    public decimal Total { get; set; }
}