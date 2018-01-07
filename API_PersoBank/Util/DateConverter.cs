using System;

namespace API_PersoBank.Util
{
    public static class DateConverter
    {
        public static DateTime DoubleToDateTime(double date)
        {
            DateTime date1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return date1970.AddMilliseconds(date);
        }

        public static long DateTimeToLong(DateTime date)
        {
            return (long)date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}