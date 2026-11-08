# Модуль Counterparties

Модуль реализует создание, список, просмотр, полное обновление и архивирование контрагентов организации. Отдельная строковая бизнес-классификация `Category` не связана с модулем финансовых категорий.

## API

Все маршруты требуют Bearer-аутентификации. Организация определяется существующим контекстом активного членства, а не данными HTTP-запроса.

| Метод | Маршрут | Разрешение |
|---|---|---|
| POST | `/api/counterparties/create` | `Counterparties.Create` |
| GET | `/api/counterparties` | `Counterparties.View` |
| GET | `/api/counterparties/{id}` | `Counterparties.View` |
| PUT | `/api/counterparties/{id}` | `Counterparties.Update` |
| PATCH | `/api/counterparties/{id}/archive` | `Counterparties.Archive` |

Тело POST и PUT:

```json
{
  "name": "ООО Поставщик",
  "type": "legalEntity",
  "category": "Поставщик",
  "phone": "+7 900 123-45-67",
  "email": "contact@example.com"
}
```

`name` обязателен. Остальные поля допускают `null`; PUT очищает такие поля, включая `type`. Тип сериализуется по общим правилам проекта: `individual`, `legalEntity` или `null`. Длины ограничены соответственно 200, 100, 50 и 200 символами для имени, классификации, телефона и email. Неизвестный тип и некорректный email возвращают ошибку валидации.

