namespace EmployeeManagement.Application.Models;

public record EmployeeResponse(
    int Id,
    string Name,
    DateOnly DateOfBirth,
    string PhoneNumber,
    DateTime CreatedAtUtc);
