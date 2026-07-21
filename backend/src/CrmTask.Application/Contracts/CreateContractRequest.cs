namespace CrmTask.Application.Contracts;

public record CreateContractRequest(string Title, decimal Amount, DateOnly StartDate, DateOnly EndDate);
