using System;
using System.Collections.Generic;
using System.Text;

namespace Calendar.Models
{
    public class JulianDate
    {

        private Int32 mjd;


        private JulianDate(Int32 mjd)
        {
            this.mjd = mjd;
        }

        public static JulianDate Now()
        {
            DateTime dateTime = DateTime.Now;
            return new JulianDate(CalDateToMJD(dateTime.Year, dateTime.Month, dateTime.Day));
        }

        public static JulianDate FromCalendarDate(Int32 year, Int32 month, Int32 day)
        {
            return new JulianDate(CalDateToMJD(year, month, day));
        }

        public static JulianDate FromCalendarDate(CalendarVector calVector)
        {
            return new JulianDate(CalDateToMJD(calVector.Year, calVector.Month, calVector.Day));
        }

        public static JulianDate FromJulian(double julian)
        {
            return new JulianDate(JulianToMJD(julian));
        }

        public static JulianDate FromMJD(Int32 mjd)
        {
            return new JulianDate(mjd);
        }

        public static JulianDate FromDayOfYear(Int32 year, Int32 doy)
        {
            return new JulianDate(DayOfYearToMJD(year,doy));
        }

        public static JulianDate FromDecimalYear(Double decYear)
        {
            return new JulianDate(DecimalYearToMJD(decYear));
        } 

        
        public static JulianDate[,] CalendarMonth(Int32 year, Int32 month)
        {
            JulianDate[,] array = new JulianDate[6,7];

            Int32 row, col, pos, mjd;
            Int32 mjd1 = JulianDate.CalDateToMJD(year, month, 1);
            Int32 dow1 = JulianDate.MJDToDayOfWeek(mjd1);

            dow1 = (dow1 < 4) ? dow1 + 7 : dow1;

            for(pos = dow1 - 1, mjd = mjd1; pos > -1; pos--, mjd--)
            {
                col = pos % 7;
                row = (int)Math.Floor(pos / 7.0);
                array[row, col] = JulianDate.FromMJD(mjd);
            }

            for (pos = dow1, mjd = mjd1 + 1; pos < 42; pos++, mjd++)
            {
                col = pos % 7;
                row = (int) Math.Floor(pos / 7.0);
                array[row,col] = JulianDate.FromMJD(mjd);
            }

            return array;

        }

        public Int32 MJD { get => mjd; }

        public double Julian { get => MJDToJulian(mjd); }
        
        public Int32 Year { get => MJDToCalDate(mjd).Year; }

        public Int32 Month { get => MJDToCalDate(mjd).Month; }

        public Month MonthEnum { get => (Month)Month; }

        public Int32 DayOfWeek { get => MJDToDayOfWeek(mjd); }

        public Weekday DayOfWeekEnum { get => (Weekday)DayOfWeek; }

        public Int32 DayOfMonth { get => MJDToCalDate(mjd).Day; }

        public Int32 DayOfYear { get => MJDToDayOfYear(mjd); }

        public Int32 GpsWeek { get => MJDToGpsWeek(mjd); }

        public Double DecimalYear { get => MJDToDecimalYear(mjd); }

        public bool IsLeapYear { get => isLeapYear(Year); }

