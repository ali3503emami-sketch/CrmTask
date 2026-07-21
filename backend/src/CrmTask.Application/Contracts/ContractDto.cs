using CrmTask.Domain.Contracts;

namespace CrmTask.Application.Contracts;

public record ContractDto(
    Guid Id,
    Guid CustomerId,
    string Title,
    decimal Amount,
    DateOnly StartDate,
    DateOnly EndDate,
    ContractStatus Status);
