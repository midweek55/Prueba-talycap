namespace ApiClientes.Models;

/// <summary>
/// Entidad de dominio que representa un cliente almacenado en la base de datos.
/// </summary>
public class Cliente
{
    public int IdCliente { get; set; }

    public string Identificacion { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string? Email { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }
}
