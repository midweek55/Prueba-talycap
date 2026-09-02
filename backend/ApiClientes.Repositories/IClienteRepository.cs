using ApiClientes.Models;

namespace ApiClientes.Repositories;

/// <summary>
/// Contrato de acceso a datos para la entidad Cliente.
/// </summary>
public interface IClienteRepository
{
    /// <summary>
    /// Busca un cliente por su numero de identificacion invocando el stored procedure
    /// sp_ObtenerClientePorIdentificacion.
    /// </summary>
    Task<Cliente?> ObtenerPorIdentificacionAsync(string identificacion);
}
