namespace Namaa.Domain.Entities;

internal static class DomainGuard
{
    public static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", parameterName);

        return value;
    }

    public static long Positive(long value, string parameterName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value must be greater than zero.");

        return value;
    }

    public static byte Positive(byte value, string parameterName)
    {
        if (value == 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value must be greater than zero.");

        return value;
    }

    public static short Positive(short value, string parameterName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value must be greater than zero.");

        return value;
    }

    public static int NonNegative(int value, string parameterName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative.");

        return value;
    }

    public static long Positive(long? value, string parameterName)
    {
        if (!value.HasValue || value.Value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value must be greater than zero.");

        return value.Value;
    }

    public static decimal Positive(decimal value, string parameterName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value must be greater than zero.");

        return value;
    }

    public static decimal NonNegative(decimal value, string parameterName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative.");

        return value;
    }

    public static decimal Percentage(decimal value, string parameterName)
    {
        if (value is < 0 or > 100)
            throw new ArgumentOutOfRangeException(parameterName, "Value must be between zero and one hundred.");

        return value;
    }

    public static TEnum Defined<TEnum>(TEnum value, string parameterName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentOutOfRangeException(parameterName, "Value is not defined.");

        return value;
    }

    public static string OneOf(string value, string parameterName, params string[] allowedValues)
    {
        Required(value, parameterName);

        if (!allowedValues.Contains(value, StringComparer.Ordinal))
            throw new ArgumentException($"Value must be one of: {string.Join(", ", allowedValues)}.", parameterName);

        return value;
    }

    public static void EndNotBeforeStart(DateTime? start, DateTime? end, string parameterName)
    {
        if (start.HasValue && end.HasValue && end.Value < start.Value)
            throw new ArgumentException("End date cannot be before start date.", parameterName);
    }

    public static void OptionalPositive(long? value, string parameterName)
    {
        if (value.HasValue && value.Value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value must be greater than zero when provided.");
    }
}
