# Guía de trabajo en equipo (Git)

Este proyecto se trabaja con **una rama principal (`main`) y cuatro ramas de desarrollo**, una por integrante. Nadie sube cambios directo a `main` — todo pasa primero por la rama personal de cada quien y luego se integra mediante un Pull Request.

## Ramas del repositorio

| Rama | Responsable |
|---|---|
| `main` | Rama principal. Solo recibe código ya revisado y funcionando. |
| `alejandra` | Rama de trabajo de Alejandra. |
| `angie` | Rama de trabajo de Angie. |
| `rafa` | Rama de trabajo de Rafa. |
| `sergio` | Rama de trabajo de Sergio. |

## Flujo de trabajo básico

1. **Antes de empezar a trabajar cada día**, actualiza tu rama con lo último de `main`:
   ```bash
   git checkout main
   git pull origin main
   git checkout tu-rama
   git merge main
   ```

2. **Trabaja normalmente en tu rama** y ve haciendo commits pequeños y descriptivos:
   ```bash
   git add .
   git commit -m "Agrega formulario de login"
   ```

3. **Sube tus cambios a tu rama en GitHub** (no a `main`):
   ```bash
   git push origin tu-rama
   ```

4. **Cuando una funcionalidad esté lista**, abre un **Pull Request** desde tu rama hacia `main` en GitHub. Describe brevemente qué hiciste y qué historia de usuario cubre.

5. **Espera revisión de al menos un compañero** antes de fusionar (esto evita que se rompa algo que otro ya tenía funcionando). Una vez aprobado, se fusiona el Pull Request a `main`.

6. Después de que tu Pull Request se fusione, **vuelve al paso 1** para actualizar tu rama con los cambios más recientes de `main` antes de seguir trabajando.

## Buenas prácticas

- **Un commit, un cambio claro.** Evita commits gigantes que mezclen varias cosas distintas.
- **Nombres de commit descriptivos**, en español o inglés pero consistente para todo el equipo. Ejemplos:
  - `Agrega validación de formulario de registro`
  - `Corrige bug en cálculo de total del carrito`
- **Nunca subas el archivo `.env`** ni ninguna contraseña o llave secreta al repositorio (ver `.gitignore`).
- Si dos personas necesitan tocar el mismo archivo al mismo tiempo, avisen en el chat del equipo antes de empezar, para minimizar conflictos de fusión (merge conflicts).
- Si tienen dudas sobre a qué rama le corresponde una tarea, revisen el Product Backlog priorizado y el cronograma en `docs/`.

## Configuración inicial (una sola vez, cada integrante)

```bash
git clone <URL-del-repositorio>
cd capys-bakery
git checkout tu-rama
```

Requisitos para correr el proyecto localmente:
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022+ (recomendado) o Visual Studio Code con la extensión **C# Dev Kit**
- SQL Server Express 2025 instalado localmente, o acceso a la instancia compartida que el equipo defina
- Cadena de conexión local en `appsettings.Development.json` (nunca se sube al repositorio — ya está en `.gitignore`)

```bash
cd src/CapysBakery.Web
dotnet restore
dotnet run
```

## Notas específicas de C# / .NET para evitar conflictos

- **No subir las carpetas `bin/` ni `obj/`** — se generan automáticamente al compilar y ya están en `.gitignore`. Si por error alguien las sube, hay que eliminarlas del repositorio (`git rm -r --cached bin obj`) y volver a commitear.
- **El archivo `.csproj` sí se versiona** (a diferencia de `bin`/`obj`), ya que ahí quedan registrados los paquetes NuGet que el proyecto necesita. Si agregas un paquete nuevo (`dotnet add package ...`), asegúrate de subir el `.csproj` actualizado.
- Si dos personas agregan paquetes NuGet distintos casi al mismo tiempo, es común que el `.csproj` tenga conflictos de fusión sencillos de resolver a mano (solo hay que conservar ambas líneas de `<PackageReference>`).
