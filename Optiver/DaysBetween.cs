using System;

namespace Optiver
{
    internal class DaysBetween
    {
        private static readonly int[] _DaysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        /// <summary>
        /// Returns the total number of days between the given days.
        /// </summary>
        public static int Get(int year1, int month1, int day1, int year2, int month2, int day2)
        {
            if (!IsValidDate(year1, month1, day1) || !IsValidDate(year2, month2, day2))
                throw new ArgumentOutOfRangeException("Invalid date range.");

            var totalDaysInDate1 = GetDaysInDate(year1, month1, day1);
            var totalDaysInDate2 = GetDaysInDate(year2, month2, day2);
            return Math.Abs(totalDaysInDate2 - totalDaysInDate1);
        }

        /// <summary>
        /// Returns the total number of days for any given date.
        /// </summary>
        private static int GetDaysInDate(int year, int month, int days)
        {
            int totalDays = year * 365 + days;
            for (int i = 0; i < month - 1; i++)
            {
                totalDays += _DaysInMonth[i];
            }
            totalDays += GetLeapYears(year, month);
            return totalDays;
        }

        /// <summary>
        /// Returns the total number of leap years for given year and month.
        /// </summary>
        private static int GetLeapYears(int year, int month)
        {
            if (month <= 2)
                --year;
            return (year / 4) - (year / 100) + (year / 400);
        }

        /// <summary>
        /// Returns true if the date is valid.
        /// </summary>
        private static bool IsValidDate(int year, int month, int days)
        {
            return year >= 0 && month > 0 && month <= 12 && days > 0;
        }

        public static void Test(int runs = 10000)
        {
            Random r = new Random();
            for (int i = 0; i < runs; i++)
            {
                var start = DateTime.Now.AddDays(-r.Next(0, 99999));
                var end = DateTime.Now.AddDays(r.Next(0, 99999));
                var actual = (int)(end - start).TotalDays;
                var expected = Get(start.Year, start.Month, start.Day, end.Year, end.Month, end.Day);
                //Console.WriteLine($"{actual} \t {expected}");
                if (actual != expected)
                {
                    throw new Exception("Mismatch");
                }
            }
            Console.WriteLine("Success");
        }
    }
}