using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using davisHCS.Models;
using Microsoft.EntityFrameworkCore;

namespace davisHCS
{    
    internal static class ChangeProcessor
    {
        // A TrackChange is a request to change a MemberTrack record, which tracks changes to a Member record according to several "tracks" - for example, eligibility status, current LOB, premium rate, current PCP, or whatever is
        // registered as a "track".  Each record has a single effective date, assuming an "open termination" afterwards.
        // Any records in MemberTrack with an effective date at or after the effective date in the TrackChange record will be invalidated.
        // The highest effective record with a date earlier than this effective date will be relinked to this new record.  Each MemberTrack is processed in isolation to other MemberTracks.
        
        internal static async Task ProcessTrackChangesAsync()
        {
            using (var con = new cediMCSimContext())
            {
                var memberTracks = await con.MemberTracks.ToListAsync();

                // Sort these records in Member / Track / Effective order.
                var tcTBD = await con.TrackChanges.Where(a => a.Processed == 0).OrderBy(a=>a.TrackId).ThenBy(a=>a.MemberId).ThenBy(a=>a.EffectiveDt).ToListAsync();

                foreach (var tc in tcTBD)
                {
                    tc.Processed = 1;

                    // NextSeq Calculation
                    int nextSeq = memberTracks.Where(a => a.MemberId == tc.MemberId && a.TrackId == tc.TrackId).Count() + 1;

                    // Invalidate all valid MemberTrack records with an effective date greater than or equal  to this EffectiveDate
                    var memTrackRecords = 
                        memberTracks.Where(a => a.MemberId == tc.MemberId && a.TrackId == tc.TrackId && a.EffectiveDt >= tc.EffectiveDt && a.Valid == 1).ToList();

                    if (memTrackRecords != null && memTrackRecords.Count() > 0)
                        foreach (var tr in memTrackRecords)
                            tr.Valid = 0;

                    var newMT = new MemberTrack()
                    {
                        MemberId = tc.MemberId,
                        TrackId = tc.TrackId,
                        Valid = 1,
                        EffectiveDt = tc.EffectiveDt,
                        TrackDataChar = tc.TrackDataChar,
                        TrackDataInt = tc.TrackDataInt,
                        Seq = nextSeq
                    };

                    con.MemberTracks.Add(newMT);

                    // Restitch any previous records
                    if (nextSeq != 1)
                    {
                        var seq = memberTracks.Where(a => a.MemberId == tc.MemberId && a.TrackId == tc.TrackId && a.Valid == 1 && a.EffectiveDt < tc.EffectiveDt).Select(a=>a.Seq).ToList();
                        if (seq.Count > 0)
                        {
                            int maxSeq = seq.Max();
                            var mt = con.MemberTracks.Where(a => a.MemberId == tc.MemberId && a.TrackId == tc.TrackId && a.Seq == maxSeq).FirstOrDefault();
                            if (mt != null)
                            {
                                mt.NextSeq = nextSeq;
                            }
                        }
                    }
                }
                await con.SaveChangesAsync();
            }            
        }
    }
}
