using FluentValidation;
using MediatR;

namespace InventoryApp.Identity.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures.Select(f => f.ErrorMessage).Distinct().ToArray();
            var responseType = typeof(TResponse);

            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Common.Models.Result<>))
            {
                var result = typeof(Common.Models.Result<>)
                    .MakeGenericType(responseType.GetGenericArguments()[0])
                    .GetMethod(nameof(Common.Models.Result.Failure), new[] { typeof(string[]), typeof(string) })!
                    .Invoke(null, new object[] { errors, null! })!;
                return (TResponse)result;
            }

            if (responseType == typeof(Common.Models.Result))
                return (TResponse)(object)Common.Models.Result.Failure(errors);
        }

        return await next();
    }
}
