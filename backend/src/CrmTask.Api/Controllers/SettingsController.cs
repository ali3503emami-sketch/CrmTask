using CrmTask.Application.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CrmTask.Api.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController(ISettingsService settingsService, IValidator<UpdateAppSettingsRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AppSettingsDto>> Get(CancellationToken cancellationToken)
    {
        var settings = await settingsService.GetAsync(cancellationToken);
        return Ok(settings);
    }

    [HttpPut]
    public async Task<ActionResult<AppSettingsDto>> Update(UpdateAppSettingsRequest request, CancellationToken cancellationToken)
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

        var settings = await settingsService.UpdateAsync(request, cancellationToken);
        return Ok(settings);
    }
}
