# 📦 Inventario API

API REST desarrollada en ASP.NET Core 8 con autenticación JWT y SQL Server.

## 🚀 Tecnologías
- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger / OpenAPI
- EPPlus (exportación Excel)

## 📋 Funcionalidades
- Autenticación con JWT y roles (Admin/Usuario)
- CRUD de Productos, Categorías y Proveedores
- Control de movimientos de stock (entradas y salidas)
- Alertas de productos bajo stock mínimo
- Exportación de reportes a Excel

## ⚙️ Configuración
1. Clona el repositorio
2. Copia `appsettings.example.json` y renómbralo `appsettings.json`
3. Configura tu servidor SQL Server y ejecuta el script de base de datos
4. Ejecuta el proyecto con Visual Studio 2022

## 📌 Endpoints principales
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | /api/auth/register | Registrar usuario |
| POST | /api/auth/login | Iniciar sesión |
| GET | /api/productos | Listar productos |
| GET | /api/productos/bajo-stock | Productos bajo mínimo |
| POST | /api/movimientos/entrada | Registrar entrada |
| POST | /api/movimientos/salida | Registrar salida |
| GET | /api/reportes/productos-excel | Exportar a Excel |
