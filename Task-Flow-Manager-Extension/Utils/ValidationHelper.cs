using Task_Flow_Manager_Extension.Exceptions;
using Task_Flow_Manager_Extension.Infrastructure.Dto;

namespace Task_Flow_Manager_Extension.Utils;

public static class ValidationHelper
{
    /// <summary>
    /// Ensures the specified value is not null or, in the case of a string, not empty or whitespace.
    /// </summary>
    /// <typeparam name="T">The type of the value being validated.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <param name="customMessage">Optional custom error message to include in the validation exception.</param>
    /// <exception cref="ValidationException">Thrown when the value is null or invalid.</exception>
    public static void EnsureRequired<T>(T? value, string fieldName, string? customMessage = null)
    {
        if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
        {
            ThrowValidation(fieldName, customMessage ?? $"{fieldName} is required.");
        }
    }

    /// <summary>
    /// Validates that a user's access level is included in the allowed list.
    /// </summary>
    /// <param name="userAccessLevel">The access level of the current user.</param>
    /// <param name="allowedAccessLevels">A list of allowed access levels.</param>
    /// <param name="customMessage">Optional custom message for the validation error.</param>
    /// <exception cref="ValidationException">
    /// Thrown if the user's access level is not in the allowed list.
    /// </exception>
    public static void EnsureUserHasAccess(string userAccessLevel, IEnumerable<string> allowedAccessLevels,
        string? customMessage = null)
    {
        if (!allowedAccessLevels.Any(level =>
                string.Equals(level, userAccessLevel, StringComparison.OrdinalIgnoreCase)))
        {
            ThrowValidation("user_access_level",
                customMessage ??
                "The member lacks sufficient permissions to oversee the management of the supplier's firewall policy.");
        }
    }

    /// <summary>
    /// Validates that the specified string is not null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ValidationException">Thrown if the string is null or empty.</exception>
    public static void EnsureIsNotNull(string? value, string fieldName, string? customMessage = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ThrowValidation(fieldName, customMessage ?? $"{fieldName} is required.");
        }
    }
    

    /// <summary>
    /// Validates that all specified string fields are non-null and non-empty.
    /// </summary>
    /// <param name="message">The error message to use if any fields are missing.</param>
    /// <param name="fields">An array of tuples containing the value and field name for each field to validate.</param>
    /// <exception cref="ValidationException">Thrown if one or more fields are empty.</exception>
    public static void EnsureAllRequired(string message, params (string? Value, string FieldName)[] fields)
    {
        var emptyFields = fields
            .Where(f => string.IsNullOrWhiteSpace(f.Value))
            .Select(f => f.FieldName)
            .ToList();

        if (emptyFields.Any())
        {
            ThrowValidation("MultipleFields", message);
        }
    }

    /// <summary>
    /// Ensures that a string has at least the specified minimum length.
    /// </summary>
    /// <param name="value">The string value to validate.</param>
    /// <param name="minLength">The minimum number of characters required.</param>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <param name="customMessage">Optional custom message for the error.</param>
    /// <exception cref="ValidationException">Thrown if the string is too short or empty.</exception>
    public static void EnsureMinLength(string? value, int minLength, string fieldName, string? customMessage = null)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < minLength)
        {
            ThrowValidation(fieldName, customMessage ?? $"{fieldName} must be at least {minLength} characters long.");
        }
    }

    /// <summary>
    /// Validates that two strings match each other, ignoring case.
    /// </summary>
    /// <param name="value1">The first value to compare.</param>
    /// <param name="value2">The second value to compare.</param>
    /// <param name="fieldName">The field name to report in case of mismatch.</param>
    /// <param name="customMessage">Optional custom message.</param>
    /// <exception cref="ValidationException">Thrown if the values do not match.</exception>
    public static void EnsureValuesMatchIgnoreCase(string value1, string value2, string fieldName,
        string? customMessage = null)
    {
        if (!string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase))
        {
            ThrowValidation(fieldName, customMessage ?? $"{fieldName} values do not match.");
        }
    }

    /// <summary>
    /// Validates the structure of an email address (must contain "@" and ".").
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <param name="customMessage">Optional custom message to override the default error.</param>
    /// <exception cref="ValidationException">Thrown if the email format is invalid.</exception>
    public static void EnsureValidEmail(string? email, string fieldName, string? customMessage = null)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
        {
            ThrowValidation(fieldName, customMessage ?? "Invalid email format.");
        }
    }

    /// <summary>
    /// Ensures that a value is not duplicated in another existing record.
    /// </summary>
    /// <typeparam name="T">The type of the entity to check for duplicates.</typeparam>
    /// <param name="currentKey">The key of the current object (null indicates a new entry).</param>
    /// <param name="fetchFunc">A function that retrieves the existing conflicting entity, if any.</param>
    /// <param name="conflictField">The field that is conflicting.</param>
    /// <param name="conflictingNameSelector">Function to extract the display name of the conflicting entity.</param>
    /// <param name="contextDescription">A description of the context, e.g., "partner", "user".</param>
    /// <exception cref="ValidationException">Thrown if a duplicate value is found.</exception>
    /// <exception cref="NotFoundException">Thrown if the conflicting entity could not be found (unexpected).</exception>
    public static void EnsureNoDuplicateValueWithAnother<T>(
        string? currentKey,
        Func<T?> fetchFunc,
        string conflictField,
        Func<T, string> conflictingNameSelector,
        string contextDescription
    )
    {
        if (currentKey != null) return;
        var entity = fetchFunc() ?? throw new NotFoundException($"{contextDescription} not found");
        var name = conflictingNameSelector(entity);
        throw new ValidationException([
            new ErrorDetails(conflictField, $"Value is already registered with another {contextDescription}: {name}")
        ]);
    }

    /// <summary>
    /// Ensures that a string value matches an expected value, ignoring case.
    /// </summary>
    /// <param name="value">The actual value.</param>
    /// <param name="expectedValue">The expected value.</param>
    /// <param name="fieldName">The name of the field being validated.</param>
    /// <param name="customMessage">Optional custom message for mismatch.</param>
    /// <exception cref="ValidationException">Thrown if the value does not exactly match the expected value.</exception>
    public static void EnsureExactValue(string? value, string expectedValue, string fieldName,
        string? customMessage = null)
    {
        if (!string.Equals(value, expectedValue, StringComparison.OrdinalIgnoreCase))
        {
            ThrowValidation(fieldName, customMessage ?? $"{fieldName} must be '{expectedValue}'.");
        }
    }

    /// <summary>
    /// Throws a <see cref="ValidationException"/> with a specified field name and message.
    /// </summary>
    /// <param name="fieldName">The name of the field that caused the validation failure.</param>
    /// <param name="message">The message to include in the validation exception.</param>
    /// <exception cref="ValidationException">Always thrown.</exception>
    public static void ThrowValidation(string fieldName, string message)
    {
        throw new ValidationException([new ErrorDetails(fieldName, message)]);
    }

    public static uint ParseAndValidateUInt(string? value, string fieldName, string? customMessage = null)
    {
        if (!uint.TryParse(value, out var result))
        {
            ThrowValidation(fieldName, customMessage ?? $"{fieldName} must be a valid positive integer.");
        }
        return result;
    }
}