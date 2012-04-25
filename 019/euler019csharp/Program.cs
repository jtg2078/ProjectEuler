using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace euler019csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();

            Console.WriteLine("The number of Sundays fell on the first of the month during the twentieth century (1 Jan 1901 to 31 Dec 2000)");
            sw.Start();
            var sum1 = test();
            sw.Stop();
            Console.WriteLine(string.Format("List<DateUnit> and LINQ filter way(tick:{0}): {1}", sw.ElapsedTicks, sum1));

            sw.Reset();

            sw.Start();
            var sum2 = test2();
            sw.Stop();
            Console.WriteLine(string.Format("plain iterate through way(tick:{0}): {1}", sw.ElapsedTicks, sum2));

            sw.Reset();

            sw.Start();
            var sum3 = test3();
            sw.Stop();
            Console.WriteLine(string.Format("LINQ + BCL DateTime Object(tick:{0}): {1}", sw.ElapsedTicks, sum3));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        [DebuggerDisplay("{Month}/{Day}/{Year}/{DayOfWeek}")]
        public class DateUnit
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }
            public int DayOfWeek { get; set; }
            public DateUnit(int year, int month, int date, int day)
            {
                Year = year;
                Month = month;
                Day = date;
                DayOfWeek = day;
            }
        }

        static int test()
        {
            var cal = new List<DateUnit>();
            // initial setting
            var year = 1900;
            var month = 1;
            var day = 1;
            var day_of_week = 0; // monday = 0, tuesday=1,..... sunday =6
            var month_counter = 0;

            while (true)
            {
                if (month == 4 || month == 6 || month == 9 || month == 11)
                    month_counter = 30;
                else if (month == 2)
                    month_counter = year % 4 == 0 && year % 100 != 0 || year % 400 == 0 ? 29 : 28;
                else
                    month_counter = 31;

                for (day = 1; day <= month_counter; day++)
                {
                    cal.Add(new DateUnit(year, month, day, day_of_week % 7));
                    day_of_week++;
                }

                year = month == 12 ? year + 1 : year;
                month = month == 12 ? 1 : month + 1;

                if (year == 2001)
                    break;
            }
            return cal.Where(l => l.Year >= 1901 && l.Day == 1 && l.DayOfWeek == 6).Count();
        }

        static int test2()
        {
            // initial setting
            var year = 1900;
            var month = 1;
            var day = 1;
            var day_of_week = 0; // monday = 0, tuesday=1,..... sunday =6
            var count = 0;
            var month_counter = 0;

            while (true)
            {
                month_counter = month == 4 || month == 6 || month == 9 || month == 11 ?
                    30 : month == 2 ? year % 4 == 0 && year % 100 != 0 || year % 400 == 0 ? 29 : 28 : 31;

                for (day = 1; day <= month_counter; day++)
                {
                    if (year >= 1901 && day == 1 && day_of_week % 7 == 6)
                        count++;

                    day_of_week++;
                }

                if (month == 12)
                {
                    month = 1;
                    year++;

                    if (year == 2001)
                        break;
                }
                else
                    month++;
            }

            return count;
        }

        static int test3()
        {
            var start = new DateTime(1901, 1, 1);
            var end = new DateTime(2000, 12, 31);
            return Enumerable.Range(0, end.Subtract(start).Days)
                .Select(l => start.AddDays(l))
                .Where(l => l.Day == 1 && l.DayOfWeek == DayOfWeek.Sunday)
                .Count();
        }

        static int test4()
        {
            var day_of_week = 0; // monday = 0, tuesday=1,..... sunday =6
            var count = 0;
            var days_in_month = new int[] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            for (int year = 1900; year <= 2000; year++)
            {
                days_in_month[2] = year % 4 == 0 && year % 100 != 0 || year % 400 == 0 ? 29 : 28;
                for (int month = 1; month <= 12; month++)
                {
                    for (int day = 1; day <= days_in_month[month]; day++)
                    {
                        if (year >= 1901 && day == 1 && day_of_week % 7 == 6)
                            count++;

                        day_of_week++;
                    }
                }
            }

            return count;
        }
       
    }
}
