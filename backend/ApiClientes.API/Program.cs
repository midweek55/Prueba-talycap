using System.Text.RegularExpressions;
using ApiClientes.Repositories;
using ApiClientes.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

const string AngularCorsPolicy = "AngularCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontro la cadena de conexion 'DefaultConnection'.");

builder.Services.AddDbContext<ApiClientesDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptions =>
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .WithMethods("GET", "POST", "OPTIONS")
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ApiClientes",
        Version = "v1",
        Description = "API REST para la consulta de clientes por numero de identificacion."
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiClientes v1");
});

await EnsureDatabaseReadyAsync(app.Services, app.Logger, connectionString);

app.UseCors(AngularCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// Espera a que SQL Server este disponible (con reintentos), crea la base de datos DBClientes
/// si no existe y ejecuta el script de inicializacion (tabla, stored procedure y datos de prueba).
/// </summary>
static async Task EnsureDatabaseReadyAsync(IServiceProvider services, ILogger logger, string connectionString)
{
    const int maxAttempts = 15;
    var delay = TimeSpan.FromSeconds(5);

    var masterConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
    {
        InitialCatalog = "master"
    };
    var databaseName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            logger.LogInformation("Esperando a SQL Server (intento {Attempt}/{MaxAttempts})...", attempt, maxAttempts);

            using var masterConnection = new SqlConnection(masterConnectionStringBuilder.ConnectionString);
            await masterConnection.OpenAsync();

            using var createDbCommand = masterConnection.CreateCommand();
            createDbCommand.CommandText =
                $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}') CREATE DATABASE [{databaseName}];";
            await createDbCommand.ExecuteNonQueryAsync();

            logger.LogInformation("Base de datos '{DatabaseName}' verificada/creada correctamente.", databaseName);
            break;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(ex, "SQL Server no esta listo todavia. Reintentando en {Delay}s...", delay.TotalSeconds);
            await Task.Delay(delay);
        }
    }

    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApiClientesDbContext>();

    var scriptPath = Path.Combine(AppContext.BaseDirectory, "Database", "init.sql");
    if (!File.Exists(scriptPath))
    {
        logger.LogWarning("No se encontro el script de inicializacion en {Path}.", scriptPath);
        return;
    }

    var script = await File.ReadAllTextAsync(scriptPath);
    var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

    foreach (var batch in batches)
    {
        var trimmedBatch = batch.Trim();
        if (string.IsNullOrWhiteSpace(trimmedBatch))
        {
            continue;
        }

        await context.Database.ExecuteSqlRawAsync(trimmedBatch);
    }

    logger.LogInformation("Script de inicializacion de base de datos ejecutado correctamente.");
}
