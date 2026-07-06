# Backend Thiskord

La documentation complete du backend est disponible ici:

- `docs/backend.md`

## Demarrage rapide

### Avec Docker Compose (recommande en dev)

```powershell
docker compose up
```

Backend expose sur `http://localhost:8080`.

### Arreter

```powershell
docker compose down
```

### Reinitialiser aussi la base (suppression volumes)

```powershell
docker compose down -v
```

## Build image backend (mode production)

```powershell
docker build -t backend .
docker run -p 8080:8080 backend
```

## Notes

- Le script SQL d'initialisation est `Thiskord_db.sql`.
- Le hub SignalR est mappe sur `/chatHub`.
- Les logs applicatifs fichiers sont ecrits dans `logs.txt`.

## Également un grand merci à tous les contributeurs pour ce projet de licence :

<a href="https://github.com/thiskordv2/Thiskord-Back/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=thiskordv2/Thiskord-Back" />
</a>

Made with [contrib.rocks](https://contrib.rocks).
