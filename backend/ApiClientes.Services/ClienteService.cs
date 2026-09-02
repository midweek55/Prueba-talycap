using ApiClientes.DTOs;
using ApiClientes.Repositories;

namespace ApiClientes.Services;

/// <summary>
/// Logica de negocio relacionada con clientes.
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDTO?> BuscarClientePorIdentificacionAsync(string identificacion)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            throw new ArgumentException("La identificacion es obligatoria.", nameof(identificacion));
        }

        var cliente = await _clienteRepository.ObtenerPorIdentificacionAsync(identificacion.Trim());

        if (cliente is null)
        {
            return null;
        }

        return new ClienteDTO
        {
            IdCliente = cliente.IdCliente,
            Identificacion = cliente.Identificacion,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            Email = cliente.Email,
            FechaCreacion = cliente.FechaCreacion,
            FechaActualizacion = cliente.FechaActualizacion
        };
    }
}
