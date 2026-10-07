using EmployeeManagement.Application.Contracts;
using EmployeeManagement.Application.Models;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Services;

public class EmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IReadOnlyList<EmployeeResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);
        return employees.Select(MapToResponse).ToList();
    }

    public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        var phoneNumber = request.PhoneNumber.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Employee name is required.", nameof(request));
        }

        if (request.DateOfBirth == default)
        {
            throw new ArgumentException("A valid date of birth is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("Phone number is required.", nameof(request));
        }

        var employee = new Employee
        {
            Name = name,
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = phoneNumber
        };

        var createdEmployee = await _employeeRepository.AddAsync(employee, cancellationToken);
        return MapToResponse(createdEmployee);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _employeeRepository.DeleteAsync(id, cancellationToken);
    }

    private static EmployeeResponse MapToResponse(Employee employee)
    {
        return new EmployeeResponse(
            employee.Id,
            employee.Name,
            employee.DateOfBirth,
            employee.PhoneNumber,
            employee.CreatedAtUtc);
    }
}
