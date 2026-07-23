using CrmTask.Application.ReferenceData;
using CrmTask.Domain.ReferenceData;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CrmTask.Api.Controllers;

[Route("api/activity-fields")]
public class ActivityFieldsController(IReferenceListService service, IValidator<CreateReferenceListItemRequest> validator)
    : ReferenceListControllerBase(service, validator, ReferenceListKind.ActivityField);
