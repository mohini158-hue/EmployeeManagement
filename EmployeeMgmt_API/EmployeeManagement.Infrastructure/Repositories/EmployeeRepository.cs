using EmployeeManagement.Application.Contracts;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeDbContext _context;

    public EmployeeRepository(EmployeeDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .OrderByDescending(e => e.CreatedAtUtc)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        // Ensure we don't send a client-assigned Id to the database for identity column
        employee.ClearId();
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (employee is null)
        {
            return false;
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
