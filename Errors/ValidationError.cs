using System;

namespace BranaOS.Opus.UseCases.DefaultPipelines.Errors;


public record ValidationError(string Message, string ValidationKey, string? PropertyName = null) : Core.Error
(
  Key: $"{ErrorKey}.{ValidationKey}{(PropertyName is not null ? $".{PropertyName}" : string.Empty)}",
  Message: Message
)
{
  public const string ErrorKey = "validation_error";
}