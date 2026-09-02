# =============================================
# Etapa 1: build - compila la aplicacion Angular
# =============================================
FROM node:18-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration production

# =============================================
# Etapa 2: runtime - sirve los archivos estaticos con nginx
# =============================================
FROM nginx:1.25-alpine AS final

# Plantilla procesada automaticamente por el entrypoint de nginx (envsubst)
COPY nginx.conf /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist/frontend /usr/share/nginx/html

ENV URL_API=http://backend:5000

EXPOSE 4200

CMD ["nginx", "-g", "daemon off;"]
