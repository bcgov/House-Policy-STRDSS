using System.Globalization;
using System.Text.RegularExpressions;

namespace StrDss.Common
{
    public static class DateUtils
    {
        public static (bool parsed, DateTime? parsedDate) ParseDate(object val)
        {
            if (val == null)
                return (true, null);

            if (val.GetType() == typeof(DateTime))
            {
                return (true, (DateTime)val);
            }

            var formats = new string[] { "yyyyMMdd", "yyyy-MM-dd", "yyyy/MM/dd", "yyyy.MM.dd", "yyyyMd", "yyyy-M-d", "yyyy/M/dd", "yyyy.M.d" };
            var dateStr = val.ToString();

            if (dateStr!.IsEmpty())
                return (true, null);

            return (DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate), parsedDate);
        }

        public static string CovertToString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Returns Pacific time if VancouverTimeZone or PacificTimeZone is defined in the system
        /// Otherwise return UTC time.
        /// </summary>
        /// <param name="utcDate"></param>
        /// <returns></returns>
        public static DateTime ConvertUtcToPacificTime(DateTime utcDate)
        {
            var date = ConvertTimeFromUtc(utcDate, Constants.VancouverTimeZone);

            if (date != null)
                return (DateTime)date;

            date = ConvertTimeFromUtc(utcDate, Constants.PacificTimeZone);

            if (date != null)
                return (DateTime)date;

            return utcDate;
        }

        private static DateTime? ConvertTimeFromUtc(DateTime date, string timeZoneId)
        {
            try
            {
                var timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(date, timezone);
            }
            catch (TimeZoneNotFoundException)
            {
                return null;
            }
        }

        public static DateTime ConvertPacificToUtcTime(DateTime pstDate)
        {
            var date = ConvertTimeToUtc(pstDate, Constants.VancouverTimeZone);

            if (date != null)
                return (DateTime)date;

            date = ConvertTimeToUtc(pstDate, Constants.PacificTimeZone);

            if (date != null)
                return (DateTime)date;

            return pstDate;
        }

        private static DateTime? ConvertTimeToUtc(DateTime date, string timeZoneId)
        {
            try
            {
                var timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeToUtc(date, timezone);
            }
            catch (TimeZoneNotFoundException)
            {
                return null;
            }
        }

        public static (DateTime utcDateFrom, DateTime utcDateTo) GetUtcDateRange(DateTime pstDateFrom, DateTime pstDateTo)
        {
            pstDateFrom = pstDateFrom.Date;
            pstDateTo = pstDateTo.Date.AddDays(1).AddSeconds(-1);

            var utcDateFrom = ConvertPacificToUtcTime(pstDateFrom);
            var utcDateTo = ConvertPacificToUtcTime(pstDateTo);

            return (utcDateFrom, utcDateTo);
        }

        public static string? NormalizeReportPeriod(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim();

            // Try YYYY-MM or YYYY-MM-DD
            if (DateTime.TryParseExact(input, new[] { "yyyy-MM", "yyyy-MM-dd", "yyyy-M", "yyyy-M-d" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt1))
                return $"{dt1.Year:D4}-{dt1.Month:D2}";

            // Try Month-YYYY or Month-YY
            var monthYearMatch = Regex.Match(input, @"^(?<month>[A-Za-z]+)[\s\-](?<year>\d{4}|\d{2})$");
            if (monthYearMatch.Success)
            {
                var monthName = monthYearMatch.Groups["month"].Value;
                var yearStr = monthYearMatch.Groups["year"].Value;
                if (DateTime.TryParseExact(monthName, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var monthDt) ||
                    DateTime.TryParseExact(monthName, "MMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out monthDt))
                {
                    int year = yearStr.Length == 2 ? 2000 + int.Parse(yearStr) : int.Parse(yearStr);
                    return $"{year:D4}-{monthDt.Month:D2}";
                }
            }

            // Try YYYY-Month
            var yearMonthMatch = Regex.Match(input, @"^(?<year>\d{4})[\s\-](?<month>[A-Za-z]+)$");
            if (yearMonthMatch.Success)
            {
                var year = int.Parse(yearMonthMatch.Groups["year"].Value);
                var monthName = yearMonthMatch.Groups["month"].Value;
                if (DateTime.TryParseExact(monthName, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var monthDt) ||
                    DateTime.TryParseExact(monthName, "MMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out monthDt))
                {
                    return $"{year:D4}-{monthDt.Month:D2}";
                }
            }

            // Try YYYY-DD (treat as YYYY-MM)
            var yearDashMonth = Regex.Match(input, @"^(?<year>\d{4})-(?<month>\d{2})$");
            if (yearDashMonth.Success)
            {
                return input;
            }

            return null;
        }
    }
}
