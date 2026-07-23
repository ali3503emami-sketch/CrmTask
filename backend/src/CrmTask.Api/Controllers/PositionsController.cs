using CrmTask.Application.ReferenceData;
using CrmTask.Domain.ReferenceData;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CrmTask.Api.Controllers;

[Route("api/positions")]
public class PositionsController(IReferenceListService service, IValidator<CreateReferenceListItemRequest> validator)
    : ReferenceListControllerBase(service, validator, ReferenceListKind.Position);
