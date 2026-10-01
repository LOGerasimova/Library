# Library

## Запуск через Docker

Для запуска:

```powershell
docker compose down -v
docker compose up --build
```

После запуска:
Приложение — по адресу <http://localhost:8080/books>, 
Swagger UI — по адресу <http://localhost:8080/swagger>, 
PostgreSQL — на `localhost:5432`.
