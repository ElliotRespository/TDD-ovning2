using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Trangselskatt
{
    public class Metoder
    {
        private static readonly Dictionary<(TimeSpan Start, TimeSpan Slut), int> Avgiftsintervall = new Dictionary<(TimeSpan, TimeSpan), int>
        {
            { (new TimeSpan(6, 0, 0), new TimeSpan(6, 29, 59)), 8 },
            { (new TimeSpan(6, 30, 0), new TimeSpan(6, 59, 59)), 13 },
            { (new TimeSpan(7, 0, 0), new TimeSpan(7, 59, 59)), 18 },
            { (new TimeSpan(8, 0, 0), new TimeSpan(8, 29, 59)), 13 },
            { (new TimeSpan(8, 30, 0), new TimeSpan(14, 59, 59)), 8 },
            { (new TimeSpan(15, 0, 0), new TimeSpan(15, 29, 59)), 13 },
            { (new TimeSpan(15, 30, 0), new TimeSpan(16, 59, 59)), 18 },
            { (new TimeSpan(17, 0, 0), new TimeSpan(17, 59, 59)), 13 },
            { (new TimeSpan(18, 0, 0), new TimeSpan(18, 29, 59)), 8 },
            { (new TimeSpan(18, 30, 0), new TimeSpan(5, 59, 59)), 0 }
        };

        public static void RäknaTotalBelopp(string datumTider)
        {
            var tidpunkter = ParseInputData(datumTider);
            var dagligaAvgifter = BeräknaDagligaAvgifter(tidpunkter);

            int totalAvgift = dagligaAvgifter.Values.Sum();
            Console.WriteLine($"Total avgiften är {totalAvgift} kr");
        }

        public static List<DateTime> ParseInputData(string datumTider)
        {
            return datumTider.Split(',')
            .Select(dt => dt.Trim())
            .Select(dtStr =>
            {
            if (DateTime.TryParseExact(dtStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tidpunkt))
            {
              return (DateTime?)tidpunkt;
            }
              return null;
            })
             .Where(dt => dt.HasValue)
             .Select(dt => dt!.Value)
             .ToList();
        }


        public static Dictionary<DateTime, int> BeräknaDagligaAvgifter(List<DateTime> tidpunkter)
        {
            var dagligaAvgifter = new Dictionary<DateTime, int>();
            DateTime? senasteDebiteradeTidpunkt = null;
            int senasteAvgift = 0;

            foreach (var tidpunkt in tidpunkter.OrderBy(t => t))
            {
                if (senasteDebiteradeTidpunkt.HasValue && tidpunkt < senasteDebiteradeTidpunkt.Value.AddMinutes(60))
                {
                    // Inom 60 minuter från senaste passage, uppdatera endast om högre avgift
                    int aktuellAvgift = BeräknaAvgift(tidpunkt);
                    if (aktuellAvgift > senasteAvgift)
                    {
                        dagligaAvgifter[tidpunkt.Date] = dagligaAvgifter[tidpunkt.Date] - senasteAvgift + aktuellAvgift;
                        senasteAvgift = aktuellAvgift;
                    }
                }
                else
                {
                    // Ny debitering
                    senasteDebiteradeTidpunkt = tidpunkt;
                    senasteAvgift = BeräknaAvgift(tidpunkt);
                    if (dagligaAvgifter.ContainsKey(tidpunkt.Date))
                    {
                        dagligaAvgifter[tidpunkt.Date] += senasteAvgift;
                    }
                    else
                    {
                        dagligaAvgifter[tidpunkt.Date] = senasteAvgift;
                    }
                }
            }

            return dagligaAvgifter;
        }

        public static int BeräknaAvgift(DateTime tidpunkt)
        {
            if (ÄrAvgiftsfriDag(tidpunkt))
            {
                return 0;
            }

            TimeSpan tid = tidpunkt.TimeOfDay;
            foreach (var intervall in Avgiftsintervall)
            {
                if (tid >= intervall.Key.Start && tid < intervall.Key.Slut)
                {
                    return intervall.Value;
                }
            }

            return 0;
        }

        public static bool ÄrAvgiftsfriDag(DateTime datum)
        {
            return datum.DayOfWeek == DayOfWeek.Saturday || datum.DayOfWeek == DayOfWeek.Sunday || datum.Month == 7;
        }
    }
}
