using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Validation;

public abstract class Validator<T, TC> : AbstractValidator<T> where TC : DbContext
{
    public static string Default = "Invalid request.";

    protected Validator(TC dbContext = null)
    {
        DbContext = dbContext;
    }

    protected TC DbContext { get; set; }

    public ValidationResult StdValidate(ValidationContext<T> context)
    {
        return Validate(context);
    }

    public Task<ValidationResult> StdValidateAsync(ValidationContext<T> context,
        CancellationToken cancellation = default)
    {
        return ValidateAsync(context, cancellation)
            .ContinueWith(
                x => x.Result, cancellation
            );
    }


    public ValidationResult StdValidate(T instance)
    {
        return ValidationsTools.FromFluentValidationResult(Validate(instance));
    }

    public Task<ValidationResult> StdValidateAsync(T instance, CancellationToken cancellation = default)
    {
        return ValidateAsync(instance, cancellation)
            .ContinueWith(x =>
                    ValidationsTools.FromFluentValidationResult(x.Result), cancellation
            );
    }
}