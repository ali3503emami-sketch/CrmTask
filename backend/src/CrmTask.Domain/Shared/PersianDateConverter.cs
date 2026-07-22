using System.Globalization;

namespace CrmTask.Domain.Shared;

/// <summary>
/// Converts Gregorian dates to a zero-padded Jalali (Shamsi) "yyyy/MM/dd" string,
/// via .NET's built-in <see cref="PersianCalendar"/> — no external dependency needed.
/// Every entity with a date field stores this alongside the canonical Gregorian
/// value (per product decision: Gregorian for correctness/sorting, Shamsi string
/// kept available too), rather than converting on every read.
/// </summary>
public static class PersianDateConverter
{
    private static readonly PersianCalendar Calendar = new();

    public static string ToShamsi(DateOnly date) => ToShamsi(date.ToDateTime(TimeOnly.MinValue));

    public static string ToShamsi(DateTimeOffset date) => ToShamsi(date.Date);

    private static string ToShamsi(DateTime date)
    {
        var year = Calendar.GetYear(date);
        var month = Calendar.GetMonth(date);
        var day = Calendar.GetDayOfMonth(date);
        return $"{year:0000}/{month:00}/{day:00}";
    }
}
