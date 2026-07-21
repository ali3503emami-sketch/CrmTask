namespace CrmTask.Domain.Contacts;

/// <summary>
/// Whether a logged contact was initiated by the customer (inbound) or by us (outbound).
/// </summary>
public enum ContactDirection
{
    Inbound,
    Outbound,
}
