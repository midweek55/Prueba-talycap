// Configuracion usada por el build de produccion (servido por nginx dentro de Docker).
// La ruta es relativa: nginx.conf hace proxy_pass de /api/ hacia el servicio "backend".
export const environment = {
  production: true,
  apiUrl: '/api'
};
