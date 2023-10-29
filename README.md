# Hall_Of_Fame

Тестовое задание для NST

Для разработчика NST: [Документация API](http://90.188.90.247/swagger/index.html)

## Инструкция по развёртыванию

### Требования

- [.NET Core SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

### Настройка базы данных

1. Создайте новую базу данных в PostgreSQL.

2. Внесите необходимые настройки подключения к базе данных в файл `appsettings.json`.

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Ваша_строка_подключения_здесь"
   }
