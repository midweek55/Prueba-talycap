namespace ApiClientes.DTOs;

/// <summary>
/// Datos de un cliente expuestos por la API.
/// </summary>
public class ClienteDTO
{
    public int IdCliente { get; set; }

    public string Identificacion { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string? Email { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }
}
