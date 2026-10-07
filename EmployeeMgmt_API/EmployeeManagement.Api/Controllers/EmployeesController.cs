using EmployeeManagement.Application.Models;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly EmployeeService _employeeService;

    public EmployeesController(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _employeeService.GetAllAsync(cancellationToken);
        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var employee = (await _employeeService.GetAllAsync(cancellationToken))
            .FirstOrDefault(x => x.Id == id);

        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> Create([FromBody] CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _employeeService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }
        catch (ArgumentException ex)
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                ["errors"] = [ex.Message]
            }));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int  id, CancellationToken cancellationToken)
    {
        var deleted = await _employeeService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
