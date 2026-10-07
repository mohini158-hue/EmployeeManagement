namespace EmployeeManagement.Application.Models;

public record CreateEmployeeRequest(
    string Name,
    DateOnly DateOfBirth,
    string PhoneNumber);
