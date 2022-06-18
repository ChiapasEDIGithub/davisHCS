using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace davisHCS.Models
{
    public partial class cediMCSimContext : DbContext
    {
        public cediMCSimContext()
        {
        }

        public cediMCSimContext(DbContextOptions<cediMCSimContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ethnicity> Ethnicities { get; set; }
        public virtual DbSet<IntegrationActivity> IntegrationActivities { get; set; }
        public virtual DbSet<IntegrationMemberTrack> IntegrationMemberTracks { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<MemberLob> MemberLobs { get; set; }
        public virtual DbSet<MemberTrack> MemberTracks { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<Track> Tracks { get; set; }
        public virtual DbSet<TrackChange> TrackChanges { get; set; }
        public virtual DbSet<VwEligMember> VwEligMembers { get; set; }
        public virtual DbSet<VwEligSubscriber> VwEligSubscribers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Program.gCnn);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Ethnicity>(entity =>
            {
                entity.ToTable("Ethnicity");

                entity.Property(e => e.CreationDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EthnicityName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<IntegrationActivity>(entity =>
            {
                entity.ToTable("IntegrationActivity");

                entity.Property(e => e.CreationDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FileSource)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProcessSource)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ScheduleDt).HasColumnType("date");
            });

            modelBuilder.Entity<IntegrationMemberTrack>(entity =>
            {
                entity.ToTable("IntegrationMemberTrack");

                entity.Property(e => e.CreationDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IntegrationActivity)
                    .WithMany(p => p.IntegrationMemberTracks)
                    .HasForeignKey(d => d.IntegrationActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IntegrationMemberTrack_IntegrationActivity");

                entity.HasOne(d => d.MemberTrack)
                    .WithMany(p => p.IntegrationMemberTracks)
                    .HasForeignKey(d => d.MemberTrackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IntegrationMemberTrack_MemberTrack");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("Language");

                entity.Property(e => e.CreationDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LanguageName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LocationType)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.StateCd)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.ZipCd)
                    .HasMaxLength(9)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.ToTable("Member");

                entity.HasIndex(e => e.EthnicityId, "IX_Member_EthnicityId");

                entity.HasIndex(e => e.IntegrationActivityId, "IX_Member_IntegrationActivityId");

                entity.HasIndex(e => e.LanguageId, "IX_Member_LanguageId");

                entity.HasIndex(e => e.MailingLocationId, "IX_Member_MailingLocationId");

                entity.HasIndex(e => e.RelationMemberId, "IX_Member_RelationMemberId");

                entity.HasIndex(e => e.ResidentialLocationId, "IX_Member_ResidentialLocationId");

                entity.Property(e => e.CreationDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GenderCd)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RelationCd)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Ssn)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SSN");

                entity.Property(e => e.UpdateDt).HasColumnType("datetime");

                entity.HasOne(d => d.Ethnicity)
                    .WithMany(p => p.Members)
                    .HasForeignKey(d => d.EthnicityId)
                    .HasConstraintName("FK_Member_Ethnicity");

                entity.HasOne(d => d.IntegrationActivity)
                    .WithMany(p => p.Members)
                    .HasForeignKey(d => d.IntegrationActivityId)
                    .HasConstraintName("FK_Member_IntegrationActivity");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Members)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_Member_Language");

                entity.HasOne(d => d.MailingLocation)
                    .WithMany(p => p.MemberMailingLocations)
                    .HasForeignKey(d => d.MailingLocationId)
                    .HasConstraintName("FK_Member_Location1");

                entity.HasOne(d => d.RelationMember)
                    .WithMany(p => p.InverseRelationMember)
                    .HasForeignKey(d => d.RelationMemberId)
                    .HasConstraintName("FK_Member_Member");

                entity.HasOne(d => d.ResidentialLocation)
                    .WithMany(p => p.MemberResidentialLocations)
                    .HasForeignKey(d => d.ResidentialLocationId)
                    .HasConstraintName("FK_Member_Location");
            });

            modelBuilder.Entity<MemberLob>(entity =>
            {
                entity.ToTable("MemberLOB");

                entity.HasIndex(e => e.MemberId, "IX_MemberLOB_MemberId");

                entity.HasIndex(e => e.OrganizationProviderId, "IX_MemberLOB_OrganizationProviderId");

                entity.Property(e => e.CreationDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LineOfBusiness)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Source)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDt).HasColumnType("datetime");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.MemberLobs)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MemberLOB_Member");

                entity.HasOne(d => d.OrganizationProvider)
                    .WithMany(p => p.MemberLobs)
                    .HasForeignKey(d => d.OrganizationProviderId)
                    .HasConstraintName("FK_MemberLOB_Provider");
            });

            modelBuilder.Entity<MemberTrack>(entity =>
            {
                entity.ToTable("MemberTrack");

                entity.HasIndex(e => e.MemberId, "IX_MemberTrack_MemberLOBId");

                entity.HasIndex(e => e.TrackId, "IX_MemberTrack_TrackId");

                entity.Property(e => e.CreationDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EffectiveDt).HasColumnType("datetime");

                entity.Property(e => e.TrackDataChar)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.MemberTracks)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MemberTrack_Member");

                entity.HasOne(d => d.Track)
                    .WithMany(p => p.MemberTracks)
                    .HasForeignKey(d => d.TrackId)
                    .HasConstraintName("FK_MemberTrack_Track");
            });

            modelBuilder.Entity<Provider>(entity =>
            {
                entity.ToTable("Provider");

                entity.HasIndex(e => e.LanguageId, "IX_Provider_LanguageId");

                entity.HasIndex(e => e.LocationId, "IX_Provider_LocationId");

                entity.Property(e => e.CreationDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Ein)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("EIN");

                entity.Property(e => e.Npi)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("NPI");

                entity.Property(e => e.OrgName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhysicianDateOfBirth).HasColumnType("date");

                entity.Property(e => e.PhysicianFirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhysicianGender)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PhysicianLastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PhysicianMiddleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProviderType)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Ssn)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("SSN");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Providers)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_Provider_Language");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Providers)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("FK_Provider_Location");
            });

            modelBuilder.Entity<Track>(entity =>
            {
                entity.ToTable("Track");

                entity.Property(e => e.CreationDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ValidFrom).HasColumnType("date");

                entity.Property(e => e.ValidTo).HasColumnType("date");
            });

            modelBuilder.Entity<TrackChange>(entity =>
            {
                entity.ToTable("TrackChange");

                entity.HasIndex(e => e.IntegrationActivityId, "IX_TrackChange_IntegrationActivityId");

                entity.HasIndex(e => e.MemberId, "IX_TrackChange_MemberTrackId");

                entity.HasIndex(e => e.TrackId, "IX_TrackChange_TrackId");

                entity.Property(e => e.CreationDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EffectiveDt).HasColumnType("datetime");

                entity.Property(e => e.TrackDataChar)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IntegrationActivity)
                    .WithMany(p => p.TrackChanges)
                    .HasForeignKey(d => d.IntegrationActivityId)
                    .HasConstraintName("FK_TrackChange_IntegrationActivity");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.TrackChanges)
                    .HasForeignKey(d => d.MemberId)
                    .HasConstraintName("FK_TrackChange_Member");

                entity.HasOne(d => d.Track)
                    .WithMany(p => p.TrackChanges)
                    .HasForeignKey(d => d.TrackId)
                    .HasConstraintName("FK_TrackChange_Track");
            });

            modelBuilder.Entity<VwEligMember>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VW_ELIG_MEMBERS");

                entity.Property(e => e.Fromdt)
                    .HasColumnType("datetime")
                    .HasColumnName("FROMDT");

                entity.Property(e => e.MemId).HasColumnName("MEM_ID");

                entity.Property(e => e.SubId).HasColumnName("SUB_ID");

                entity.Property(e => e.Throughdt)
                    .HasColumnType("datetime")
                    .HasColumnName("THROUGHDT");
            });

            modelBuilder.Entity<VwEligSubscriber>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VW_ELIG_SUBSCRIBERS");

                entity.Property(e => e.EligStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ELIG_STATUS");

                entity.Property(e => e.Fromdt)
                    .HasColumnType("datetime")
                    .HasColumnName("FROMDT");

                entity.Property(e => e.MemId).HasColumnName("MEM_ID");

                entity.Property(e => e.Throughdt)
                    .HasColumnType("datetime")
                    .HasColumnName("THROUGHDT");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
