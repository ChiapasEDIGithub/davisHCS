using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using davisHCS.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft;

namespace davisHCS
{
    public class SeedData
    {
        internal static int gMemberPopulation = 10000;
        internal static int gChildTarget = 1500;
        internal static int gSpouseTarget = 1000;

        internal static Random rnd = new Random();

        // Initial Migration: Scaffold-DbContext "Server=(local);Database=cediMCSim;User id=sa;Password=strongPass1;" Microsoft.EntityFrameworkCore.SqlServer -Output Models -Force
        internal static List<string> maleFirstNames = new List<string>();
        internal static List<string> femaleFirstNames = new List<string>();
        internal static List<string> lastNames = new List<string>();
        internal static List<int> sanFranciscoZipCodes = new List<int> { 94016, 94102, 94104, 94105, 94107, 94108, 94109, 94110, 94111, 94112, 94114, 94115, 94116, 94117, 94118, 94119, 94120, 94121, 9412, 94123, 94124, 94125, 94126, 94127, 94129, 94130, 94131, 94132, 94133, 94134, 94137, 94139, 94140, 94141, 94142, 94143, 94144, 94144, 94145, 94146, 94147, 94151, 94158, 94159, 94160, 94161, 94163, 94164, 94172, 94177, 94188 };

        public static void Initialize()
        {
            using (var con = new cediMCSimContext())
            {
                // Completely remove existing data
                con.Database.EnsureDeleted();
                con.Database.EnsureCreated();
                
                var cnn = new SqlConnection(Program.gCnn);
                cnn.Open();
                var cmd = new SqlCommand(@"
                    CREATE VIEW [dbo].[VW_ELIG_SUBSCRIBERS]
                    AS
                    SELECT			MEM.ID MEM_ID,
				                    MT.EffectiveDt FROMDT,
				                    MT2.EffectiveDt THROUGHDT,
				                    MT.TrackDataChar ELIG_STATUS
                    FROM			MEMBER MEM
                    INNER JOIN		MEMBERTRACK MT
                    ON				MT.MEMBERID = MEM.ID
                    AND				MT.VALID = 1
                    LEFT JOIN		MEMBERTRACK MT2
                    ON				MT2.MEMBERID = MEM.ID
                    AND				MT.NextSeq = MT2.SEQ
                    AND				MT.TRACKID = MT2.TRACKID
                    WHERE			MEM.RELATIONMEMBERID IS NULL
                    AND				MT.TRACKID = (SELECT ID FROM TRACK WHERE NAME='ELIGIBILITY')", cnn);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand(@"
                    CREATE VIEW [dbo].[VW_ELIG_MEMBERS]
                    AS
                    SELECT		SUB.MEM_ID SUB_ID,
			                    MEM.ID MEM_ID,
			                    FROMDT,
			                    THROUGHDT
                    FROM		[VW_ELIG_SUBSCRIBERS] SUB
                    INNER JOIN	[MEMBER] MEM
                    ON			SUB.MEM_ID = COALESCE(MEM.RELATIONMEMBERID, MEM.ID)
                    WHERE		SUB.ELIG_STATUS = 'ELIGIBLE'", cnn);
                cmd.ExecuteNonQuery();
                cnn.Close();

            }            
        }

        // Generate a mock address in San Francisco
        internal static Location GenerateMockLocation(string locationType)
        {
            StringBuilder address = new StringBuilder();
            address.Append(rnd.Next(1000, 10000).ToString() + " ");
            switch (rnd.Next(1, 10))
            {
                case 1:
                    address.Append("MAIN ST");
                    break;
                case 2:
                    address.Append("FICTION DR");
                    break;
                case 3:
                    address.Append("MOCK AVE");
                    break;
                case 4:
                    address.Append("MADEUP LN");
                    break;
                case 5:
                    address.Append("SIMULATED ST");
                    break;
                case 6:
                    address.Append("EMULATED DR");
                    break;
                case 7:
                    address.Append("VIRTUAL AVE");
                    break;
                case 8:
                    address.Append("INVENTED LN");
                    break;
                default:
                    address.Append("SYNTHESIZED ST");
                    break;
            }
            string zipCd = sanFranciscoZipCodes[rnd.Next(0, sanFranciscoZipCodes.Count)].ToString() + "0000";

            return new Location { AddressLine1 = address.ToString(), City = "SAN FRANCISCO", ZipCd = zipCd, LocationType = locationType };
        }

        internal static Provider GenerateMockProvider(string providerType, Language l, string zipCd, string orgName)
        {
            Provider p = new Provider();
            p.Language = l;
            p.ProviderType = providerType;
            p.Location = GenerateMockLocation("BUSINESS");
            p.Location.ZipCd = zipCd;
            p.Npi = "1234567897";
            p.Ein = rnd.Next(900000000, 999999999).ToString();
            if (!string.IsNullOrEmpty(orgName))
                p.OrgName = orgName;
            else
            {
                p.PhysicianDateOfBirth = Convert.ToDateTime("1970-01-01");
                p.PhysicianGender = rnd.Next(1, 3) == 1 ? "M" : "F";
                p.PhysicianLastName = lastNames[rnd.Next(0, lastNames.Count)];
                if (p.PhysicianGender == "M")
                {
                    p.PhysicianFirstName = maleFirstNames[rnd.Next(0, maleFirstNames.Count)].ToUpper();
                    p.PhysicianMiddleName = maleFirstNames[rnd.Next(0, maleFirstNames.Count)].ToUpper();
                }
                else
                {
                    p.PhysicianFirstName = femaleFirstNames[rnd.Next(0, femaleFirstNames.Count)].ToUpper();
                    p.PhysicianMiddleName = femaleFirstNames[rnd.Next(0, femaleFirstNames.Count)].ToUpper();

                }
                p.Ssn = rnd.Next(100000000, 900000000).ToString();
                p.Language = l;
            }
            return p;
        }

        public static async Task GenerateSeedPopulation()
        {
            List<Member> mems = new List<Member>();
            List<Location> locs = new List<Location>();
            List<MemberLob> memLob = new List<MemberLob>();
            Dictionary<string, List<Provider>> dctLngProv = new Dictionary<string, List<Provider>>();
            Dictionary<string, List<Provider>> dctZipProv = new Dictionary<string, List<Provider>>();

            // Load in the first names and last names from open-source JSON sources.
            string namesFile = File.ReadAllText(Path.Join(Environment.CurrentDirectory, "names.json"));
            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(namesFile);

            //var obj =JsonDocument.Parse(namesFile);
            foreach (var token in ((Newtonsoft.Json.Linq.JArray)(obj["malefirstname"])).ToList()) { maleFirstNames.Add(token.ToString()); }
            foreach (var token in ((Newtonsoft.Json.Linq.JArray)(obj["femalefirstname"])).ToList()) { femaleFirstNames.Add(token.ToString()); }
            foreach (var token in ((Newtonsoft.Json.Linq.JArray)(obj["lastname"])).ToList()) { lastNames.Add(token.ToString()); }

            // Establish a root member population
            using (var con = new cediMCSimContext())
            {

                ///////////////////////////////////////////////////////////////////////////////////////////////////
                // Member Demographics

                var integrationActivity = new IntegrationActivity { Name = "Initial Data Seed" };
                con.IntegrationActivities.Add(integrationActivity);

                // Seed Language Data
                con.Languages.Add(new Language { LanguageName = "English" });
                con.Languages.Add(new Language { LanguageName = "Spanish" });
                con.Languages.Add(new Language { LanguageName = "Vietnamese" });
                con.Languages.Add(new Language { LanguageName = "Chinese (Mandarin)" });
                con.Languages.Add(new Language { LanguageName = "Chinese (Cantonese)" });
                con.Languages.Add(new Language { LanguageName = "Russian" });
                con.Languages.Add(new Language { LanguageName = "French" });
                con.Languages.Add(new Language { LanguageName = "Korean" });
                con.Languages.Add(new Language { LanguageName = "Hebrew" });

                // Seed Ethnicity Data
                con.Ethnicities.Add(new Ethnicity { EthnicityName = "Caucasian" });
                con.Ethnicities.Add(new Ethnicity { EthnicityName = "Hispanic" });
                con.Ethnicities.Add(new Ethnicity { EthnicityName = "Asian or Pacific Islander" });
                con.Ethnicities.Add(new Ethnicity { EthnicityName = "Black" });
                con.Ethnicities.Add(new Ethnicity { EthnicityName = "American Indian or Alaskan Native" });

                await con.SaveChangesAsync();

                // First pass:  Create 10K Demographic Members
                for (int i = 1; i <= gMemberPopulation; i++)
                {
                    Member m = new Member();
                    m.IntegrationActivity = integrationActivity;
                    m.EthnicityId = rnd.Next(1, 6);
                    m.LanguageId = rnd.Next(1, 11) <= 9 ? 1 : rnd.Next(2, 10);
                    m.Ssn = rnd.Next(100000000, 900000000).ToString();
                    m.LastName = lastNames[rnd.Next(0, lastNames.Count)];

                    // Assign the age to one of four groups: 5-17; 18 - 25; 26 - 55; 56-100
                    int ageGroup = rnd.Next(1, 5);
                    switch (ageGroup)
                    {
                        case 1:
                            m.DateOfBirth = DateTime.Now.AddDays(-rnd.Next(Convert.ToInt32(365.25 * 1), Convert.ToInt32(365.25 * 18)));
                            m.RelationCd = "Child";
                            break;
                        case 2:
                            m.DateOfBirth = DateTime.Now.AddDays(-rnd.Next(Convert.ToInt32(365.25 * 18), Convert.ToInt32(365.25 * 25)));
                            break;
                        case 3:
                            m.DateOfBirth = DateTime.Now.AddDays(-rnd.Next(Convert.ToInt32(365.25 * 26), Convert.ToInt32(365.25 * 55)));
                            break;
                        default:
                            m.DateOfBirth = DateTime.Now.AddDays(-rnd.Next(Convert.ToInt32(365.25 * 56), Convert.ToInt32(365.25 * 100)));
                            break;
                    }

                    // Male or Female
                    int genderSeed = rnd.Next(1, 100);
                    if (genderSeed <= 49)
                    {
                        m.GenderCd = "F";
                        m.FirstName = femaleFirstNames[rnd.Next(0, femaleFirstNames.Count)].ToUpper();
                        m.MiddleName = femaleFirstNames[rnd.Next(0, femaleFirstNames.Count)].ToUpper();
                    }
                    else
                    {
                        m.GenderCd = "M";
                        m.FirstName = maleFirstNames[rnd.Next(0, maleFirstNames.Count)].ToUpper();
                        m.MiddleName = maleFirstNames[rnd.Next(0, maleFirstNames.Count)].ToUpper();
                    }

                    // Keep Members untracked for right now to keep the family module working on an in-memory list of Members.
                    mems.Add(m);
                }

                // 95% will be M/F Spousal relationships, 5% will be same sex spousal relationships.  All relationships will have same address.
                for (int i = 1; i < gSpouseTarget; i++)
                {
                    int findMemberId = 0;

                    while (true)
                    {
                        findMemberId = rnd.Next(0, gMemberPopulation);
                        if (mems[findMemberId].RelationCd == null)
                        {
                            int now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                            int dob = int.Parse(mems[findMemberId].DateOfBirth.Value.ToString("yyyyMMdd"));
                            int age = (now - dob) / 10000;

                            if (age >= 25 && age <= 60)
                                break;
                        }
                    }

                    var m = mems[findMemberId];
                    string spouseGenderCd = rnd.Next(1, 100) < 95 ? (m.GenderCd == "M" ? "F" : "M") : m.GenderCd;
                    var spouses = mems.Where(a => a.RelationCd == null && a.DateOfBirth.Value.AddDays(-1500) < m.DateOfBirth.Value && a.DateOfBirth.Value.AddDays(1500) > m.DateOfBirth.Value && a.GenderCd == spouseGenderCd && a != m);
                    var possibleSpouse = spouses.ToArray()[rnd.Next(1, spouses.Count())];

                    if (possibleSpouse != null)
                    {
                        possibleSpouse.LastName = m.LastName;
                        possibleSpouse.RelationMember = m;
                        possibleSpouse.RelationCd = "Spouse";
                        possibleSpouse.RelationMember = m;
                        m.RelationCd = "Subscriber";
                    }
                }

                var parents = mems.Where(a => a.DateOfBirth.Value > DateTime.Now.AddYears(-60) && a.DateOfBirth.Value < DateTime.Now.AddYears(-25));

                for (int i = 1; i < gChildTarget; i++)
                {
                    int findMemberId = 0;

                    while (true)
                    {
                        findMemberId = rnd.Next(0, gMemberPopulation);

                        if (mems[findMemberId].RelationCd == "Child")
                            break;
                    }

                    var m = mems[findMemberId];
                    var parentMember = parents.ToArray()[rnd.Next(1, parents.Count())];
                    var realParent = parentMember.RelationMember == null ? parentMember : parentMember.RelationMember;
                    m.LastName = realParent.LastName;
                    m.RelationCd = "Child";
                    m.RelationMember = realParent;
                    realParent.RelationCd = "Subscriber";
                }

                // Now, create addresses for population.  This will assume everyone lives in San Francisco at a fictitious street address.  All family members will share the same address.
                foreach (var m in mems)
                {
                    if (m.RelationCd == null || m.RelationCd == "Subscriber" || (m.RelationCd == "Child" && m.RelationMember == null))
                    {
                        Location l = GenerateMockLocation("RESIDENTIAL");
                        m.ResidentialLocation = l;
                        locs.Add(l);
                    }
                }

                // Now, children and spouses linked to a subscriber
                foreach (var m in mems)
                    if (m.RelationMember != null)
                        m.ResidentialLocation = m.RelationMember.ResidentialLocation;

                // Mailing is same as Residential.
                foreach (var m in mems)
                    m.MailingLocation = m.ResidentialLocation;

                // Commit address to database
                foreach (var l in locs)
                    await con.Locations.AddAsync(l);

                // Commit Members to database
                foreach (var m in mems)
                    if (m.RelationMember == null)
                        await con.Members.AddAsync(m);

                foreach (var m in mems)
                    if (m.RelationCd == "Subscriber")
                        await con.Members.AddAsync(m);

                foreach (var m in mems)
                    if (m.RelationCd == "Spouse")
                        await con.Members.AddAsync(m);

                foreach (var m in mems)
                    if (m.RelationCd == "Child")
                    {
                        // Clear out the Child relationship code if they're not actually related to someone
                        if (m.RelationMember == null) m.RelationCd = null;
                        await con.Members.AddAsync(m);
                    }

                await con.SaveChangesAsync();

                ///////////////////////////////////////////////////////////////////////////////////////////////////
                // Providers & Physicians

                List<Provider> provs = new List<Provider>();

                // Create five provider organizations
                for (int i = 1; i <= 5; i++)
                {
                    var p = GenerateMockProvider("ORGANIZATION", con.Languages.First(), sanFranciscoZipCodes.First().ToString() + "0000", "GENERIC PROVIDER ORG" + i.ToString("00"));
                    await con.Locations.AddAsync(p.Location);
                    await con.Providers.AddAsync(p);
                    provs.Add(p);
                }
                await con.SaveChangesAsync();

                int maxLanguages = con.Languages.Count();
                int currentLang = maxLanguages;

                var lang = con.Languages.ToList();
                foreach (var l in lang) dctLngProv[l.LanguageName] = new List<Provider>();
                foreach (int zip in sanFranciscoZipCodes) dctZipProv[zip.ToString() + "0000"] = new List<Provider>();

                // Add a three physicians for every zip code.  Rotate through languages.
                for (int i = 1; i <= 3; i++)
                {
                    foreach (var zip in sanFranciscoZipCodes)
                    {
                        var p = GenerateMockProvider("PHYSICIAN", lang[currentLang - 1], zip.ToString() + "0000", null);
                        dctLngProv[lang[currentLang - 1].LanguageName].Add(p);
                        dctZipProv[zip.ToString() + "0000"].Add(p);

                        await con.Locations.AddAsync(p.Location);
                        await con.Providers.AddAsync(p);

                        currentLang--;
                        if (currentLang == 0) currentLang = maxLanguages;
                    }
                }

                await con.SaveChangesAsync();

                ///////////////////////////////////////////////////////////////////////////////////////////////////
                // Eligibility & PCP Member Tracks

                Track eligibilityTrack = new Track()
                {
                    Name = "Eligibility",
                    ValidFrom = Convert.ToDateTime("2022-01-01"),
                    ValidTo = Convert.ToDateTime("2099-12-31")
                };

                Track pcpTrack = new Track()
                {
                    Name = "PCP",
                    ValidFrom = Convert.ToDateTime("2022-01-01"),
                    ValidTo = Convert.ToDateTime("2099-12-31")
                };

                await con.Tracks.AddAsync(eligibilityTrack);
                await con.Tracks.AddAsync(pcpTrack);

                // Create a MemberLob record for every subscriber, linking them to one of the five provider organizations.  Members with a relationMember (Head of Household) do not get this, as their eligibility is
                // determined through the subscribers MemberTrack.
                foreach (var m in mems)
                {
                    if (m.RelationMember == null)
                    {
                        MemberLob ml = new MemberLob { LineOfBusiness = "HEALTHY-PEOPLE", Source = "Seed", OrganizationProvider = provs[rnd.Next(1, 5)], Member = m };
                        memLob.Add(ml);
                        con.MemberLobs.Add(ml);

                        // We don't make changes to the MemberTrack directly - we add a record to the TrackChange table, which will then ensure that the change is placed correctly there.
                        TrackChange tc = new TrackChange {  Member = m, Track = eligibilityTrack, EffectiveDt = Convert.ToDateTime("2022-01-01"), TrackDataChar = "ELIGIBLE" };
                        con.TrackChanges.Add(tc);
                    }
                }
                await con.SaveChangesAsync();

                // Select a PCP - if the member speaks english, pick a PCP in their ZIP code, if the member does not speak english, pick a PCP that speaks their language and a ZIP code as close as possible to their own
                foreach (var m in mems)
                {
                    MemberLob ml = m.RelationMember == null ? memLob.Where(a => a.Member == m).First() : memLob.Where(a => a.Member == m.RelationMember).First();

                    if (m.Language.LanguageName == "English")
                    {
                        // Member speaks english, so any provider will work.  Picking one from the list at random.
                        TrackChange tc = new TrackChange { Member = m, Track = pcpTrack, EffectiveDt = Convert.ToDateTime("2022-01-01"), TrackDataInt = dctZipProv[m.MailingLocation.ZipCd][rnd.Next(0, dctZipProv[m.MailingLocation.ZipCd].Count)].Id };
                        con.TrackChanges.Add(tc);
                    }
                    else
                    {
                        int memberZip = Convert.ToInt32(m.ResidentialLocation.ZipCd.Substring(0, 5));

                        // Member does not speak english, so search through the dctLngProv list of providers that do speak the same language, and pick the provider with the least mathematical distance between
                        // the provider's location Zip Code and the Member's Zip Code.  Since this is all fake data, this is a simple approximation of finding a PCP that is geographically closest to the member.
                        Provider currentProvider = dctLngProv[m.Language.LanguageName][0];
                        foreach (var p in dctLngProv[m.Language.LanguageName])
                        {
                            int currentProvZip = Math.Abs(memberZip - Convert.ToInt32(currentProvider.Location.ZipCd.Substring(0, 5)));
                            int thisProvZip = Math.Abs(memberZip - Convert.ToInt32(p.Location.ZipCd.Substring(0, 5)));

                            if (currentProvZip < thisProvZip)
                                currentProvider = p;
                        }
                        TrackChange tc = new TrackChange { Member = m, Track = pcpTrack, EffectiveDt = Convert.ToDateTime("2022-01-01"), TrackDataInt = currentProvider.Id };
                        con.TrackChanges.Add(tc);
                    }
                }

                await con.SaveChangesAsync();

                // Process the TrackChange records to materialize them into actual MemberTrack records.
                await ChangeProcessor.ProcessTrackChangesAsync();
            }
        }
    }
}
