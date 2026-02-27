# Contexto del proyecto (resumen de conversación)

Resumen para retomar el proyecto desde otro PC sin depender del historial de chat de Cursor.

---

## Estado del proyecto

- **Backend:** .NET 8, ASP.NET Core, SQLite, JWT, Swagger. API en `backend/` (Program.cs, DTOs, Models, AppDbContext). **Sin carpeta Tests** (se eliminó; solo queda el proyecto API). Dockerfile publica explícitamente `Backend.csproj`.
- **Frontend:** Vue 3 + Vite + TypeScript + Pinia + Vue Router + **bootstrap-vue-next** (no bootstrap-vue-3). Estructura: `src/views`, `components`, `layouts`, `stores`, `services`, `types`.

---

## Ramas (GitFlow)

| Rama | Estado |
|------|--------|
| `main` | Código estable / entrega |
| `develop` | Integración; aquí se fusionan las feature |
| `feature/setup` | ✅ Hecha (Vue, Router, Pinia, Bootstrap, estructura) |
| `feature/layouts` | ✅ Hecha (LayoutAuth, LayoutPublic, LayoutAdmin, Header/Footer) |
| `feature/auth` | ✅ Hecha (login, registro, store auth, guards, VeeValidate+Yup) |
| `feature/games` | ✅ Hecha (store games, GameCard, GamesListView, GameDetailView, AdminGamesView CRUD, GameForm, seed 6 juegos) |
| `feature/characters` | Pendiente |
| `feature/home` | Pendiente |

---

## Cómo arrancar

**Backend (Docker):**
```bash
docker compose build && docker compose up
```
- API + Swagger: http://localhost:8080

**Frontend:**
```bash
cd frontend
npm install
npm run dev
```
- App: http://localhost:5173  
- `.env`: `VITE_API_URL=` vacío para usar el proxy de Vite a `/api` → backend en 8080 (`vite.config.ts`).

**Credenciales prueba:** admin@wiki.com / admin123 (admin), usuario@wiki.com / usuario123 (user).

---

## Rutas y archivos relevantes

**Backend:** `backend/Program.cs`, `backend/DTOs/GameDtos.cs`, `backend/Models/Game.cs`, `backend/Data/AppDbContext.cs`. Seed de usuarios (admin + usuario) y de **6 juegos** al arrancar (se añaden por nombre si no existen: Zelda BOTW, Elden Ring, Red Dead 2, God of War Ragnarök, Hollow Knight, Celeste).

**Frontend:**  
- Router: `frontend/src/router/index.ts`  
- Stores: `auth.ts`, `games.ts`  
- Vistas: LoginView, RegisterView, GamesListView, GameDetailView, AdminGamesView  
- Componentes: AppHeader, GameCard, GameForm  
- API: `frontend/src/services/api.ts` (axios, baseURL, interceptor 401)

---

## Detalles ya resueltos

- **BCrypt:** usar `BCrypt.Net.BCrypt.HashPassword` / `Verify` en backend.
- **Bootstrap:** bootstrap-vue-next con `BApp` en App.vue (no bootstrap-vue-3).
- **Formularios:** VeeValidate + Yup, `<form>` nativo con `@submit.prevent`; errores con BAlert (no alerts nativos). Confirmaciones con BModal (no `confirm()` nativo).
- **Docker:** en el Dockerfile usar `dotnet publish Backend.csproj` porque en la carpeta ya no hay solución con varios proyectos.
- **GameCard:** solo muestra imagen si `imageUrl` es una URL válida (http/https) para evitar icono roto.

---

## Próximos pasos posibles

- Feature **characters**: store, listado público, detalle, AdminCharactersView CRUD.
- Feature **home**: página principal pública.
- Actualizar este archivo cuando se cierre una rama o se añadan features.

---

Cuando sigas desde otro PC: abre este archivo, haz `git pull`, y continúa por la rama que toque (por ejemplo `feature/characters`).
