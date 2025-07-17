using System;
using BranaOS.Opus.Core;
using BranaOS.Opus.UseCases.Behaviors.Abstract;
using BranaOS.Opus.UseCases.DefaultPipelines.Abstract;

namespace BranaOS.Opus.UseCases.DefaultPipelines.Behaviors;

public class TransactionalBehavior<TRequest, TResponse>
(
  IUnitOfWork _unitOfWork
) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
{
  public async Task<Result<TResponse>> Execute(TRequest request, Func<Task<Result<TResponse>>> next)
  {
    var response = await next();

    if (request is ITransactionalUseCase)
    {
      await _unitOfWork.SaveChangesAsync();
    }

    return response;
  }
}
