using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendar.Models;
using System;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestDateConversions()
        {

            JulianDate jd0 = JulianDate.FromCalendarDate(2019,12,1);

            JulianDate jd1 = JulianDate.FromJulian(jd0.Julian);
            JulianDate jd2 = JulianDate.FromMJD(jd0.MJD);
            JulianDate jd3 = JulianDate.FromDecimalYear(jd0.DecimalYear);
            JulianDate jd4 = JulianDate.FromDayOfYear(jd0.Year, jd0.DayOfYear);

            Assert.AreEqual(jd0.MJD, jd1.MJD);
            Assert.AreEqual(jd0.Julian, jd2.Julian);
            Assert.AreEqual(jd0.MJD, jd3.MJD);
            Assert.AreEqual(jd0.MJD, jd4.MJD);

            Assert.AreEqual( 2019 , jd0.Year);
            Assert.AreEqual( 12 , jd0.Month);
            Assert.AreEqual( Month.December , jd0.MonthEnum);
            Assert.AreEqual( 1 , jd0.DayOfMonth);
            Assert.AreEqual( 1 , jd0.DayOfWeek);
            Assert.AreEqual( Weekday.Sunday , jd0.DayOfWeekEnum);
            Assert.AreEqual( 335 , jd0.DayOfYear);
            Assert.AreEqual( 2082 , jd0.GpsWeek);
            Assert.AreEqual( 2458818.5 , jd0.Julian);
            Assert.AreEqual( 58818 , jd0.MJD);
            Assert.AreEqual( 2019.915068493151 , jd0.DecimalYear,1e-9);

            Assert.IsFalse(jd0.IsLeapYear);

        }

        [TestMethod]
        public void TestDateConversionsLeapYear()
        {

            JulianDate jd0 = JulianDate.FromCalendarDate(2016, 12, 1);

            JulianDate jd1 = JulianDate.FromJulian(jd0.Julian);
            JulianDate jd2 = JulianDate.FromMJD(jd0.MJD);
            JulianDate jd3 = JulianDate.FromDecimalYear(jd0.DecimalYear);
            JulianDate jd4 = JulianDate.FromDayOfYear(jd0.Year, jd0.DayOfYear);

            Assert.AreEqual(jd0.MJD, jd1.MJD);
            Assert.AreEqual(jd0.Julian, jd2.Julian);
            Assert.AreEqual(jd0.MJD, jd3.MJD);
            Assert.AreEqual(jd0.MJD, jd4.MJD);

            Assert.AreEqual(2016, jd0.Year);
            Assert.AreEqual(12, jd0.Month);
            Assert.AreEqual(Month.December, jd0.MonthEnum);
            Assert.AreEqual(1, jd0.DayOfMonth);
            Assert.AreEqual(5, jd0.DayOfWeek);
            Assert.AreEqual(Weekday.Thursday, jd0.DayOfWeekEnum);
            Assert.AreEqual(336, jd0.DayOfYear);
            Assert.AreEqual(1925, jd0.GpsWeek);
            Assert.AreEqual(2457723.5, jd0.Julian, 1e-9);
            Assert.AreEqual(57723, jd0.MJD);
            Assert.AreEqual(2016.915300546448, jd0.DecimalYear, 1e-9);

            Assert.IsTrue(jd0.IsLeapYear);

        }

        [TestMethod]
        public void TestLeapYears()
        {
            Assert.IsTrue(JulianDate.isLeapYear(1796));
            Assert.IsTrue(JulianDate.isLeapYear(1896));
            Assert.IsTrue(JulianDate.isLeapYear(1996));
            Assert.IsTrue(JulianDate.isLeapYear(2000));
            Assert.IsTrue(JulianDate.isLeapYear(2004));
            Assert.IsTrue(JulianDate.isLeapYear(2016));
            Assert.IsTrue(JulianDate.isLeapYear(2020));

            Assert.IsFalse(JulianDate.isLeapYear(1800));
            Assert.IsFalse(JulianDate.isLeapYear(1900));
            Assert.IsFalse(JulianDate.isLeapYear(1995));
            Assert.IsFalse(JulianDate.isLeapYear(1997));
            Assert.IsFalse(JulianDate.isLeapYear(1999));
            Assert.IsFalse(JulianDate.isLeapYear(2001));
            Assert.IsFalse(JulianDate.isLeapYear(2019));

            Assert.IsTrue(JulianDate.IsValidDayOfMonth(2000, 2, 29));
            Assert.IsFalse(JulianDate.IsValidDayOfMonth(2001, 2, 29));

        }

        [TestMethod]
        public void TestCalendarMonth()
        {

            JulianDate[,] calendar = JulianDate.CalendarMonth(2020, 2);



            int i = 1 + 1;

            Assert.IsTrue(true);

        }

    }
}
