namespace CrmTask.Domain.Contracts;

/// <summary>
/// Derived, not stored — see <see cref="Contract.GetStatus"/>. Storing this would
/// go stale the moment a day passes without anyone touching the row.
/// </summary>
public enum ContractStatus
{
    Active,
    ExpiringSoon,
    Ended,
}
