# MyTodos

A simple, single-user to-do list application. Users can create, edit, complete, and delete tasks — no login or multi-user support. Built with a React + TypeScript frontend and a .NET Web API backend, backed by an in-memory data store (no database; data resets on backend restart).

Full requirements: [MyTodos-PRD.md](./MyTodos-PRD.md). Conventions: [CLAUDE.md](./CLAUDE.md).

## Current Status

Full CRUD is implemented end to end: create, list, view one, edit, toggle complete, and delete todos, on both the backend and the frontend. Backend covered by xUnit tests (`MyTodos.Api.Tests/`).

## Project Structure

- `MyTodos.Api/` — ASP.NET Core Web API (.NET 8), in-memory `TodoService` (`ConcurrentDictionary`, singleton), async CRUD endpoints, `Guid` ids, `DateOnly` due dates.
- `MyTodos.Api.Tests/` — xUnit tests for `TodoService` and `TodosController` (CRUD, title validation, 404 paths).
- `mytodos-frontend/` — React + TypeScript app (Vite), axios-based API client, full create/edit/complete/delete UI.

## Running the Backend

```
cd MyTodos.Api
dotnet run
```

Runs at `https://localhost:5001` (and `http://localhost:5000`) via the `https` launch profile. Swagger UI available at `/swagger` in development.

Run the tests from the repo root:

```
dotnet test
```

> Note: this machine has the **.NET 10 SDK** installed (no separate .NET 8 SDK), but both projects target **net8.0** as required — the SDK restores the net8.0 reference packs via NuGet and builds/runs/tests without issue.

## Running the Frontend

```
cd mytodos-frontend
npm install
npm run dev
```

Runs at `http://localhost:5173` and talks to `https://localhost:5001/api` (see `.env.development`, `VITE_API_BASE_URL`). Add a todo via the form at the top; click a row's checkbox to toggle complete, the pencil icon to edit, or the trash icon to delete (confirmation dialog). If the browser blocks the API calls with a certificate warning, run `dotnet dev-certs https --trust` once to trust the local ASP.NET Core dev certificate.

## API

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/todos` | Get all todos |
| GET | `/api/todos/{id}` | Get a single todo |
| POST | `/api/todos` | Create a todo (`title` required, `dueDate` optional) |
| PUT | `/api/todos/{id}` | Update a todo's `title`, `dueDate`, `isComplete` |
| DELETE | `/api/todos/{id}` | Delete a todo |

Title is required on create/update; an empty title returns `400`. A missing `id` on GET/PUT/DELETE returns `404`.
