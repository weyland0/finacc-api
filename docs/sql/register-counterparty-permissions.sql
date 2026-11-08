-- Повторно применимая регистрация разрешений контрагентов.
-- Выполняется вручную после добавления дополнительных системных ролей.
-- Коды соответствуют Permissions.Counterparties и RoleCodes из модуля Access.
BEGIN;

-- Явные UUID обязательны: новые сущности получают ID в приложении, а не через DEFAULT БД.
INSERT INTO permissions (id, code, description)
VALUES
    ('019a6139-6000-7000-8000-000000000007'::uuid, 'Counterparties.View', 'Просмотр контрагентов'),
    ('019a6139-6000-7000-8000-000000000008'::uuid, 'Counterparties.Create', 'Создание контрагентов'),
    ('019a6139-6000-7000-8000-000000000009'::uuid, 'Counterparties.Update', 'Изменение контрагентов'),
    ('019a6139-6000-7000-8000-00000000000a'::uuid, 'Counterparties.Archive', 'Архивирование контрагентов')
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
