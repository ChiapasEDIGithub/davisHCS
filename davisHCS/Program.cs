using System;
using System.Threading.Tasks;

// First Name database: Copyright © 2015. The Compute.io Authors.
namespace davisHCS
{
    public class Program
    {
        internal static string gCnn = "";

        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("DAVISHCS - Chiapas EDI Technologies Health Care Simulator");
                Console.WriteLine("(C) 2022 Chiapas EDI Technologies, Inc.  Licensed under the MIT License.");                
                Console.WriteLine(@"Usage: davishcs.exe ""Database String"" <option>");
                Console.WriteLine("Options:");
                Console.WriteLine("INIT - Delete (if exists) and create initial population of 10000 members, providers, eligibility & PCP records.  Takes about 2 minutes.");
                Console.WriteLine("ELIG <days> - Simulate <days> worth of random eligibility and PCP changes.  365 will simulate a full year of changes.");
                Console.WriteLine("ELIG <date> - (Format: '2022-01-01' YYYY-MM-DD0 Simulate random eligibility and PCP changes for a specific date.");
            }
            else
            {

                if (args.Length > 1)
                {
                    gCnn = args[0];

                    if (args[1].ToUpper() == "INIT")
                    {
                        SeedData.Initialize();
                        await SeedData.GenerateSeedPopulation();
                    }

                    // 
                    if (args[1].ToUpper() == "ELIG")
                    {
                        if (args.Length > 1 && args[1].Length > 0)
                        {

                            if (DateTime.TryParse(args[2], out DateTime dt))
                            {
                                await EligibilityChanges.TickAsync(dt);
                                Console.WriteLine(dt.ToShortDateString());
                            }
                            else
                            {
                                if (int.TryParse(args[2], out int days))
                                {
                                    for (int i = 1; i < days; i++)
                                    {
                                        DateTime dt2 = Convert.ToDateTime("2022-01-01").AddDays(i);
                                        await EligibilityChanges.TickAsync(dt2);
                                        Console.WriteLine(dt2.ToShortDateString());
                                    }
                                }
                            }
                        }
                    }

                    if (args[1].ToUpper() == "TEST")
                    {
                        // Process all changes
                        await ChangeProcessor.ProcessTrackChangesAsync();
                    }
                }
            }
        }
    }
}
