namespace FinaccApi.Modules.Access.Domain;

public static class Permissions
{
    public static class Organization
    {
        public const string View = "Organization.View";
        public const string Update = "Organization.Update";
    }

    public static class Members
    {
        public const string View = "Members.View";
        public const string Invite = "Members.Invite";
        public const string ChangeRole = "Members.ChangeRole";
        public const string Remove = "Members.Remove";
    }

    public static class Categories
    {
        public const string View = "Categories.View";
        public const string Create = "Categories.Create";
        public const string Update = "Categories.Update";
        public const string Delete = "Categories.Delete";
    }

    /// <summary>
    /// Разрешения на выполнение операций контрагентами.
    /// </summary>
    public static class Counterparties
    {
        /// <summary>
        /// Просмотр контрагентов.
        /// </summary>
        public const string View = "Counterparties.View";

        /// <summary>
        /// Создание контрагентов.
        /// </summary>
        public const string Create = "Counterparties.Create";

        /// <summary>
        /// Изменение контрагентов.
        /// </summary>
        public const string Update = "Counterparties.Update";

        /// <summary>
        /// Архивирование контрагентов.
        /// </summary>
        public const string Archive = "Counterparties.Archive";
    }

    public static class Accounts
    {
        public const string View = "Accounts.View";
        public const string Create = "Accounts.Create";
        public const string Update = "Accounts.Update";
        public const string Delete = "Accounts.Delete";
    }

    public static class Transactions
    {
        public const string View = "Transactions.View";
        public const string Create = "Transactions.Create";
        public const string Update = "Transactions.Update";
        public const string Delete = "Transactions.Delete";
        public const string Correct = "Transactions.Correct";
    }

    public static class Invoices
    {
        public const string View = "Invoices.View";
        public const string Create = "Invoices.Create";
        public const string Update = "Invoices.Update";
        public const string Pay = "Invoices.Pay";
        public const string Cancel = "Invoices.Cancel";
    }

    public static class Reports
    {
        public const string PnlView = "Reports.Pnl.View";
        public const string CashflowView = "Reports.Cashflow.View";
    }
}
