# Prueba Talycap — Consulta de Clientes

Solución fullstack containerizada con **.NET 6** (API REST en capas), **Angular 15** (SPA con Bootstrap 5) y **SQL Server 2019**, orquestada con Docker Compose.

## Arquitectura

```
proyecto-fullstack/
├── backend/
│   ├── ApiClientes.sln
│   ├── ApiClientes.API/            Capa de presentación (Controllers, Program.cs, Swagger)
│   ├── ApiClientes.Services/       Capa de negocio
│   ├── ApiClientes.Repositories/   Capa de datos (EF Core, DbContext, acceso al SP)
│   ├── ApiClientes.DTOs/           Objetos de transferencia de datos
│   ├── ApiClientes.Models/         Entidades de dominio
│   ├── ApiClientes.Database/       Script de inicialización (tabla, SP, datos de prueba)
│   ├── Dockerfile                  Build multi-stage (SDK -> runtime ASP.NET)
│   └── .dockerignore
├── frontend/
│   ├── src/app/
│   │   ├── components/buscar-cliente/
│   │   ├── services/cliente.service.ts
│   │   └── models/cliente.model.ts
│   ├── Dockerfile                  Build multi-stage (Node -> nginx)
│   ├── nginx.conf                  Sirve la SPA y hace proxy de /api hacia el backend
│   └── .dockerignore
├── docker-compose.yml
├── .env                            Credenciales de desarrollo (SA_PASSWORD)
└── README.md
```

## Requisitos previos

- [Docker](https://www.docker.com/products/docker-desktop/)
- [Docker Compose](https://docs.docker.com/compose/) (incluido en Docker Desktop)

Opcional, solo si quieres ejecutar cada parte fuera de Docker:

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js 18+](https://nodejs.org/) y npm

## Para levantar la solución

Desde la raíz del repositorio:

```bash
docker-compose up --build
```

Esto construye y levanta 3 contenedores:

| Servicio    | Descripción                              | Puerto host |
|-------------|-------------------------------------------|-------------|
| `sqlserver` | SQL Server 2019 con la base `DBClientes`  | 1433        |
| `backend`   | API REST en .NET 6                        | 5000        |
| `frontend`  | SPA Angular servida por nginx              | 4200        |

El `backend` espera activamente a que `sqlserver` responda (healthcheck en Compose + reintentos internos en `Program.cs`), crea la base de datos `DBClientes` si no existe y ejecuta el script `ApiClientes.Database/init.sql` (tabla `Clientes`, stored procedure `sp_ObtenerClientePorIdentificacion` y datos de prueba). Todo esto es idempotente: puedes reiniciar los contenedores sin duplicar datos ni errores.

> La primera vez, SQL Server puede tardar 30-60 segundos en estar listo. El backend reintentará automáticamente hasta 15 veces (cada 5s) antes de darse por vencido.

## URLs de acceso

- **Frontend**: http://localhost:4200
- **API**: http://localhost:5000/api/clientes/{identificacion}
- **Swagger**: http://localhost:5000/swagger

## Pasos para verificar

1. Abre http://localhost:4200 en el navegador.
2. Ingresa una identificación válida, por ejemplo `12345678`, y presiona **Buscar**.
   - Deberías ver Nombre, Apellido, Email y Fecha de Creación del cliente.
3. Ingresa una identificación inexistente, por ejemplo `00000000`.
   - Debería aparecer un mensaje de error indicando que el cliente no existe.
4. Intenta buscar sin ingresar nada: el botón valida el campo y muestra un mensaje sin llamar a la API.

Datos de prueba precargados (`Identificacion` → `Nombre Apellido`):

| Identificación | Nombre  | Apellido  | Email                          |
|-----------------|---------|-----------|---------------------------------|
| 12345678         | Juan    | Perez     | juan.perez@example.com          |
| 87654321         | Maria   | Gonzalez  | maria.gonzalez@example.com      |
| 11223344         | Carlos  | Ramirez   | carlos.ramirez@example.com      |
| 99887766         | Ana     | Torres    | ana.torres@example.com          |

## Para detener

```bash
docker-compose down
```

Para eliminar también el volumen de datos de SQL Server (borra la base de datos):

```bash
docker-compose down -v
```

## Logs

```bash
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f sqlserver
```

## Desarrollo fuera de Docker (opcional)

### Backend

```bash
cd backend
dotnet build
cd ApiClientes.API
dotnet run
```

Configura la variable de entorno `ConnectionStrings__DefaultConnection` (o edita `appsettings.json`) apuntando a tu instancia local de SQL Server. La API queda disponible en `http://localhost:5000` y Swagger en `http://localhost:5000/swagger`.

### Frontend

```bash
cd frontend
npm install
npm start
```

`npm start` ejecuta `ng serve --host 0.0.0.0` (puerto 4200 por defecto) y usa `src/environments/environment.ts`, que apunta directamente a `http://localhost:5000/api`.

## Notas técnicas

- **Comunicación entre contenedores**: dentro de la red de Docker, `backend` se conecta a `sqlserver` por nombre de servicio, y `frontend` (nginx) hace *proxy* de `/api/` hacia `http://backend:5000/api/` (variable `URL_API`, inyectada en `nginx.conf` vía `envsubst` al iniciar el contenedor). Como el navegador del usuario no forma parte de la red interna de Docker, la SPA nunca llama a `http://backend:5000` directamente: llama a `/api/...` sobre su propio origen (`http://localhost:4200`), y es nginx quien reenvía la petición al backend dentro de la red de contenedores.
- **CORS**: habilitado en `Program.cs` para el origen `http://localhost:4200` (métodos `GET`, `POST`, `OPTIONS`), útil para ejecutar el frontend con `ng serve` contra la API sin pasar por nginx.
- **Reintentos de conexión a la base de datos**: `EnableRetryOnFailure` en EF Core (nivel de conexión/transacción) + un bucle de reintentos explícito en `Program.cs` antes de ejecutar el script de inicialización (nivel de arranque de la app).
- **Stored Procedure**: `sp_ObtenerClientePorIdentificacion` se invoca desde `ApiClientes.Repositories` con `FromSqlRaw` de Entity Framework Core.
- **Swagger**: documentación automática habilitada en `http://localhost:5000/swagger`, incluyendo comentarios XML de los endpoints.
