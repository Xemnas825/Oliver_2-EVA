# Contexto del proyecto (resumen de conversación)

Resumen para retomar el proyecto desde otro PC sin depender del historial de chat de Cursor.

---

## Estado del proyecto

- **Backend:** .NET 8, ASP.NET Core, SQLite, JWT, Swagger. Listo y con tests.
- **Frontend:** Vue 3 + Vite + TypeScript + Pinia + Vue Router + **bootstrap-vue-next** (no bootstrap-vue-3). Feature **feature/setup** hecha.

---

## Ramas (GitFlow)

| Rama | Uso |
|------|-----|
| `main` | Código estable / entrega |
| `develop` | Integración; aquí se fusionan las feature |
| `feature/setup` | ✅ Hecha (Vue, Router, Pinia, Bootstrap, estructura) |
| `feature/layouts` | Siguiente: LayoutAuth, LayoutPublic, LayoutAdmin + Header/Footer (público y admin) |
| `feature/auth` | Login, registro, store auth, guards |
| `feature/games` | Listado + CRUD juegos |
| `feature/characters` | Listado + CRUD personajes |
| `feature/home` | Página principal pública |
| `feature/admin` | Pantalla administración |

---

## Cómo arrancar

**Backend (Docker):**
```bash
docker compose build && docker compose up
```
- API + Swagger: http://localhost:8080 (puerto 8080 porque en Mac el 5000 lo usa AirPlay)

**Frontend:**
```bash
cd frontend
npm install
npm run dev
```
- App: http://localhost:5173  
- `.env` tiene `VITE_API_URL=http://localhost:8080`

**Credenciales prueba:** admin@wiki.com / admin123 (admin), usuario@wiki.com / usuario123 (user).

---

## Qué toca en feature/layouts

1. **Tres layouts:** LayoutAuth (sin header/footer para login y registro), LayoutPublic (Header + Footer), LayoutAdmin (HeaderAdmin + FooterAdmin).
2. **Componentes:** Header, Footer (público), HeaderAdmin, FooterAdmin. Header con sección actual (resaltado o breadcrumbs).
3. **Router:** Asignar cada ruta a un layout vía `meta.layout`. Vistas mínimas: Login, Registro, Admin (placeholder).
4. HomeView ya existe; usará LayoutPublic.

---

## Cambios importantes que ya están hechos

- Backend: BCrypt como `BCrypt.Net.BCrypt` (no solo `BCrypt`). JWT con claims `sub` y `role`. Swagger en la raíz (http://localhost:8080).
- Frontend: **bootstrap-vue-next** con setup **BApp** en App.vue (no bootstrap-vue-3, está deprecado). Excluir carpeta `Tests` en Backend.csproj para que compile solo el API.

---

## Tests backend (sin .NET instalado)

```bash
cd backend
docker run --rm -v "$(pwd):/src" -w /src mcr.microsoft.com/dotnet/sdk:8.0 dotnet test WikiVideojuegos.sln
```

---

Cuando sigas desde el otro PC: abre este archivo, haz `git pull`, y continúa por la rama que toque (por ejemplo `feature/layouts`).
