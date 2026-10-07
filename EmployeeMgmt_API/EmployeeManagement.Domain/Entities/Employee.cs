namespace EmployeeManagement.Domain.Entities;

public class Employee
{
    public int Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Controlled setters to prevent callers from directly assigning Id.
    public void AssignId(int id) => Id = id;
    public void ClearId() => Id = 0;
}
