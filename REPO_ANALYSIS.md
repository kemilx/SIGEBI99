# Visión general del repositorio SIGEBI

El repositorio contiene una solución ASP.NET Core orientada a la gestión de biblioteca (SIGEBI) con varios proyectos separados por capas. A continuación se resume cada componente y cómo se integran.

## Proyectos principales
- **SIGEBI.Api**: API REST que expone endpoints para libros, préstamos, notificaciones y penalizaciones. Configura Swagger, validación FluentValidation y una comprobación de creación de base de datos al arrancar para entornos relacionales, además de un middleware de manejo de excepciones. 【F:SIGEBI.Api/Program.cs†L1-L62】
- **SIGEBI.Application**: capa de aplicación que registra servicios y validadores para operaciones de préstamos, libros, usuarios, reportes y penalizaciones. La composición se hace mediante inyección de dependencias. 【F:SIGEBI.Application/DependencyInjection.cs†L1-L30】
- **SIGEBI.Domain**: define entidades y reglas de negocio. Por ejemplo, `Libro` valida longitudes de campos y controla estados (disponible, reservado, prestado, dañado, inactivo) junto con el conteo de ejemplares disponibles. 【F:SIGEBI.Domain/Entities/Libro.cs†L7-L200】
- **SIGEBI.Persistence**: implementación de acceso a datos con Entity Framework Core. El `SIGEBIDbContext` expone conjuntos para usuarios, roles, libros, préstamos, penalizaciones, notificaciones y configuración, aplicando configuraciones específicas por entidad. Incluye repositorios como `LibroRepository` para consultas y mutaciones comunes. 【F:SIGEBI.Persistence/DbContext/SIGEBIDbContext.cs†L1-L35】【F:SIGEBI.Persistence/Repositories/LibroRepository.cs†L1-L66】
- **SIGEBI.IOC**: módulo de infraestructura para registro centralizado de dependencias. Configura el `SIGEBIDbContext` con SQL Server o InMemory según la cadena de conexión y registra todos los repositorios antes de vincular la capa de aplicación. 【F:SIGEBI.IOC/DependencyInjection.cs†L1-L44】
- **SIGEBI.Web**: interfaz MVC/Razor con autenticación Identity. Inicializa su propio `ApplicationDbContext` (SQL Server o InMemory) y reutiliza las dependencias del núcleo SIGEBI para servir vistas y controladores web. 【F:SIGEBI.Web/Program.cs†L1-L54】

## API de ejemplo
El `LibroController` muestra el patrón típico de los controladores: delega en `ILibroService` para obtener, buscar, crear y actualizar libros, devolviendo DTOs mapeados desde la entidad de dominio. 【F:SIGEBI.Api/Controllers/LibroController.cs†L1-L87】

## Flujo de composición
El punto de entrada de la API y del front-end web llama a `AddSIGEBIDependencies`, que primero registra la persistencia y luego la capa de aplicación, permitiendo que servicios y controladores consuman repositorios configurados para SQL Server o una base en memoria para desarrollo/pruebas. 【F:SIGEBI.Api/Program.cs†L9-L36】【F:SIGEBI.Web/Program.cs†L6-L54】【F:SIGEBI.IOC/DependencyInjection.cs†L13-L44】
