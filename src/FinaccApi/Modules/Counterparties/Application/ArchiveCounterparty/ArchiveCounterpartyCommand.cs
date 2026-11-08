namespace FinaccApi.Modules.Counterparties.Application.ArchiveCounterparty;

/// <summary>
/// Команда архивирования контрагента.
/// </summary>
/// <param name="CounterpartyId">Идентификатор контрагента.</param>
public sealed record ArchiveCounterpartyCommand(int CounterpartyId);
