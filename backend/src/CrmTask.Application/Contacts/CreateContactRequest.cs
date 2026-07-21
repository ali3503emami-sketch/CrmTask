using CrmTask.Domain.Contacts;

namespace CrmTask.Application.Contacts;

public record CreateContactRequest(
    ContactDirection Direction,
    string Summary,
    DateTimeOffset ContactedAt,
    DateTimeOffset? NextFollowUpAt);
