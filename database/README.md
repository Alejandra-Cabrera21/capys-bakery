# Base de datos

- **Desarrollo:** SQL Server Express 2025, local en la máquina de cada integrante.
- **Producción:** MSSQL 2025 provisto por el hosting (MonsterASP.NET).

Aquí se documentará:
- El diagrama entidad-relación (ER) del modelo de datos.
- Script de creación del esquema (`schema.sql`) y/o migraciones de Entity Framework Core.
- Datos semilla (seed) para pruebas: productos de ejemplo, usuarios de prueba por rol.
- Procedimientos almacenados, si el equipo decide usar el enfoque Repository + Stored Procedures en vez de EF Core puro.

## Cadena de conexión

Cada uno tiene que configura su propia cadena de conexión local en `appsettings.Development.json` (no se sube al repositorio, veanlo en `.gitignore` en la raíz). Ejemplo de formato para SQL Server Express local:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CapysBakeryDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

La cadena de conexión de producción (MonsterASP.NET) se obtiene desde el panel de control de cada uno y se configura como variable de entorno o en `appsettings.Production.json`, nunca en texto plano en el repositorio.
