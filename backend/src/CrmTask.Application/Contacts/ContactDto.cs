using CrmTask.Domain.Contacts;

namespace CrmTask.Application.Contacts;

public record ContactDto(
    Guid Id,
    Guid CustomerId,
    ContactDirection Direction,
    string Summary,
    DateTimeOffset ContactedAt,
    DateTimeOffset? NextFollowUpAt);
