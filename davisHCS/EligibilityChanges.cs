using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using davisHCS.Models;
using Microsoft.EntityFrameworkCore;

namespace davisHCS
{
    internal static class EligibilityChanges
    {
        internal static Random rnd = new Random();

        // Simulate a single days of eligibility changes across the 10K Member population:
        // 1. Randomly pick twenty enrolled Members who are subscribers (i.e., not linked to another MemberId) and disenroll them.
        // 2. If there are more than 100 people disenrolled, randomly re-enroll twenty of them.

        // Randomly pick ten more members who have active eligibility and do the following:
        // 1.  Change their PCP to a random PCP

        internal static async Task TickAsync(DateTime dt)
        {
            // All effective dates will be to the first of them month:
            DateTime firstOfMonth = new DateTime(dt.Year, dt.Month, 1);
            SortedSet<int> memberAffectedId = new SortedSet<int>();

            using (var con = new cediMCSimContext())
            {
                int eligTrackId = con.Tracks.Where(a => a.Name == "Eligibility").First().Id;
                int pcpTrackId = con.Tracks.Where(a => a.Name == "PCP").First().Id;

                // Get a list of all Subscribers member Id's that are active at the beginning of this month
                var eligsubs = await
                    (from memelig in con.VwEligSubscribers
                     orderby memelig.MemId, memelig.Fromdt
                     where memelig.Fromdt <= firstOfMonth && (memelig.Throughdt == null  || memelig.Throughdt > firstOfMonth) && memelig.EligStatus == "ELIGIBLE"
                     select new { memelig.MemId, memelig.Fromdt, memelig.EligStatus }
                     ).ToListAsync();

                SortedSet<int> disenrollees = new SortedSet<int>();

                // Randomly Disenroll twenty enrolled Subscribers
                for (int i = 0; i < 20; i++)
                {
                    int idx = rnd.Next(0, eligsubs.Count - 1);
                    if (!disenrollees.Contains(eligsubs[idx].MemId))
                    {
                        disenrollees.Add(eligsubs[idx].MemId);
                        memberAffectedId.Add(eligsubs[idx].MemId);
                    }
                }

                // Create a new IntegrationActivity for our disenrollments, then create TrackChange records to do it
                IntegrationActivity ia = new IntegrationActivity() { ProcessSource = "DISENROLLMENTS " + dt.ToString() };
                con.IntegrationActivities.Add(ia);

                foreach (int i in disenrollees)
                    con.TrackChanges.Add(new TrackChange() { MemberId = i, EffectiveDt = firstOfMonth, IntegrationActivity = ia, TrackId = eligTrackId, TrackDataChar = "TERM" });

                // Commit all changes
                await con.SaveChangesAsync();

                var eligsubs2 = await
                    (from memelig in con.VwEligSubscribers
                     orderby memelig.MemId, memelig.Fromdt
                     where memelig.Fromdt <= firstOfMonth && memelig.Throughdt == null && memelig.EligStatus == "TERM"
                     select new { memelig.MemId, memelig.Fromdt}
                     ).ToListAsync();

                // If > 100 disenrolled, randomly re-enroll 20 Subscribers.
                SortedSet<int> enrollees = new SortedSet<int>();
                if (eligsubs2.Count() > 100)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        int idx = rnd.Next(0, eligsubs2.Count() - 1);
                        if (!enrollees.Contains(eligsubs2[idx].MemId))
                            if (!memberAffectedId.Contains(eligsubs2[idx].MemId))
                            {
                                enrollees.Add(eligsubs2[idx].MemId);
                                memberAffectedId.Add(eligsubs2[idx].MemId);
                            }
                    }
                }

                // Create a new IntegrationActivity for our enrollments, then create TrackChange records to do it
                ia = new IntegrationActivity() { ProcessSource = "ENROLLMENTS " + dt.ToString() };
                con.IntegrationActivities.Add(ia);

                foreach (int i in enrollees)
                    con.TrackChanges.Add(new TrackChange() { MemberId = i, EffectiveDt = firstOfMonth, IntegrationActivity = ia, TrackId = eligTrackId, TrackDataChar = "ELIGIBLE" });

                // Get a list of all member IDs who are eligible right now.
                var eligmem = await
                    (from memelig in con.VwEligMembers
                     orderby memelig.MemId, memelig.Fromdt
                     where memelig.Fromdt <= firstOfMonth && (memelig.Throughdt == null || memelig.Throughdt > firstOfMonth)
                     select new { memelig.MemId, memelig.Fromdt }
                     ).ToListAsync();

                // Randomly change ten member's PCP
                ia = new IntegrationActivity() { ProcessSource = "PCP CHANGES " + dt.ToString() };
                int pcpCount = con.Providers.Count();
                con.IntegrationActivities.Add(ia);

                SortedSet<int> pcpChg = new SortedSet<int>();
                for (int i = 1; i <= 10; i++)
                {
                    int idx = rnd.Next(0, eligmem.Count());
                    if (!pcpChg.Contains(eligmem[idx].MemId))
                        if (!memberAffectedId.Contains(eligmem[idx].MemId))
                            pcpChg.Add(eligmem[idx].MemId);                    
                }
                foreach (int memId in pcpChg)
                    con.TrackChanges.Add(new TrackChange() { MemberId = memId, EffectiveDt = firstOfMonth, TrackDataInt = rnd.Next(0, pcpCount), TrackId = pcpTrackId, IntegrationActivity = ia });

                // Commit all changes
                await con.SaveChangesAsync();

                // Process all changes
                await ChangeProcessor.ProcessTrackChangesAsync();

            }
        }
    }
}
