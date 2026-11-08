namespace FinaccApi.Modules.Counterparties.Application.GetCounterpartyById;

/// <summary>
/// Запрос контрагента по идентификатору.
/// </summary>
/// <param name="CounterpartyId">Идентификатор контрагента.</param>
public sealed record GetCounterpartyByIdQuery(int CounterpartyId);
