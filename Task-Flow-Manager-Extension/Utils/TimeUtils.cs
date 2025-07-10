namespace Task_Flow_Manager_Extension.Utils;

public static class TimeUtils
{
    private const string DateFormat = "dd MMMM, yyyy hh:mm:ss tt";

    public static DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;
    public static string GetFormattedLocalNow() => DateTime.Now.ToString(DateFormat);
    public static string FormatToLocalTimeString(DateTimeOffset time) => time.LocalDateTime.ToString(DateFormat);
    public static string LastDayOfYear => $"{DateTime.Now.Year}-12-31";
    public static string Timezone => TimeZoneInfo.Local.Id;
}