        /// <summary>
        /// Converts calendar date to Julian date using algorithm
        /// from "Practical Ephemeris Calculations" by Oliver Montenbruck
        /// (Springer-Verlag, 1989). Uses astronomical year for B.C.dates
        /// (2 BC = -1 yr).
        /// </summary>
        /// <param name="year">4 Digit Year</param>
        /// <param name="month">Calendar Month</param>
        /// <param name="day">Day Of Month</param>
        /// <returns>Julian Date</returns>
        public static Int32 CalDateToMJD(Int32 year, Int32 month, Int32 day)
        {

            Int32 y, m, b;
            double date1, date2, date;


            if (month > 12 || month < 1)
            {
                throw new System.ArgumentException("Invalid Month", "month");
            }

            if (!IsValidDayOfMonth(year, month, day))
            {
                throw new System.ArgumentException("Invalid Day of Month", "day");
            }

            if (month > 2) {
                y = year;
                m = month;
            }
            else
            {
                y = year - 1;
                m = month + 12;
            }

            date1 = 4.5 + 31 * (10 + 12 * 1582);   // Last day of Julian calendar(1582.10.04 Noon)
            date2 = 15.5 + 31 * (10 + 12 * 1582);  // First day of Gregorian calendar(1582.10.15 Noon)
            
            date = day + 31 * (month + 12 * year);

            if (date <= date1)
            {
                b = -2;
            }
            else if (date >= date2)
            {
                b = (Int32)( Math.Floor(y / 400.0) - Math.Floor(y / 100.0));
            }
            else {
                throw new System.ArgumentException("Dates between October 5 & 15, 1582 do not exist","jDay");
                
            }

            return (int) (Math.Floor(365.25 * y) + Math.Floor(30.6001 * (m + 1)) + b - 679004 + day);

        }

        /// <summary>
        /// Converts Modified Julian date to calendar date using algorithm
        /// from "Practical Ephemeris Calculations" by Oliver Montenbruck
        /// (Springer-Verlag, 1989). Must use astronomical year for B.C.
        /// dates(2 BC = -1 yr).
        /// </summary>
        /// <param name="jDay"></param>
        /// <returns>A CalendarVector containing Day of Month, Month and Year</returns>
        public static CalendarVector MJDToCalDate(Int32 jDay)
        {

            Int32 a, b, c, d, e, f, day, month, year;

            a = jDay + 2400001;

            if (a < 0)
            {
                throw new System.ArgumentException("Invalid Julian Date", "jDay");
            }

            if (a < 2299161)
            {
                c = a + 1524;
            }
            else
            {
                b = (int)(Math.Floor((a - 1867216.25) / (36524.25)));
                c = a + b - (int)(Math.Floor(b / 4.0)) + 1525;
            }

            d = (int) Math.Floor((c - 122.1)/(365.25));
            e = (int) Math.Floor(365.25 * d);
            f = (int) Math.Floor((c - e)/30.6001);

            day = c - e - (int)Math.Floor(30.6001 * f);
            month = f - 1 - (12 * (int)Math.Floor(f / 14.0));
            year = d - 4715 - (int)Math.Floor((7 + month) / 10.0);

            return new CalendarVector(year, month, day);
        }

        /// <summary>
        /// Converts a Modified Julian Date to a Julian Date
        /// </summary>
        /// <param name="mjd">Modified Julian Date</param>
        /// <returns>Julian Date</returns>
        public static double MJDToJulian(Int32 mjd)
        {
            return mjd + 2400000.5;
        }

        /// <summary>
        /// Converts a Julian Date to a Modified Julian Date
        /// </summary>
        /// <param name="julian">Julian Date</param>
        /// <returns>Modified Julian Date</returns>
        public static Int32 JulianToMJD(double julian)
        {
            return (int) Math.Round(julian - 2400000.5);
        }

        /// <summary>
        /// Converts a Modified Julian Date to a Decimal Year
        /// </summary>
        /// <param name="mjd">Modified Julian Date</param>
        /// <returns>Decimal Year</returns>
        public static double MJDToDecimalYear(Int32 mjd)
        {
            Int32 year = MJDToCalDate(mjd).Year;
            Int32 mjd0 = CalDateToMJD(year, 1, 1);
            Int32 mjd1 = CalDateToMJD(year + 1, 1, 1);
            return (year + ((mjd - mjd0) / ((double)(mjd1 - mjd0))));
        }

        /// <summary>
        /// Converts a Decimal Year into a Modified Julian Date
        /// </summary>
        /// <param name="decYear">Decimal Year</param>
        /// <returns>Modified Julian Date</returns>
        public static Int32 DecimalYearToMJD(double decYear)
        {
            Int32 year = (int)Math.Floor(decYear);
            Int32 nDays = CalDateToMJD(year + 1, 1, 1) - CalDateToMJD(year, 1, 1);
            Int32 doy = 1 + (int)Math.Round((decYear - year) * nDays);
            return DayOfYearToMJD(year, doy);
        }

