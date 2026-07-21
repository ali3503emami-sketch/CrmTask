using CrmTask.Application.Customers;
using CrmTask.Application.Staff;
using CrmTask.Application.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CrmTask.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(
    ITaskService taskService,
    ICustomerService customerService,
    IStaffService staffService,
    IValidator<CreateTaskRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskItemDto>>> GetAll([FromQuery] Guid? customerId, CancellationToken cancellationToken)
    {
        var tasks = await taskService.GetAllAsync(customerId, cancellationToken);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItemDto>> Create(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        if (await staffService.GetByIdAsync(request.AssignedToStaffId, cancellationToken) is null)
        {
            return NotFound($"Staff member {request.AssignedToStaffId} was not found.");
        }

        if (request.CustomerId.HasValue && await customerService.GetByIdAsync(request.CustomerId.Value, cancellationToken) is null)
        {
            return NotFound($"Customer {request.CustomerId} was not found.");
        }

        var task = await taskService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { customerId = task.CustomerId }, task);
    }

    [HttpPost("{id:guid}/mark-done")]
    public async Task<ActionResult<TaskItemDto>> MarkAsDone(Guid id, CancellationToken cancellationToken)
    {
        var task = await taskService.MarkAsDoneAsync(id, cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost("{id:guid}/reassign")]
    public async Task<ActionResult<TaskItemDto>> Reassign(Guid id, ReassignTaskRequest request, CancellationToken cancellationToken)
    {
        if (await staffService.GetByIdAsync(request.StaffId, cancellationToken) is null)
        {
            return NotFound($"Staff member {request.StaffId} was not found.");
        }

        var task = await taskService.ReassignAsync(id, request.StaffId, cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPut("{id:guid}/checklist-items/{checklistItemId:guid}")]
    public async Task<ActionResult<TaskItemDto>> SetChecklistItemValue(
        Guid id,
        Guid checklistItemId,
        SetChecklistItemValueRequest request,
        CancellationToken cancellationToken)
    {
        var task = await taskService.SetChecklistItemValueAsync(id, checklistItemId, request.Value, cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }
}
