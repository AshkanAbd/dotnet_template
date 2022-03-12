using System.Collections;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Validation;

public static class ValidationsTools
{
    public static bool Failed(this ValidationResult validationResult)
    {
        return !validationResult.IsValid;
    }

    public static IEnumerable<ValidationFailure> Messages(this ValidationResult validationResult)
    {
        return validationResult.Errors.Select(x => new ValidationFailure {
            PropertyName = x.PropertyName,
            ErrorMessage = x.ErrorMessage,
        }).ToList();
    }

    public static ValidationResult FromFluentValidationResult(ValidationResult result)
    {
        return new(result.Errors);
    }

    public static IRuleBuilderOptions<T, TProperty> WhenNotNull<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule)
    {
        return rule.Configure(config =>
            config.ApplyCondition(x => config.GetPropertyValue(x.InstanceToValidate) != null)
        );
    }

    public static IRuleBuilderOptions<T, TProperty> WithDefaultMessage<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule
    )
    {
        return rule.WithMessage(Validator<T, DbContext>.Default);
    }

    public static IRuleBuilderOptions<T, TProperty> WithDefaultMessage<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, string errorMessage
    )
    {
        return rule.WithMessage(errorMessage.IsNullOrWhiteSpace() ? Validator<T, DbContext>.Default : errorMessage);
    }

    public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> rule)
    {
        return rule.Matches("^(09)[0-9]{9}$");
    }

    public static IRuleBuilderOptions<T, string> Color<T>(this IRuleBuilder<T, string> rule)
    {
        return rule.Matches("^#([a-f0-9]{3}){1,2}$");
    }

    public static IRuleBuilderOptions<T, string> Domain<T>(this IRuleBuilder<T, string> rule)
    {
        return rule.Matches("^((https?):\\/\\/)?(www\\.)?([a-z0-9](\\.?))+\\.[a-z]{2,}(\\/?)$");
    }

    public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> rule)
    {
        return rule.Matches("^((https?):\\/\\/)?(www\\.)?([a-z0-9](\\.?))+\\.[a-z]{2,}(\\/?)(.*)$");
    }

    public static IRuleBuilderOptions<T, string> Gender<T>(this IRuleBuilder<T, string> rule)
    {
        return rule.Must(x => x == "آقا" || x == "خانم");
    }

    public static IRuleBuilderOptions<T, string> NationalCode<T>(this IRuleBuilder<T, string> rule)
    {
        return rule.Matches("^[0-9]{10}$");
    }

    public static IRuleBuilderOptions<T, string> Number<T>(this IRuleBuilder<T, string> rule)
    {
        return rule.Matches("^[0-9]+$");
    }

    public static IRuleBuilderOptions<T, string> Number<T>(this IRuleBuilder<T, string> rule, int digitCount)
    {
        return rule.Matches($"^[0-9]{{{digitCount}}}$");
    }

    public static IRuleBuilderOptions<T, IFormFile> MaxSize<T>(this IRuleBuilder<T, IFormFile> rule, long size)
    {
        return rule.Must(x => x.Length <= size);
    }

    public static IRuleBuilderOptions<T, IFormFile> SupportedTypes<T>(this IRuleBuilder<T, IFormFile> rule,
        IEnumerable<string> types)
    {
        return rule.Must(x => types.Contains(x.ContentType));
    }

    public static IRuleBuilderOptions<T, IFormFile> Image<T>(this IRuleBuilder<T, IFormFile> rule)
    {
        return rule.Must(x => new List<string> {
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/webp"
            }.Contains(x.ContentType.ToLower())
        );
    }

    public static IRuleBuilderOptions<T, IFormFile> ImageOrAudio<T>(this IRuleBuilder<T, IFormFile> rule)
    {
        return rule.Must(x => new List<string> {
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/webp",
                "audio/mpeg",
                "audio/mp3",
                "audio/aac",
            }.Contains(x.ContentType.ToLower())
        );
    }

    public static IRuleBuilderOptions<T, IFormFile> Audio<T>(this IRuleBuilder<T, IFormFile> rule)
    {
        return rule.Must(x => new List<string> {
                "audio/mpeg",
                "audio/mp3",
                "audio/aac",
            }.Contains(x.ContentType.ToLower())
        );
    }

    public static IRuleBuilderOptions<T, IFormFile> Video<T>(this IRuleBuilder<T, IFormFile> rule)
    {
        return rule.Must(x => new List<string> {
                "video/3gpp",
                "video/mp4",
                "video/mpeg",
                "video/mpeg",
                "video/ogg",
                "video/webm",
                "video/x-ms-wmv",
                "video/ms-asf",
                "video/x-m4v",
                "video/x-msvideo",
            }.Contains(x.ContentType.ToLower())
        );
    }

    public static IRuleBuilderOptions<T, IFormFile> ImageOrVideo<T>(this IRuleBuilder<T, IFormFile> rule)
    {
        return rule.Must(x => new List<string> {
                "video/3gpp",
                "video/mp4",
                "video/mpeg",
                "video/mpeg",
                "video/ogg",
                "video/webm",
                "video/x-ms-wmv",
                "video/ms-asf",
                "video/x-m4v",
                "video/x-msvideo",
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/webp"
            }.Contains(x.ContentType.ToLower())
        );
    }

    public static IRuleBuilderOptions<T, IFormFile> Svg<T>(this IRuleBuilder<T, IFormFile> rule)
    {
        return rule.Must(x => new List<string> {
                "image/svg+xml",
            }.Contains(x.ContentType.ToLower())
        );
    }

    public static IRuleBuilderOptions<T, IFormFile> ImageOrSvg<T>(this IRuleBuilder<T, IFormFile> rule)
    {
        return rule.Must(x => new List<string> {
                "image/svg+xml",
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/webp"
            }.Contains(x.ContentType.ToLower())
        );
    }

    public static IRuleBuilderOptions<T, TProperty> Include<T, TProperty>(this IRuleBuilder<T, TProperty> rule,
        IEnumerable<TProperty> values)
    {
        return rule.Must(values.Contains);
    }

    public static IRuleBuilderOptions<T, TProperty> Include<T, TProperty>(this IRuleBuilder<T, TProperty> rule,
        params TProperty[] values)
    {
        return rule.Must(values.Contains);
    }

    public static IRuleBuilderOptions<T, TProperty> IncludeAsync<T, TProperty>(this IRuleBuilder<T, TProperty> rule,
        Task<IEnumerable<TProperty>> values)
    {
        return rule.MustAsync(async (x, _) => (await values).Contains(x));
    }

    public static IRuleBuilderOptions<T, TProperty> IncludeAsync<T, TProperty>(this IRuleBuilder<T, TProperty> rule,
        Task<TProperty[]> values)
    {
        return rule.MustAsync(async (x, _) => (await values).Contains(x));
    }

    public static IRuleBuilderOptions<T, TProperty> MinimumCount<T, TProperty>(this IRuleBuilder<T, TProperty> rule,
        int count) where TProperty : ICollection
    {
        // return rule.Must(x => x.DistinctDynamic().CountDynamic() >= count);
        throw new NotImplementedException();
    }

    public static IRuleBuilderOptions<T, TProperty> MinimumCountAsync<T, TProperty>(
        this IRuleBuilder<T, TProperty> rule, Task<int> predicate) where TProperty : ICollection
    {
        // return rule.MustAsync(async (x, _) => x.DistinctDynamic().CountDynamic() >= await predicate);
        throw new NotImplementedException();
    }

    public static IRuleBuilderOptions<T, TProperty> MaximumCount<T, TProperty>(this IRuleBuilder<T, TProperty> rule,
        int count) where TProperty : ICollection
    {
        // return rule.Must(x => x.DistinctDynamic().CountDynamic() <= count);
        throw new NotImplementedException();
    }

    public static IRuleBuilderOptions<T, TProperty> MaximumCountAsync<T, TProperty>(
        this IRuleBuilder<T, TProperty> rule, Task<int> predicate) where TProperty : ICollection
    {
        // return rule.MustAsync(async (x, _) => x.DistinctDynamic().CountDynamic() <= await predicate);
        throw new NotImplementedException();
    }

    public static IRuleBuilderOptions<T, TProperty> Count<T, TProperty>(this IRuleBuilder<T, TProperty> rule,
        int count)
        where TProperty : ICollection
    {
        return rule.Must(x => x.Count == count);
    }

    public static IRuleBuilderOptions<T, TProperty> CountAsync<T, TProperty>(
        this IRuleBuilder<T, TProperty> rule, Task<int> predicate) where TProperty : ICollection
    {
        return rule.MustAsync(async (x, _) => x.Count == await predicate);
    }

    public static IRuleBuilderOptions<T, TProperty> LessThanOrEqualToAsync<T, TProperty>(
        this IRuleBuilder<T, TProperty> rule, Task<TProperty> predicate)
        where TProperty : IComparable<TProperty>, IComparable
    {
        return rule.MustAsync(async (x, _) => x.CompareTo(await predicate) <= 0);
    }

    public static IRuleBuilderOptions<T, TProperty> LessThanAsync<T, TProperty>(
        this IRuleBuilder<T, TProperty> rule, Task<TProperty> predicate)
        where TProperty : IComparable<TProperty>, IComparable
    {
        return rule.MustAsync(async (x, _) => x.CompareTo(await predicate) < 0);
    }

    public static IRuleBuilderOptions<T, TProperty> GreaterThanOrEqualToAsync<T, TProperty>(
        this IRuleBuilder<T, TProperty> rule, Task<TProperty> predicate)
        where TProperty : IComparable<TProperty>, IComparable
    {
        return rule.MustAsync(async (x, _) => x.CompareTo(await predicate) >= 0);
    }

    public static IRuleBuilderOptions<T, TProperty> GreaterThanAsync<T, TProperty>(
        this IRuleBuilder<T, TProperty> rule, Task<TProperty> predicate)
        where TProperty : IComparable<TProperty>, IComparable
    {
        return rule.MustAsync(async (x, _) => x.CompareTo(await predicate) > 0);
    }
}