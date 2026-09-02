using ApiClientes.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiClientes.Repositories;

/// <summary>
/// Implementacion de acceso a datos para Cliente basada en Entity Framework Core,
/// invocando el stored procedure sp_ObtenerClientePorIdentificacion.
/// </summary>
public class ClienteRepository : IClienteRepository
{
    private readonly ApiClientesDbContext _context;

    public ClienteRepository(ApiClientesDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObtenerPorIdentificacionAsync(string identificacion)
    {
        var parametro = new SqlParameter("@Identificacion", identificacion);

        // Los stored procedures no son componibles por EF Core (no admiten TOP/WHERE adicional),
        // por lo que se materializa la lista completa y se toma el primer resultado en memoria.
        var resultado = await _context.Clientes
            .FromSqlRaw("EXEC sp_ObtenerClientePorIdentificacion @Identificacion", parametro)
            .AsNoTracking()
            .ToListAsync();

        return resultado.FirstOrDefault();
    }
}
