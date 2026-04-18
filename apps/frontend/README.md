# Motorsport ERP Frontend

Frontend application for Motorsport ERP built with React, TypeScript and Vite.

## Stack

- React 19
- TypeScript
- Vite
- Axios
- React Router

## First launch on a new environment

Run from `apps/frontend`:

```bash
npm run setup
npm run dev
```

Then open the URL shown by Vite (`http://localhost:3000` by default).

## Scripts

- `npm run setup` - install dependencies with `npm ci` (strictly from `package-lock.json`).
- `npm run setup:dev` - install dependencies with `npm install` (update lock file).
- `npm run dev` - start local dev server.
- `npm run build` - type check and build production bundle.
- `npm run lint` - run ESLint.
- `npm run preview` - preview production build locally.