Список поддерживает `search`, `type` и `includeArchived=false`. Поиск выполняется в PostgreSQL через `ILIKE` с экранированием `\`, `%` и `_`. Сортировка — по имени, затем ID. Архивная запись доступна по ID, но её изменение возвращает `Counterparty.Archived` (HTTP 409). Повторное архивирование успешно и сохраняет аудит без изменений. Физического удаления нет.

Ответ содержит `id`, `organizationId`, `name`, `type`, `category`, `phone`, `email`, `isArchived`. Успех и ошибки преобразуются существующим `ToActionResult()`; чужой и отсутствующий ID одинаково возвращают `Counterparty.NotFound` (HTTP 404).

## Архитектура

Модуль повторяет структуру Categories: Domain, Application с отдельными use-case handlers, API и регистрация зависимостей. Используются существующие `ApplicationDbContext`, `ICurrentOrganization`, `ICurrentUser`, `IPermissionChecker`, `Result` и FluentValidation. Новых библиотек и инфраструктурных абстракций нет.

Каждый handler сначала загружает текущую организацию и проверяет разрешение. Существующий `CurrentOrganization` допускает только активное членство. Все запросы контрагентов ограничены `OrganizationId` на стороне БД; чтение использует `AsNoTracking()`. Доменная сущность наследует `AuditableEntity`, реализует `IOrganizationOwnedEntity`, не позволяет менять организацию и блокирует редактирование архива. Обработчики вызывают `MarkCreated` и `MarkUpdated` с текущим пользователем.

## Миграция и права

Сгенерирована EF Core 9 migration `20261108101223_CreateCounterparties` и обновлён snapshot. Предыдущие миграции сохранены без изменений.

Таблица `counterparties` содержит обязательный FK организации с `ON DELETE RESTRICT`, nullable тип в строковом `varchar(50)`, ограничения длины, `is_archived boolean NOT NULL DEFAULT FALSE` и аудит по общей конфигурации. Есть индекс организации и индексы пользователей аудита; уникального индекса имени нет.

В этой же миграции через `ON CONFLICT DO NOTHING` добавляются четыре разрешения и назначения существующим ролям:

| Код роли (`RoleCodes`) | Права Counterparties |
|---|---|
| `owner` | View, Create, Update, Archive |
| `admin` | View, Create, Update, Archive |
| `accountant` | View, Create, Update, Archive |
| `viewer` | View |

Числовые ID ролей не используются. Другие права и назначения не изменяются. В `Down()` удаляется только таблица контрагентов; записи прав сохраняются, поскольку могли существовать до миграции.

Миграция **не применялась** к пользовательской БД. Для самостоятельного применения из корня репозитория:

```sh
dotnet ef database update CreateCounterparties --project src/FinaccApi
```

В исходном проекте нет автоматического создания системных ролей. На пустой БД миграция зарегистрирует permissions, но выдавать их будет некому. Если роли создаются после миграции, повторно выполните [скрипт регистрации](sql/register-counterparty-permissions.sql) в выбранной БД:

```sh
psql "$FINACC_DATABASE_URL" -v ON_ERROR_STOP=1 -f docs/sql/register-counterparty-permissions.sql
```

Скрипт повторно применим и не создаёт/не меняет роли.

## Проверка

Проверено 8 ноября 2026 года:

- Сборка приложения и тестового проекта успешна, предупреждений и ошибок компиляции нет.
- Полный `dotnet test`: **98 тестов, 97 успешных, 1 неуспешный, 0 пропущенных**.
- Все **67 новых тестов Counterparties успешны**: сценарии handlers, tenant isolation, доступ, валидация, аудит, архив, очистка полей, DI, доменные инварианты и метаданные модели.
- Отдельный запуск чистого исходного `HEAD 8297892` через `git archive`: **31 тест, 30 успешных, 1 неуспешный**. Тот же существующий тест `RegisterHandlerTests.Handle_WhenEmailAlreadyExists_ReturnsFailure` падает на строке 61: `RegisterHandler` вычисляет `emailExists`, но не проверяет его. Посторонний код Access не исправлялся.
- SQL прямой и обратной миграции сгенерирован и проверен. `dotnet ef migrations has-pending-model-changes` подтвердил отсутствие расхождений модели и snapshot.
- `git diff --check` не обнаружил ошибок пробелов.

Команды проверки:

```sh
dotnet build FinaccApi.sln --no-restore -m:1 -nodeReuse:false -p:UseSharedCompilation=false
dotnet test FinaccApi.sln --no-restore -m:1 -nodeReuse:false -p:UseSharedCompilation=false
dotnet ef migrations has-pending-model-changes --project src/FinaccApi --no-build
dotnet ef migrations script AddUniqueActiveOrganizationMemberIndex CreateCounterparties --project src/FinaccApi --no-build
dotnet ef migrations script CreateCounterparties AddUniqueActiveOrganizationMemberIndex --project src/FinaccApi --no-build
```

Ограничение проверки: живая PostgreSQL не использовалась. InMemory-тесты не доказывают выполнение `ILIKE` и ограничений PostgreSQL; для модели отдельно проверены реляционные метаданные и преобразование nullable enum. Миграции к БД не применялись. Исходная ошибка регистрации мешает полному зелёному прогону существующих тестов.

## Файлы

Созданы:

```text
src/FinaccApi/Modules/Counterparties/
  Domain/Counterparty.cs
  Domain/CounterpartyType.cs
  Domain/CounterpartyConstraints.cs
  Application/CounterpartyDetailsResponse.cs
  Application/CounterpartyErrors.cs
  Application/CreateCounterparty/CreateCounterpartyCommand.cs
  Application/CreateCounterparty/CreateCounterpartyCommandValidator.cs
  Application/CreateCounterparty/CreateCounterpartyHandler.cs
  Application/GetCounterparties/GetCounterpartiesQuery.cs
  Application/GetCounterparties/GetCounterpartiesHandler.cs
  Application/GetCounterpartyById/GetCounterpartyByIdQuery.cs
  Application/GetCounterpartyById/GetCounterpartyByIdHandler.cs
  Application/UpdateCounterparty/UpdateCounterpartyCommand.cs
  Application/UpdateCounterparty/UpdateCounterpartyCommandValidator.cs
  Application/UpdateCounterparty/UpdateCounterpartyHandler.cs
  Application/ArchiveCounterparty/ArchiveCounterpartyCommand.cs
  Application/ArchiveCounterparty/ArchiveCounterpartyCommandValidator.cs
  Application/ArchiveCounterparty/ArchiveCounterpartyHandler.cs
  Api/CreateCounterpartyRequest.cs
  Api/UpdateCounterpartyRequest.cs
  Api/CounterpartiesController.cs
  CounterpartiesModule.cs
src/FinaccApi/DataAccess/Configurations/CounterpartyConfiguration.cs
src/FinaccApi/DataAccess/Migrations/20261108101223_CreateCounterparties.cs
src/FinaccApi/DataAccess/Migrations/20261108101223_CreateCounterparties.Designer.cs
tests/FinaccApi.Tests/Modules/Counterparties/Application/CounterpartyHandlerTests.cs
tests/FinaccApi.Tests/Modules/Counterparties/Domain/CounterpartyTests.cs
tests/FinaccApi.Tests/Modules/Counterparties/DataAccess/CounterpartyConfigurationTests.cs
docs/sql/register-counterparty-permissions.sql
docs/counterparties.md
```

Изменены:

```text
src/FinaccApi/DataAccess/ApplicationDbContext.cs
src/FinaccApi/DataAccess/Migrations/ApplicationDbContextModelSnapshot.cs
src/FinaccApi/Modules/Access/Domain/Permissions.cs
src/FinaccApi/Program.cs
```
