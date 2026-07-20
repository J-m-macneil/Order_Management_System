using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Application.Common.Exceptions;

namespace Application.Common.Validation;

public static partial class CommandValidation
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    public static void RequiredText(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BadRequestException($"{fieldName} is required.");
        }

        MaximumLength(value, fieldName, maxLength);
    }

    public static void OptionalText(string? value, string fieldName, int maxLength)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            MaximumLength(value, fieldName, maxLength);
        }
    }

    public static void Required(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BadRequestException($"{fieldName} is required.");
        }
    }

    public static void Email(string? value, string fieldName = "Email")
    {
        RequiredText(value, fieldName, 255);

        if (!EmailValidator.IsValid(value))
        {
            throw new BadRequestException($"{fieldName} must be a valid email address.");
        }
    }

    public static void OptionalPhone(string? value, string fieldName = "Phone")
    {
        OptionalText(value, fieldName, 50);

        if (!string.IsNullOrWhiteSpace(value) && !PhoneNumberPattern().IsMatch(value))
        {
            throw new BadRequestException($"{fieldName} must be a valid phone number.");
        }
    }

    public static void PositiveId(int value, string fieldName)
    {
        if (value <= 0)
        {
            throw new BadRequestException($"{fieldName} is required.");
        }
    }

    public static void NonNegative(decimal value, string fieldName)
    {
        if (value < 0)
        {
            throw new BadRequestException($"{fieldName} cannot be negative.");
        }
    }

    public static void Positive(decimal value, string fieldName)
    {
        if (value <= 0)
        {
            throw new BadRequestException($"{fieldName} must be greater than zero.");
        }
    }

    public static void Percentage(decimal value, string fieldName)
    {
        if (value is < 0 or > 100)
        {
            throw new BadRequestException($"{fieldName} must be between 0 and 100.");
        }
    }

    public static void Currency(string? value)
    {
        RequiredText(value, "Currency", 3);

        if (!CurrencyPattern().IsMatch(value!))
        {
            throw new BadRequestException("Currency must be a three-letter ISO code.");
        }
    }

    public static void MinimumLength(string value, string fieldName, int minimumLength)
    {
        if (value.Length < minimumLength)
        {
            throw new BadRequestException($"{fieldName} must be at least {minimumLength} characters.");
        }
    }

    public static void Date(DateTime value, string fieldName)
    {
        if (value == default)
        {
            throw new BadRequestException($"{fieldName} is required.");
        }
    }

    private static void MaximumLength(string value, string fieldName, int maxLength)
    {
        if (value.Length > maxLength)
        {
            throw new BadRequestException($"{fieldName} must be {maxLength} characters or fewer.");
        }
    }

    [GeneratedRegex(@"^[0-9+()\s-]+$")]
    private static partial Regex PhoneNumberPattern();

    [GeneratedRegex(@"^[A-Z]{3}$")]
    private static partial Regex CurrencyPattern();
}
