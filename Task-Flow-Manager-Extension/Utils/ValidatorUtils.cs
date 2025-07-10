using System.Text.RegularExpressions;
using Task_Flow_Manager_Extension.Exceptions;

namespace Task_Flow_Manager_Extension.Utils;

public static class ValidatorUtils
{
    public static void NotEmpty(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException(fieldName, $"{fieldName} cannot be null or empty.");
    }

    public static void MaxLength(string value, int maxLength, string fieldName)
    {
        if (value.Length > maxLength)
            throw new ValidationException(fieldName, $"{fieldName} cannot exceed {maxLength} characters.");
    }
    
    public static void MinLength(string value, int minLength, string fieldName)
    {
        if (value.Length < minLength)
            throw new ValidationException(fieldName, $"{fieldName} must be at least {minLength} characters long.");
    }
    
    public static void MustBeEmail(string value, string fieldName)
    {
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(value, pattern))
            throw new ValidationException(fieldName, $"{fieldName} format is invalid.");
    }
    
    public static void MustBePositive(long number, string fieldName)
    {
        if (number <= 0)
            throw new ValidationException(fieldName, $"{fieldName} must be greater than zero.");
    }

    public static void MustBeWithinRange(int number, int min, int max, string fieldName)
    {
        if (number < min || number > max)
            throw new ValidationException(fieldName, $"{fieldName} must be between {min} and {max}.");
    }
    
    public static void NotNullDate(DateOnly? value, string fieldName)
    {
        if (!value.HasValue)
            throw new ValidationException(fieldName, $"{fieldName} must be provided.");
    }

    public static void NotInPast(DateOnly value, string fieldName)
    {
        if (value < DateOnly.FromDateTime(DateTime.Today))
            throw new ValidationException(fieldName, $"{fieldName} cannot be in the past.");
    }

    public static void NotInFuture(DateOnly value, string fieldName)
    {
        if (value > DateOnly.FromDateTime(DateTime.Today))
            throw new ValidationException(fieldName, $"{fieldName} cannot be in the future.");
    }

    public static void EndDateNotBeforeStartDate(DateOnly? start, DateOnly? end, string startFieldName, string endFieldName)
    {
        if (start.HasValue && end.HasValue && end < start)
            throw new ValidationException(endFieldName, $"{endFieldName} cannot be earlier than {startFieldName}.");
    }
    
    public static void MinValue(decimal value, decimal min, string fieldName)
    {
        if (value < min)
            throw new ValidationException(fieldName, $"{fieldName} must be at least {min}.");
    }

}