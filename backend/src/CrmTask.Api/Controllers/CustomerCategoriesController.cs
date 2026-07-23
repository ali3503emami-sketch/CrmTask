using CrmTask.Application.ReferenceData;
using CrmTask.Domain.ReferenceData;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CrmTask.Api.Controllers;

[Route("api/customer-categories")]
public class CustomerCategoriesController(IReferenceListService service, IValidator<CreateReferenceListItemRequest> validator)
    : ReferenceListControllerBase(service, validator, ReferenceListKind.CustomerCategory);
