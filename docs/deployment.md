# Deployment

## Local dev stack (Docker Compose)

```bash
docker compose up --build          # from the repo root
docker compose down                # stop
docker compose down -v             # stop + wipe the DB/upload volumes
```

Services:

| Service | Image | Port | Notes |
|---|---|---|---|
| `db`  | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | `sa` / `Your_strong_Pass123`, data in the `mssql-data` volume |
| `api` | built from `backend/Dockerfile` | 8080 | waits for the DB healthcheck, migrates + seeds on startup |

The API applies EF Core migrations on startup (with a bounded retry while the DB finishes booting)
and seeds RBAC + dev data (Development environment).

## Configuration

Configuration binds from `appsettings.json`, environment-specific overrides, and environment
variables (double-underscore = nested key, e.g. `ConnectionStrings__DefaultConnection`).

| Setting | Env var | Purpose |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` | SQL Server connection. Windows Auth on the host; **SQL auth** in containers. |
| `Jwt:SigningKey` | `Jwt__SigningKey` | JWT signing secret (≥32 chars). **Set a real secret per environment.** |
| `Jwt:Issuer` / `Jwt:Audience` | `Jwt__Issuer` / `Jwt__Audience` | Token issuer/audience. |
| `Cors:AllowedOrigins` | `Cors__AllowedOrigins__0`, `__1`, … | Allowed origins. **Required in non-Development** (empty = no cross-origin access; the permissive fallback is Development-only). |
| `FileStorage:RootPath` | `FileStorage__RootPath` | Upload directory (mount a volume to persist). |
| `FileStorage:MaxFileSizeBytes` | `FileStorage__MaxFileSizeBytes` | Upload size cap (default 25 MB). |
| `FileStorage:AllowedContentTypes` | `FileStorage__AllowedContentTypes__0`, … | MIME allow-list (empty = allow any; **populate in production**). |
| `Fcm:CredentialsPath` / `Fcm:ProjectId` | `Fcm__*` | Firebase push (optional; logs when unset). |
| `Smtp:*` | `Smtp__*` | Email delivery (optional; logs when unset). |

## Production checklist

- [ ] Real `Jwt:SigningKey` from a secret store (never the committed placeholder).
- [ ] SQL auth (or managed identity) connection string; do not use the `sa` dev password.
- [ ] `Cors:AllowedOrigins` set to the real client origin(s).
- [ ] `FileStorage:AllowedContentTypes` populated; uploads on a persistent/backed volume.
- [ ] `ASPNETCORE_ENVIRONMENT=Production` (disables Swagger + dev seed; keeps rate limiting on).
- [ ] TLS terminated at a reverse proxy / load balancer in front of the API (HTTP inside the container).
- [ ] Run migrations deliberately for production (the startup auto-migrate is convenient for
      dev/compose; gate or replace it with a controlled migration step for prod).
- [ ] Log sink / retention configured (Serilog writes console + rolling files by default).

## Health & observability

- `GET /health` — liveness + DB connectivity (used by orchestrators).
- Structured logs via Serilog with a per-request `CorrelationId` (console + `logs/log-*.txt`).
- Security response headers and per-IP rate limiting are enabled outside the test host.
