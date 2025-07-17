using System;
using BranaOS.Opus.Core;
using BranaOS.Opus.UseCases.Behaviors.Abstract;
using BranaOS.Opus.UseCases.DefaultPipelines.Errors;
using FluentValidation;

namespace BranaOS.Opus.UseCases.DefaultPipelines.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> _validators) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  public async Task<Result<TResponse>> Execute(TRequest request, Func<Task<Result<TResponse>>> next)
  {
    if (!_validators.Any())
    {
      return await next();
    }

    var context = new ValidationContext<TRequest>(request);

    var validationResults = await Task.WhenAll(
      _validators.Select(v => v.ValidateAsync(context))
    );

    var failures = validationResults
      .SelectMany(r => r.Errors)
      .Where(f => f is not null)
      .ToList();

    if (failures.Count == 0)
    {
      return await next();
    }

    var errors = failures
      .Select(f => new ValidationError(f.ErrorMessage, f.ErrorCode, f.PropertyName))
      .ToList();

    return Result.Fail<TResponse>(errors);
  }
}