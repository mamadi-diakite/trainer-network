namespace TrainerNetwork.Domain;

internal static partial class Guard
{
    public static string Required(string? value, string paramName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", paramName);
        }

        return MaxLength(value.Trim(), paramName, maxLength);
    }

    public static string? Optional(string? value, string paramName, int maxLength) =>
        string.IsNullOrWhiteSpace(value) ? null : MaxLength(value.Trim(), paramName, maxLength);

    public static Guid NotEmpty(Guid value, string paramName) =>
        value == Guid.Empty ? throw new ArgumentException("Id is required.", paramName) : value;

    public static int NotNegative(int value, string paramName) =>
        value < 0 ? throw new ArgumentOutOfRangeException(paramName, value, "Value cannot be negative.") : value;

    public static DateTime Utc(DateTime value, string paramName) =>
        value.Kind == DateTimeKind.Utc ? value : throw new ArgumentException("Date must be in UTC.", paramName);

    public static TEnum Defined<TEnum>(TEnum value, string paramName)
        where TEnum : struct, Enum =>
        Enum.IsDefined(value) ? value : throw new ArgumentOutOfRangeException(paramName, value, "Unknown value.");

    public static string Slug(string? value, string paramName, int maxLength)
    {
        var slug = Required(value, paramName, maxLength);
        return SlugPattern().IsMatch(slug)
            ? slug
            : throw new ArgumentException("Slug must use lowercase letters, digits and single hyphens.", paramName);
    }

    private static string MaxLength(string value, string paramName, int maxLength) =>
        value.Length <= maxLength
            ? value
            : throw new ArgumentException($"Value must be at most {maxLength} characters.", paramName);

    [GeneratedRegex("^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex SlugPattern();
}
