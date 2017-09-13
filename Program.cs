using System;
using System.Globalization;
using System.IO;
using System.Collections.ObjectModel;

namespace TimeZones
{
    class Program
    {
        static void Main(string[] args)
        {
            const string OUTPUTFILENAME = @"TimeZoneInfo.txt";

            DateTimeFormatInfo dateFormats = CultureInfo.CurrentCulture.DateTimeFormat;
            ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            StreamWriter sw = new StreamWriter(OUTPUTFILENAME, false);

            foreach (TimeZoneInfo timeZone in timeZones)
            {
                bool hasDST = timeZone.SupportsDaylightSavingTime;
                TimeSpan offsetFromUtc = timeZone.BaseUtcOffset;
                TimeZoneInfo.AdjustmentRule[] adjustRules;
                string offsetString = String.Format("{0:00}:{1:00}", offsetFromUtc.Hours, offsetFromUtc.Minutes);

                sw.WriteLine("{0},{1},{2}", timeZone.Id, timeZone.DisplayName, offsetString);
                sw.Write(hasDST ? "   ***Has " : "   ***Does Not Have ");
                sw.WriteLine("Daylight Saving Time***");

                adjustRules = timeZone.GetAdjustmentRules();
                sw.WriteLine("   Number of adjustment rules: {0, 26}", adjustRules.Length);
                if (adjustRules.Length > 0)
                {
                    sw.WriteLine("   Adjustment Rules:");
                    foreach (TimeZoneInfo.AdjustmentRule rule in adjustRules)
                    {
                        TimeZoneInfo.TransitionTime transTimeStart = rule.DaylightTransitionStart;
                        TimeZoneInfo.TransitionTime transTimeEnd = rule.DaylightTransitionEnd;

                        sw.WriteLine("      From {0} to {1}", rule.DateStart, rule.DateEnd);
                        sw.WriteLine("      Delta: {0}", rule.DaylightDelta);
                        if (!transTimeStart.IsFixedDateRule)
                        {
                            sw.WriteLine("      Begins at {0:t} on {1} of week {2} of {3}", transTimeStart.TimeOfDay,
                                                                                          transTimeStart.DayOfWeek,
                                                                                          transTimeStart.Week,
                                                                                          dateFormats.MonthNames[transTimeStart.Month - 1]);
                            sw.WriteLine("      Ends at {0:t} on {1} of week {2} of {3}", transTimeEnd.TimeOfDay,
                                                                                          transTimeEnd.DayOfWeek,
                                                                                          transTimeEnd.Week,
                                                                                          dateFormats.MonthNames[transTimeEnd.Month - 1]);
                        }
                        else
                        {
                            sw.WriteLine("      Begins at {0:t} on {1} {2}", transTimeStart.TimeOfDay,
                                                                           transTimeStart.Day,
                                                                           dateFormats.MonthNames[transTimeStart.Month - 1]);
                            sw.WriteLine("      Ends at {0:t} on {1} {2}", transTimeEnd.TimeOfDay,
                                                                         transTimeEnd.Day,
                                                                         dateFormats.MonthNames[transTimeEnd.Month - 1]);
                        }
                    }
                }
            }
            sw.Close();
        }
    }
}
