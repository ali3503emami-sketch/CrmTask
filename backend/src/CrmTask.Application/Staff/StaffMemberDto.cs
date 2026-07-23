namespace CrmTask.Application.Staff;

public record StaffMemberDto(Guid Id, string FullName, string PhoneNumber, string? Position, bool IsActive);
