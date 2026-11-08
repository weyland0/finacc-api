using FinaccApi.Modules.Counterparties.Domain;
using FinaccApi.Shared.Kernel;

namespace FinaccApi.Tests.Modules.Counterparties.Domain;

/// <summary>
/// Проверки доменных инвариантов контрагента без инфраструктурных зависимостей.
/// </summary>
[TestClass]
public sealed class CounterpartyTests
{
	/// <summary>
	/// Проверяет обязательный положительный идентификатор организации.
	/// </summary>
	[TestMethod]
	[DataRow(0)]
	[DataRow(-1)]
	public void Constructor_WhenOrganizationIdIsNotPositive_Throws(int organizationId)
	{
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			new Counterparty(organizationId, "Контрагент", null, null, null, null));
	}

	/// <summary>
	/// Проверяет защиту от null вместо имени непосредственно в доменной модели.
	/// </summary>
	[TestMethod]
	public void Constructor_WhenNameIsNull_Throws()
	{
		Assert.ThrowsException<ArgumentNullException>(() => new Counterparty(7, null!, null, null, null, null));
	}

	/// <summary>
	/// Проверяет обязательное непустое имя при создании и изменении.
	/// </summary>
	[TestMethod]
	[DataRow("")]
	[DataRow(" \t\r\n ")]
	public void ConstructorAndUpdate_WhenNameIsWhitespace_ThrowWithoutChangingDetails(string name)
	{
		Assert.ThrowsException<ArgumentException>(() => new Counterparty(7, name, null, null, null, null));
		var counterparty = new Counterparty(7, "Исходное имя", CounterpartyType.Individual, "Партнёр", "123", "old@example.com");

		Assert.ThrowsException<ArgumentException>(() => counterparty.UpdateDetails(name, null, null, null, null));

		Assert.AreEqual("Исходное имя", counterparty.Name);
		Assert.AreEqual(CounterpartyType.Individual, counterparty.Type);
		Assert.AreEqual("Партнёр", counterparty.Category);
		Assert.AreEqual("123", counterparty.Phone);
		Assert.AreEqual("old@example.com", counterparty.Email);
	}

	/// <summary>
	/// Проверяет неизменность владельца и очистку необязательных данных при доменном обновлении.
	/// </summary>
	[TestMethod]
	public void UpdateDetails_WhenOptionalFieldsAreNull_ClearsDetailsAndPreservesOrganization()
	{
		var counterparty = new Counterparty(7, "Исходное имя", CounterpartyType.Individual, "Партнёр", "123", "old@example.com");

		counterparty.UpdateDetails("Новое имя", null, null, null, null);

		Assert.IsInstanceOfType<IOrganizationOwnedEntity>(counterparty);
		Assert.AreEqual(7, counterparty.OrganizationId);
		Assert.AreEqual("Новое имя", counterparty.Name);
		Assert.IsNull(counterparty.Type);
		Assert.IsNull(counterparty.Category);
		Assert.IsNull(counterparty.Phone);
		Assert.IsNull(counterparty.Email);
	}

	/// <summary>
	/// Проверяет идемпотентность архива и защиту архивной записи от доменного обновления.
	/// </summary>
	[TestMethod]
	public void Archive_WhenCalledRepeatedly_PreservesDataAndPreventsUpdates()
	{
		var counterparty = new Counterparty(7, "Исходное имя", CounterpartyType.Individual, "Партнёр", "123", "old@example.com");
		counterparty.MarkCreated(11);
		var updatedAt = counterparty.UpdatedAt;

		counterparty.Archive();
		counterparty.Archive();

		Assert.ThrowsException<InvalidOperationException>(() => counterparty.UpdateDetails("Новое имя", null, null, null, null));
		Assert.IsTrue(counterparty.IsArchived);
		Assert.AreEqual(7, counterparty.OrganizationId);
		Assert.AreEqual("Исходное имя", counterparty.Name);
		Assert.AreEqual(CounterpartyType.Individual, counterparty.Type);
		Assert.AreEqual("Партнёр", counterparty.Category);
		Assert.AreEqual("123", counterparty.Phone);
		Assert.AreEqual("old@example.com", counterparty.Email);
		Assert.AreEqual(11, counterparty.UpdatedBy);
		Assert.AreEqual(updatedAt, counterparty.UpdatedAt);
	}
}
