using EmployeeManagement.Application.Contracts;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Infrastructure.Repositories;

public class InMemoryEmployeeRepository : IEmployeeRepository
{
    private readonly List<Employee> _employees = new()
    {
        CreateEmployee(1, "Aisha Patel", new DateOnly(1990, 4, 18), "+1 (415) 555-0123", DateTime.UtcNow.AddDays(-12)),
        CreateEmployee(2, "Daniel Kim", new DateOnly(1987, 11, 9), "+1 (212) 555-0142", DateTime.UtcNow.AddDays(-7))
    };

    private static Employee CreateEmployee(int id, string name, DateOnly dob, string phone, DateTime createdAt)
    {
        var e = new Employee
        {
            Name = name,
            DateOfBirth = dob,
            PhoneNumber = phone,
            CreatedAtUtc = createdAt
        };
        e.AssignId(id);
        return e;
    }

    public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Employee>>(_employees.OrderByDescending(x => x.CreatedAtUtc).ToList());
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_employees.FirstOrDefault(x => x.Id == id));
    }

    public Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (employee.Id == 0)
        {
            employee.AssignId(_employees.Count + 1);
        }
        employee.CreatedAtUtc = employee.CreatedAtUtc == default ? DateTime.UtcNow : employee.CreatedAtUtc;

        _employees.Add(employee);
        return Task.FromResult(employee);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var removed = _employees.RemoveAll(x => x.Id == id);
        return Task.FromResult(removed > 0);
    }
}