        /// <summary>
        /// Calculates a Day Of Year Value From a Modified Julian Date
        /// </summary>
        /// <param name="mjd">Modified Julian Date</param>
        /// <returns>Day Of Year</returns>
        public static Int32 MJDToDayOfYear(Int32 mjd)
        {
            Int32 year = MJDToCalDate(mjd).Year;
            Int32 foy = CalDateToMJD(year, 1, 1);
            return mjd - foy + 1;
        }

        /// <summary>
        /// Calculates a Modified Julian Date Value From Year and Day of Year Values
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="doy">Day Of Year</param>
        /// <returns>Modified Julian Date</returns>
        public static Int32 DayOfYearToMJD(Int32 year, Int32 doy)
        {
            return CalDateToMJD(year, 1, 1) + doy - 1;
        }

        /// <summary>
        /// Calculates a Gps Week Value From a Modified Julian Date
        /// </summary>
        /// <param name="mjd">Modified Julian Date</param>
        /// <returns>Gps Week</returns>
        public static Int32 MJDToGpsWeek(Int32 mjd)
        {
            Int32 mjdGps = CalDateToMJD(1980, 1, 6);
            return (int)Math.Floor((mjd - mjdGps) / 7.0);
        }

        /// <summary>
        /// Calculates a Day of Week Value From a Modified Julian Date
        /// Sunday = 1 ... Saturday = 7
        /// </summary>
        /// <param name="mjd">Modified Julian Date</param>
        /// <returns>Day of Week</returns>
        public static Int32 MJDToDayOfWeek(Int32 mjd)
        {
            return (8 + (int)Math.Floor((mjd + 3) % 7.0)) % 7;
        }

        /// <summary>
        /// Determines if a specific year is a Leap Year
        /// </summary>
        /// <param name="year">Year</param>
        /// <returns>Is a Leap Year</returns>
        public static bool isLeapYear(Int32 year)
        {
            if ((year % 4) == 0 && ((year % 100) != 0 || ((year % 400) == 0))) return true;

            return false;         
        }

        /// <summary>
        /// Determines if a specific calendar date is valid
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <param name="day">Day</param>
        /// <returns>Is Valid</returns>
        public static bool IsValidDayOfMonth(Int32 year, Int32 month, Int32 day)
        {
            if (day < 1) return false;
            if (day > 31) return false;

            if (month == 2 && isLeapYear(year) && day > 29) return false;
            if (month == 2 && !isLeapYear(year) && day > 28) return false;
            if (month == 4 && day > 30) return false;
            if (month == 6 && day > 30) return false;
            if (month == 9 && day > 30) return false;
            if (month == 11 && day > 30) return false;

            return true;

        }

    }

    public enum Weekday
    {
        Sunday = 1,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }

    static class WeekdayMethods
    {
        public static String getShortName(this Weekday day)
        {
            switch (day)
            {
                case Weekday.Sunday:
                    return "Sun";
                case Weekday.Monday:
                    return "Mon";
                case Weekday.Tuesday:
                    return "Tues";
                case Weekday.Wednesday:
                    return "Wed";
                case Weekday.Thursday:
                    return "Thur";
                case Weekday.Friday:
                    return "Fri";
                case Weekday.Saturday:
                    return "Sat";
                default:
                    return "";

            }

        }
    }

    public enum Month
    {
        January = 1,
        February, 
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }


    public class CalendarVector
    {

        public Int32 Year { get; set; }
        public Int32 Month { get; set; }
        public Int32 Day { get; set; }

        public CalendarVector()
        {
            DateTime dateTime = DateTime.Now;

            this.Year = dateTime.Year;
            this.Month = dateTime.Month;
            this.Day = dateTime.Day;

        }

        public CalendarVector(Int32 year, Int32 month, Int32 day)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;

        }

    }

    public enum CalendarValue
    {
        Day_Of_Year,
        Modified_Julian_Date,
        Decimal_Year

    }

}
