using ApiClientes.DTOs;
using ApiClientes.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiClientes.API.Controllers;

/// <summary>
/// Expone operaciones de consulta sobre clientes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene un cliente a partir de su numero de identificacion.
    /// </summary>
    /// <param name="identificacion">Numero de identificacion del cliente.</param>
    /// <returns>Los datos del cliente si existe.</returns>
    /// <response code="200">Cliente encontrado.</response>
    /// <response code="400">La identificacion enviada no es valida.</response>
    /// <response code="404">No existe un cliente con la identificacion indicada.</response>
    [HttpGet("{identificacion}")]
    [ProducesResponseType(typeof(ClienteDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ClienteDTO>> ObtenerPorIdentificacion(string identificacion)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            return BadRequest(new { mensaje = "Debe indicar una identificacion valida." });
        }

        try
        {
            var cliente = await _clienteService.BuscarClientePorIdentificacionAsync(identificacion);

            if (cliente is null)
            {
                _logger.LogInformation("No se encontro cliente con identificacion {Identificacion}.", identificacion);
                return NotFound(new { mensaje = $"No se encontro un cliente con identificacion '{identificacion}'." });
            }

            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar cliente con identificacion {Identificacion}.", identificacion);
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "Ocurrio un error al procesar la solicitud." });
        }
    }
}
