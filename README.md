# MotorsportERP

Monorepo for Motorsport ERP with frontend, backend and Docker infrastructure.

## Docs

- Frontend guide: [`apps/frontend/README.md`](apps/frontend/README.md)
- Backend guide: [`apps/backend/README.md`](apps/backend/README.md)

## Docker quick start

Before first run or after env files update, initialize env file from template:
```bat
scripts\initialize.bat
```
This script creates `.env` from `.env.dev` (if `.env` does not exist yet).

Rebuild images and start (Dockerfile/dependencies/structure changed or first start):
```bash
docker compose pull
docker compose build
docker compose up -d
```

Start stack:
```bash
docker compose up -d
```

## Docker run modes

- Full stack: `docker compose up -d`
- API + DB only (local front): `docker compose up -d api db`
- Front + DB only (local back): `docker compose up -d frontend db`
- DB only (local back and front): `docker compose up -d db`

## Useful commands

- Stop: `docker compose down`
- Stop and remove volumes (DB wipe): `docker compose down -v`