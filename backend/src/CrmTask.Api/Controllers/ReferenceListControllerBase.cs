using CrmTask.Application.ReferenceData;
using CrmTask.Domain.ReferenceData;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CrmTask.Api.Controllers;

/// <summary>
/// Shared GET/POST behavior for the app's manageable reference lists (Positions,
/// Customer Categories, Activity Fields) — all three are 100% identical CRUD over
/// a "Kind + Title" shape, so the one place that would otherwise be copy-pasted
/// three times lives here; each subclass just supplies its own route and
/// <see cref="ReferenceListKind"/>.
/// </summary>
[ApiController]
public abstract class ReferenceListControllerBase(
    IReferenceListService service,
    IValidator<CreateReferenceListItemRequest> validator,
    ReferenceListKind kind) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReferenceListItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await service.GetByKindAsync(kind, cancellationToken);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ReferenceListItemDto>> Create(CreateReferenceListItemRequest request, CancellationToken cancellationToken)
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

        var item = await service.CreateAsync(kind, request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), item);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ReferenceListItemDto>> Update(Guid id, CreateReferenceListItemRequest request, CancellationToken cancellationToken)
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

        var item = await service.UpdateAsync(id, request, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }
}
