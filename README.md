# Wiki de Videojuegos

Proyecto fullstack: backend en .NET 8 y frontend en Vue 3 (Wiki de videojuegos y personajes).

## Arrancar con Docker

```bash
docker-compose build && docker compose up
```

- **Backend API y Swagger:** http://localhost:8080 (Swagger se muestra en la página principal)  

**Base de datos:** SQLite. El fichero `wiki.db` se crea solo al arrancar (con tablas `Users`, `Games`, `Characters`). En Docker se persiste en el volumen `wiki-data`.

Cuando añadas el frontend, inclúyelo en `docker-compose.yml` y documenta aquí la URL (por ejemplo http://localhost:5173).

## Credenciales de prueba

| Rol   | Email             | Contraseña  |
|-------|-------------------|-------------|
| Admin | admin@wiki.com    | admin123    |
| User  | usuario@wiki.com  | usuario123  |

Solo el usuario **admin** puede crear, editar y eliminar juegos y personajes. El usuario normal puede registrarse, iniciar sesión y ver los listados.

## API (Backend)

- **Auth:** `POST /api/auth/register`, `POST /api/auth/login`, `GET /api/auth/me` (Bearer token)
- **Juegos:** `GET /api/games`, `GET /api/games/{id}`, `POST/PUT/DELETE /api/games/{id}` (admin)
- **Personajes:** `GET /api/characters?gameId= opcional`, `GET /api/characters/{id}`, `POST/PUT/DELETE /api/characters/{id}` (admin)

En peticiones de escritura (POST/PUT/DELETE) envía el header: `Authorization: Bearer <token>`.

## Tests del backend

Desde la carpeta `backend` (con .NET 8 SDK instalado):

```bash
cd backend
dotnet test
```

Se ejecutan tests de integración con xUnit y WebApplicationFactory (base de datos en memoria): auth (login, registro, /me), listados públicos y CRUD de juegos y personajes como admin.
