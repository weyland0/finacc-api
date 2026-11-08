-- Повторно применимая регистрация разрешений контрагентов.
-- Выполняется вручную после создания системных ролей, если их не было при применении миграции.
-- Коды соответствуют Permissions.Counterparties и RoleCodes из модуля Access.
BEGIN;

INSERT INTO permissions (code, description)
VALUES
    ('Counterparties.View', 'Просмотр контрагентов'),
    ('Counterparties.Create', 'Создание контрагентов'),
    ('Counterparties.Update', 'Изменение контрагентов'),
    ('Counterparties.Archive', 'Архивирование контрагентов')
ON CONFLICT (code) DO NOTHING;

INSERT INTO role_permissions (role_id, permission_id)
SELECT roles.id, permissions.id
FROM roles
CROSS JOIN permissions
WHERE permissions.code IN (
    'Counterparties.View',
    'Counterparties.Create',
    'Counterparties.Update',
    'Counterparties.Archive')
  AND (roles.code IN ('owner', 'admin', 'accountant')
       OR (roles.code = 'viewer' AND permissions.code = 'Counterparties.View'))
ON CONFLICT (role_id, permission_id) DO NOTHING;

COMMIT;
