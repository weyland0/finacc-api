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

Текущая схема создаётся начальной UUID-миграцией `20261108114139_InitialCreate`. Все ID и ссылки хранятся как PostgreSQL `uuid`, новые идентификаторы UUID v7 назначаются в приложении до сохранения.

Таблица `counterparties` содержит обязательный FK организации с `ON DELETE RESTRICT`, nullable тип в строковом `varchar(50)`, ограничения длины, `is_archived boolean NOT NULL DEFAULT FALSE` и аудит по общей конфигурации. Есть индекс организации и индексы пользователей аудита; уникального индекса имени нет.

Начальная миграция создаёт роль `owner`, разрешения реализованных модулей и назначения владельцу. Инструкции по пересозданию тестовой базы и изменениям API находятся в [описании перехода на UUID](guid-migration.md).

После добавления дополнительных ролей можно выполнить [повторно применимый скрипт](sql/register-counterparty-permissions.sql). Он явно передаёт UUID разрешений, находит роли и разрешения по кодам и выдаёт права по следующей схеме:

| Код роли (`RoleCodes`) | Права Counterparties |
|---|---|
| `owner` | View, Create, Update, Archive |
| `admin` | View, Create, Update, Archive |
| `accountant` | View, Create, Update, Archive |
| `viewer` | View |

Скрипт не создаёт роли и не изменяет остальные назначения.

## Проверка

Тесты Counterparties покрывают handlers, tenant isolation, доступ, валидацию, аудит, архив, очистку полей, DI, доменные инварианты и метаданные модели. Для идентификаторов используются GUID, пустой ID проверяется через `Guid.Empty`. Общие тесты `UuidIdentityTests` проверяют UUID v7 до сохранения и UUID-типы первичных/внешних ключей.

InMemory-тесты не подтверждают выполнение `ILIKE` и ограничений PostgreSQL; для этого необходима проверка на настоящей БД. Отдельный известный дефект регистрации дублирующего email описан в [заметках о переходе](guid-migration.md).
