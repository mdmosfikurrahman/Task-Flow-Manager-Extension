using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Task_Flow_Manager_Extension.Exceptions;
using Task_Flow_Manager_Extension.Infrastructure.Dto;

namespace Task_Flow_Manager_Extension.Utils;

public static class GeneralUtils
{
    /// <summary>
    /// Provides access to the current HTTP context, enabling retrieval of request-specific data such as
    /// headers, connection info, user claims, and IP address from within static utility methods.
    /// </summary>
    /// <remarks>
    /// This must be initialized (e.g., via dependency injection) before calling methods that rely on HTTP context.
    /// </remarks>
    /// <example>
    /// var ip = GeneralUtils.GetUserIp(); // Requires HttpContextAccessor to be set
    /// </example>
    public static IHttpContextAccessor? HttpContextAccessor { get; set; }

    private static string AesKey { get; set; } = string.Empty;

    public static void SetAesKey(string key)
    {
        AesKey = key ?? throw new ArgumentNullException(nameof(key), "AES encryption key cannot be null.");
    }

    private static string GetEncryptionKey()
    {
        if (string.IsNullOrWhiteSpace(AesKey))
            throw new InvalidOperationException("AES encryption key is not initialized.");
        return AesKey;
    }

    /// <summary>
    /// Returns the current time in UTC as a Unix timestamp (number of seconds since 1970-01-01T00:00:00Z).
    /// </summary>
    /// <returns>
    /// A string representing the Unix timestamp for the current UTC time.
    /// </returns>
    /// <example>
    /// var timestamp = GetUnixTimestamp(); // e.g., "1716307200"
    /// </example>
    public static string GetUnixTimestamp()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return timestamp.ToString();
    }

    /// <summary>
    /// Generates a random alphanumeric string of the specified length using uppercase letters, lowercase letters, and digits.
    /// </summary>
    /// <param name="length">The desired length of the generated string.</param>
    /// <returns>
    /// A randomly generated alphanumeric string of the specified length.
    /// </returns>
    /// <example>
    /// var token = GenerateRandomString(8); // e.g., "a9Xk2T4b"
    /// </example>
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Range(1, length)
            .Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }

    /// <summary>
    /// Generates a random numeric string of the specified length using digits 0–9.
    /// </summary>
    /// <param name="length">The number of digits in the generated string.</param>
    /// <returns>
    /// A string consisting of randomly selected digits.
    /// </returns>
    /// <example>
    /// var code = GenerateRandomNumberString(6); // e.g., "429831"
    /// </example>
    public static string GenerateRandomNumberString(int length)
    {
        const string digits = "0123456789";
        var random = new Random();
        return new string(Enumerable.Range(1, length)
            .Select(_ => digits[random.Next(digits.Length)]).ToArray());
    }

    /// <summary>
    /// Retrieves the client's IP address from the current HTTP request context.
    /// Falls back to checking the "X-Forwarded-For" header if necessary.
    /// </summary>
    /// <returns>
    /// A string containing the user's IP address, or "unknown" if the context is unavailable or the IP cannot be determined.
    /// </returns>
    /// <remarks>
    /// This method supports reverse proxy setups where the IP may be passed in the "X-Forwarded-For" header.
    /// </remarks>
    /// <example>
    /// var ip = GetUserIp(); // e.g., "203.0.113.42"
    /// </example>
    public static string GetUserIp()
    {
        var context = HttpContextAccessor?.HttpContext;
        if (context == null)
            return "unknown";

        var ip = context.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrWhiteSpace(ip) && context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        }

        return string.IsNullOrWhiteSpace(ip) ? "unknown" : ip;
    }

    /// <summary>
    /// Generates a unique short identifier string by combining the current Unix timestamp,
    /// a random 3-digit number, and a specified member ID.
    /// </summary>
    /// <param name="memberId">A string identifier (e.g., user or partner ID) to include in the generated UID.</param>
    /// <returns>
    /// A unique uppercase alphanumeric UID string.
    /// </returns>
    /// <exception cref="Exception">
    /// Rethrows any exception encountered during UID generation.
    /// </exception>
    /// <example>
    /// var uid = CreateUniqueShortUid("P123"); // e.g., "1716307200123P123"
    /// </example>
    public static string CreateUniqueShortUid(string memberId)
    {
        try
        {
            var timestamp = GetUnixTimestamp();
            var randomNumber = GenerateRandomNumberString(3);
            var systemUid = $"{timestamp}{randomNumber}{memberId}"
                .ToUpper()
                .Replace(" ", "")
                .Trim();

            return systemUid;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Computes an SHA-256 hash of the specified password and returns the result as a Base64-encoded string.
    /// </summary>
    /// <param name="password">The password string to hash.</param>
    /// <returns>
    /// A Base64-encoded string representing the SHA-256 hash of the input password.
    /// </returns>
    /// <remarks>
    /// This method is useful for storing passwords securely in hashed form. 
    /// Note that this does not include salting or iterative hashing and should be enhanced for production use.
    /// </remarks>
    /// <example>
    /// var hashed = HashPassword("MySecurePassword123!");
    /// </example>
    public static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Generates a future date by adding a specified number of years to the current UTC date,
    /// and returns it as a <see cref="DateOnly"/> value.
    /// </summary>
    /// <param name="yearsToAdd">
    /// The number of years to add to the current date. Default is 30.
    /// </param>
    /// <returns>
    /// A <see cref="DateOnly"/> representing the resulting future date.
    /// </returns>
    /// <example>
    /// var dob = GenerateFutureDob(); // 30 years from today
    /// var customDob = GenerateFutureDob(50); // 50 years from today
    /// </example>
    public static DateOnly GenerateFutureDob(int yearsToAdd = 30)
    {
        var futureDate = DateTime.UtcNow.AddYears(yearsToAdd);
        return DateOnly.FromDateTime(futureDate);
    }

    /// <summary>
    /// Gets the current UTC date as a <see cref="DateOnly"/> value, excluding the time component.
    /// </summary>
    /// <returns>
    /// A <see cref="DateOnly"/> representing today's date in UTC.
    /// </returns>
    /// <example>
    /// var today = GetTodayDate(); // e.g., 2025-05-21
    /// </example>
    public static DateOnly GetTodayDate()
    {
        return DateOnly.FromDateTime(DateTime.UtcNow);
    }

    /// <summary>
    /// Generates a unique alphanumeric API key based on a given identifier, prefixed with a random uppercase letter
    /// and suffixed with a random 15-digit number. Useful for API key generation scenarios.
    /// </summary>
    /// <param name="identifier">
    /// A string identifier (e.g., partner ID, user ID, or client code) to be embedded in the API key.
    /// </param>
    /// <returns>
    /// A string representing a unique, uppercase alphanumeric API key. Example format: "X12345ABC678901234".
    /// </returns>
    /// <example>
    /// var apiKey = GenerateApiKeyWithPrefix("partner123");
    /// </example>
    public static string GenerateApiKeyWithPrefix(string identifier)
    {
        var random = new Random();
        var randomLetter = (char)random.Next(65, 91); // A–Z
        var numberPart = GenerateRandomNumberString(15);
        var raw = $"{randomLetter}{identifier}{numberPart}";
        var apiKey = raw.Replace(" ", "").Trim().ToUpper();
        return apiKey;
    }

    /// <summary>
    /// Filters a collection to include only unique elements based on the value of a specified property.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The input collection of items to filter.</param>
    /// <param name="propertyName">
    /// The name of the property to use for determining uniqueness. The property must be a public instance property of type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="List{T}"/> containing only the first occurrence of each unique value based on the specified property.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the specified <paramref name="propertyName"/> does not exist on the type <typeparamref name="T"/>.
    /// </exception>
    /// <remarks>
    /// Comparison is done using the string representation of the property value and is case-sensitive.
    /// Null property values are treated as empty strings.
    /// </remarks>
    /// <example>
    /// var result = UniqueByKey(users, "Email");
    /// // Returns a list of users with unique email addresses.
    /// </example>
    public static List<T> UniqueByKey<T>(IEnumerable<T> collection, string propertyName)
    {
        var seenKeys = new HashSet<string>();

        var propInfo = typeof(T).GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propInfo == null)
            throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T).Name}'.");

        return (from item in collection
            let key = propInfo.GetValue(item)?.ToString() ?? string.Empty
            where seenKeys.Add(key)
            select item).ToList();
    }

    /// <summary>
    /// Converts the current UTC time into the local time of a specified time zone.
    /// </summary>
    /// <param name="timeZoneId">
    /// The time zone identifier, such as "Asia/Dhaka" or "Eastern Standard Time".
    /// This must be a valid time zone ID recognized by the system.
    /// </param>
    /// <returns>
    /// A <see cref="DateTime"/> value representing the current time in the specified time zone.
    /// If the provided time zone ID is invalid or not found, UTC time is returned as a fallback.
    /// </returns>
    /// <remarks>
    /// This method uses <see cref="TimeZoneInfo.FindSystemTimeZoneById"/> to resolve the time zone.
    /// If the time zone is invalid or unsupported on the current OS, it will log a message and fall back to UTC.
    /// </remarks>
    /// <example>
    /// var localTime = GetCurrentBranchTime("Asia/Dhaka");
    /// </example>
    /// <exception cref="TimeZoneNotFoundException">
    /// Handled internally. Indicates the specified time zone ID was not found.
    /// </exception>
    /// <exception cref="InvalidTimeZoneException">
    /// Handled internally. Indicates the time zone data is corrupt or invalid.
    /// </exception>
    public static DateTime GetCurrentBranchTime(string timeZoneId)
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            Console.WriteLine($"Timezone '{timeZoneId}' not found. Falling back to UTC.");
            return DateTime.UtcNow;
        }
        catch (InvalidTimeZoneException)
        {
            Console.WriteLine($"Invalid timezone '{timeZoneId}'. Falling back to UTC.");
            return DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Serializes the specified object into its JSON string representation using the default JSON serialization settings.
    /// </summary>
    /// <param name="data">
    /// The object to serialize. It can be any serializable type such as a class, struct, list, or dictionary.
    /// </param>
    /// <returns>
    /// A JSON-formatted string representing the serialized object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="data"/> is null.
    /// </exception>
    /// <example>
    /// var json = EncodeJson(new { Name = "Alice", Age = 30 });
    /// // Output: { "Name": "Alice", "Age": 30 }
    /// </example>
    private static string EncodeJson(object data)
    {
        return JsonSerializer.Serialize(data);
    }

    /// <summary>
    /// Encrypts a plain text JSON string using AES encryption and a provided key. 
    /// The generated IV (Initialization Vector) is prepended to the encrypted byte stream for use during decryption.
    /// </summary>
    /// <param name="plainText">
    /// The plain text JSON string to encrypt. This should be a valid JSON string representing any serializable object.
    /// </param>
    /// <returns>
    /// A Base64-encoded string containing the IV and encrypted JSON data.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided <paramref name="plainText"/> is null.
    /// </exception>
    private static string EncryptJson(string plainText)
    {
        var key = GetEncryptionKey();
        using var aes = Aes.Create();
        aes.Key = GetAesKeyFromString(key);
        aes.GenerateIV();
        var iv = aes.IV;

        using var ms = new MemoryStream();
        ms.Write(iv, 0, iv.Length);

        using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cs, Encoding.UTF8))
        {
            writer.Write(plainText);
            writer.Flush();
            cs.FlushFinalBlock();
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    /// Decrypts a previously AES-encrypted JSON string using the provided encryption key and returns the original plain text JSON.
    /// </summary>
    /// <param name="encryptedText">
    /// The Base64-encoded string that represents the AES-encrypted JSON data. The encrypted text must include the IV prefix.
    /// </param>
    /// <returns>
    /// The decrypted plain text JSON string.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="encryptedText"/> is null, empty, or consists only of whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the input is not valid Base64, or if decryption fails due to an incorrect key or corrupted cipher data.
    /// </exception>
    /// <example>
    /// var json = DecryptJson(encryptedText: "BASE64_STRING", key: "mySecretKey123");
    /// </example>
    public static string DecryptJson(string encryptedText)
    {
        var key = GetEncryptionKey();
        if (string.IsNullOrWhiteSpace(encryptedText))
            throw new ArgumentException("Encrypted text is null or empty.");

        if (!IsBase64String(encryptedText))
            throw new InvalidOperationException(
                "The file content is not a valid base64 string. Possibly not encrypted.");

        try
        {
            var fullCipher = Convert.FromBase64String(encryptedText);

            using var aes = Aes.Create();
            aes.Key = GetAesKeyFromString(key);

            var ivLength = aes.BlockSize / 8;
            var iv = new byte[ivLength];
            var cipher = new byte[fullCipher.Length - ivLength];

            Array.Copy(fullCipher, iv, ivLength);
            Array.Copy(fullCipher, ivLength, cipher, 0, cipher.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs, Encoding.UTF8);

            return reader.ReadToEnd();
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Failed to decode base64 string.", ex);
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("Decryption failed due to an invalid key or corrupt data.", ex);
        }
    }

    /// <summary>
    /// Checks if the provided string is a valid Base64-encoded string.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is valid Base64; otherwise, false.</returns>
    private static bool IsBase64String(string value)
    {
        Span<byte> buffer = new Span<byte>(new byte[value.Length]);
        return Convert.TryFromBase64String(value, buffer, out _);
    }

    /// <summary>
    /// Generates a 256-bit AES key from a plain text string by hashing it using SHA-256.
    /// </summary>
    /// <param name="key">The input string to hash into a key.</param>
    /// <returns>A 32-byte (256-bit) byte array representing the AES encryption key.</returns>
    private static byte[] GetAesKeyFromString(string key)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(key)).Take(32).ToArray();
    }

    /// <summary>
    /// Parses a date string into Unix timestamp format (seconds since epoch).
    /// </summary>
    public static string ParseToUnixTimestamp(string dateStr)
    {
        if (DateTimeOffset.TryParse(dateStr, out var parsed))
        {
            return parsed.ToUnixTimeSeconds().ToString();
        }

        throw new ValidationException([
            new ErrorDetails("credit_expire", $"Invalid date format: '{dateStr}'. Expected ISO 8601 format.")
        ]);
    }

    /// <summary>
    /// Attempts to convert an object to a specified type with optional error context.
    /// </summary>
    public static T TryConvert<T>(object? value, string? propertyName = null)
    {
        try
        {
            if (value is null or DBNull)
                return default!;

            var targetType = typeof(T);

            if (Nullable.GetUnderlyingType(targetType) is { } underlyingType)
                targetType = underlyingType;

            if (targetType.IsInstanceOfType(value))
                return (T)value;

            if (targetType == typeof(DateOnly) && DateOnly.TryParse(value.ToString(), out var dateOnlyValue))
                return (T)(object)dateOnlyValue;

            if (targetType == typeof(DateTime) && DateTime.TryParse(value.ToString(), out var dt))
                return (T)(object)dt;

            if (targetType.IsEnum)
                return (T)Enum.Parse(targetType, value.ToString()!, ignoreCase: true);

            return (T)Convert.ChangeType(value, targetType);
        }
        catch
        {
            var errorField = propertyName ?? "conversion";
            var message = $"Cannot convert value '{value}' to type '{typeof(T).Name}'.";

            throw new ValidationException([
                new ErrorDetails(errorField, message)
            ]);
        }
    }

    /// <summary>
    /// Returns the currency symbol for a given currency code.
    /// </summary>
    public static string GetCurrencySymbol(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            return "";

        currencyCode = currencyCode.Trim().ToLowerInvariant();

        var symbols = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "", currencyCode.ToUpper() },
            { "bdt", "৳" }, { "sgd", "S$" }, { "usd", "$" },
            { "inr", "₹" }, { "thb", "฿" }, { "eur", "€" },
            { "idr", "Rp" }, { "gbp", "£" }, { "jpy", "¥" },
            { "aed", "د.إ" }
        };

        return symbols.TryGetValue(currencyCode, out var symbol)
            ? symbol
            : currencyCode.ToUpper();
    }

    /// <summary>
    /// Converts a Unix timestamp (string) into a DateOnly structure.
    /// </summary>
    public static DateOnly ConvertUnixTimestampToDateOnly(string? creditExpireTimestamp)
    {
        if (string.IsNullOrWhiteSpace(creditExpireTimestamp))
            throw new ArgumentException("The creditExpireTimestamp cannot be null or empty.");

        if (!long.TryParse(creditExpireTimestamp, out var timestamp))
            throw new FormatException($"Invalid UNIX timestamp: '{creditExpireTimestamp}'.");

        var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        return DateOnly.FromDateTime(dateTime);
    }

    /// <summary>
    /// Generates a compact date string in "ddMMyy" format, useful for tagging or codes.
    /// </summary>
    public static string GetCompactDateCode()
    {
        return DateTime.UtcNow.ToString("ddMMyy");
    }

    /// <summary>
    /// Returns a default value if the provided value is either null or, in the case of a string, empty or whitespace.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The value to evaluate.</param>
    /// <param name="defaultValue">The fallback value to return if the input is null or empty (for strings).</param>
    /// <returns>
    /// The original value if it's not null or empty (for strings); otherwise, the provided default value.
    /// </returns>
    /// <example>
    /// DefaultIfEmptyOrNull("", "default") → "default"
    /// DefaultIfEmptyOrNull("Hello", "default") → "Hello"
    /// DefaultIfEmptyOrNull(null, 5) → 5
    /// </example>
    public static T DefaultIfEmptyOrNull<T>(T? value, T defaultValue)
    {
        if (value == null)
            return defaultValue;

        if (value is string str && string.IsNullOrWhiteSpace(str))
            return defaultValue;

        return value;
    }

    /// <summary>
    /// Returns a replacement value if the provided value is not equal to the expected value.
    /// </summary>
    /// <typeparam name="T">The type of the values being compared.</typeparam>
    /// <param name="value">The current value to check.</param>
    /// <param name="expectedValue">The value that is considered acceptable.</param>
    /// <param name="replacement">The value to return if the current value does not match the expected value.</param>
    /// <returns>
    /// The original value if it equals the expected value; otherwise, the replacement value.
    /// </returns>
    /// <example>
    /// ReplaceIfNotEqual("ADMIN", "ADMIN", "GUEST") → "ADMIN"
    /// ReplaceIfNotEqual("USER", "ADMIN", "GUEST") → "GUEST"
    /// </example>
    public static T ReplaceIfNotEqual<T>(T? value, T expectedValue, T replacement)
    {
        if (value == null) return replacement;

        return EqualityComparer<T>.Default.Equals(value, expectedValue)
            ? value
            : replacement;
    }

    /// <summary>
    /// Converts a human-readable date/time string (e.g., "2025-06-01 12:00:00")
    /// into a Unix timestamp string (seconds since 1970-01-01T00:00:00Z).
    /// </summary>
    /// <param name="dateTimeStr">
    /// The input date/time string in ISO 8601 or standard recognizable format (e.g., "yyyy-MM-dd HH:mm:ss").
    /// </param>
    /// <returns>
    /// A string representing the Unix timestamp. Example: "1759377600".
    /// </returns>
    /// <exception cref="ValidationException">
    /// Thrown if the input is null, empty, or not a valid date.
    /// </exception>
    /// <example>
    /// var ts = GeneralUtils.ConvertDateTimeStringToUnixTimestamp("2025-06-01 12:00:00");
    /// </example>
    public static string ConvertDateTimeStringToUnixTimestamp(string dateTimeStr)
    {
        if (string.IsNullOrWhiteSpace(dateTimeStr))
        {
            throw new ValidationException([
                new ErrorDetails("date_input", "Date/time input is required.")
            ]);
        }

        if (!DateTimeOffset.TryParse(dateTimeStr, out var parsed))
        {
            throw new ValidationException([
                new ErrorDetails("date_input",
                    $"Invalid date/time format: '{dateTimeStr}'. Expected ISO 8601 or standard datetime format.")
            ]);
        }

        return parsed.ToUnixTimeSeconds().ToString();
    }
}