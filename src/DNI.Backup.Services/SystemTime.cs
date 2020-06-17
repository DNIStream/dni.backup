using System;

namespace DNI.Backup.Services {
    public static class SystemTime {
        /// <summary>
        ///     Provides a mockable <see cref="DateTime" /> function.
        ///     Use as follows to override current time,
        ///     and reference time with SystemTime.Now in code.
        ///     SystemTime.Now = () => new DateTime(2009, 1, 1);
        /// </summary>
        public static Func<DateTime> Now = () => DateTime.Now;

        /// <summary>
        ///     Resets the static method to use the current time
        /// </summary>
        public static void Reset() {
            Now = () => DateTime.Now;
        }

        /// <summary>
        ///     If the supplied <see cref="DateTime" /> falls on a weekend,
        ///     it will be set to the following Monday;
        ///     otherwise it is not changed.
        /// </summary>
        /// <param name="date"><see cref="DateTime" /> to be checked</param>
        /// <returns>Amended <see cref="DateTime" /></returns>
        public static DateTime WeekdayOnly(DateTime date) {
            switch(date.DayOfWeek) {
                case DayOfWeek.Saturday:
                    return date.AddDays(2);
                case DayOfWeek.Sunday:
                    return date.AddDays(1);
                default:
                    return date;
            }
        }

        /// <summary>
        ///     If the current <see cref="DateTime" /> falls on a weekend,
        ///     it will be set to the following Monday;
        ///     otherwise it is not changed.
        /// </summary>
        /// <returns>Amended <see cref="DateTime" /></returns>
        public static DateTime WeekdayOnly() {
            return WeekdayOnly(Now());
        }
    }
}