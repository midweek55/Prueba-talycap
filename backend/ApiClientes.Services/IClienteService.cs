using ApiClientes.DTOs;

namespace ApiClientes.Services;

/// <summary>
/// Contrato de la logica de negocio relacionada con clientes.
/// </summary>
public interface IClienteService
{
    /// <summary>
    /// Busca un cliente por su numero de identificacion.
    /// </summary>
    /// <returns>El DTO del cliente encontrado, o null si no existe.</returns>
    Task<ClienteDTO?> BuscarClientePorIdentificacionAsync(string identificacion);
}
