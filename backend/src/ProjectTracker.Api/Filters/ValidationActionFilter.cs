using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectTracker.Api.Common;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Api.Filters;

/// <summary>Runs FluentValidation against every action-method argument that has a registered
/// IValidator&lt;T&gt;, before the controller action (and therefore the Application handler)
/// executes. This is the single place object/shape validation happens — handlers never
/// re-validate shape, only business rules (see CLAUDE.md "Validation split"). Failing
/// validation short-circuits with 422 in the standard envelope.</summary>
public class ValidationActionFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationActionFilter(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new List<ValidationError>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (_serviceProvider.GetService(validatorType) is not IValidator validator) continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
            errors.AddRange(result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)));
        }

        if (errors.Count > 0)
        {
            context.Result = new Microsoft.AspNetCore.Mvc.UnprocessableEntityObjectResult(
                ApiResponse.FromResult(Result.FailValidation(errors)));
            return;
        }

        await next();
    }
}
