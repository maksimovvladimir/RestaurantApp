# Ресторан — АИС (учебная практика ПМ.02)

Заказчик: УКИТ
Разработчик: Максимов Владимир Алексеевич

## Структура
- `RestaurantApp.Data` — модели и репозитории (Npgsql), вся бизнес-логика по факту в БД (хранимые процедуры/триггеры)
- `RestaurantApp.UI` — WinForms-клиент (.NET 8, Windows только)

## Запуск
1. Поднять БД: `psql -f restaurant_db.sql`, затем при необходимости `restaurant_seed.sql`.
2. В `RestaurantApp.Data/DbConfig.cs` указать строку подключения (или переменную окружения `RESTAURANT_DB_CONNECTION`).
3. Открыть `RestaurantApp.sln` в Visual Studio (Windows), `dotnet build`, запустить `RestaurantApp.UI`.

## Тестовый пользователь
Создать вручную через `UsersRepository.CreateUser(...)` либо SQL-вставкой (пароль хешируется SHA-256,
см. `UsersRepository.HashPassword`).

## Статус модулей
- Авторизация, Заказы — полная логика
- Брони, Меню/склад, Статистика — рабочий минимум, см. TODO в коде каждой формы
- Кухня (отдельная форма под роль kitchen) — не реализована, следующая итерация

## Ветки (история разработки)
`feature/data-layer` → `feature/auth` → `feature/orders` → `feature/reservations` →
`feature/menu-stock` → `feature/statistics`, каждая слита в `main` отдельным merge-коммитом
(`git log --oneline --graph` показывает топологию).
