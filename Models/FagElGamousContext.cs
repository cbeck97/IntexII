using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace BYUFagElGamous1_5.Models
{
    public partial class FagElGamousContext : DbContext
    {

        public FagElGamousContext()
        {
        }

        public FagElGamousContext(DbContextOptions<FagElGamousContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AgeCode> AgeCode { get; set; }
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<BurialAdultChild> BurialAdultChild { get; set; }
        public virtual DbSet<BurialWrapping> BurialWrapping { get; set; }
        public virtual DbSet<CarbonDated> CarbonDated { get; set; }
        public virtual DbSet<Cluster> Cluster { get; set; }
        public virtual DbSet<GenderCode> GenderCode { get; set; }
        public virtual DbSet<HairColor> HairColor { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Measurements> Measurements { get; set; }
        public virtual DbSet<Mummy> Mummy { get; set; }
        public virtual DbSet<MummyImage> MummyImage { get; set; }
        public virtual DbSet<Notes> Notes { get; set; }
        public virtual DbSet<Page> Page { get; set; }
        public virtual DbSet<Sample> Sample { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=rds-practice.cqtyvkzk3ywt.us-east-1.rds.amazonaws.com,1433;Database=FagElGamous;User Id=admin;Password=group1_5;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgeCode>(entity =>
            {
                entity.HasKey(e => e.AgeId)
                    .HasName("PK__AgeCode__875454C24BD2919B");

                entity.Property(e => e.AgeId)
                    .HasColumnName("AgeID")
                    .HasMaxLength(5);

                entity.Property(e => e.AgeRange).HasMaxLength(124);

                entity.Property(e => e.AgeType).HasMaxLength(124);
            });

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.BookId)
                    .HasColumnName("BookID")
                    .ValueGeneratedNever();

                entity.Property(e => e.BookName).HasMaxLength(124);
            });

            modelBuilder.Entity<BurialAdultChild>(entity =>
            {
                entity.HasKey(e => e.AdultChildId)
                    .HasName("PK__BurialAd__91225A4824B3CFAA");

                entity.Property(e => e.AdultChildId)
                    .HasColumnName("AdultChildID")
                    .HasMaxLength(5);

                entity.Property(e => e.AdultChildType).HasMaxLength(124);
            });

            modelBuilder.Entity<BurialWrapping>(entity =>
            {
                entity.HasKey(e => e.WrappingId)
                    .HasName("PK__BurialWr__515E324DC0C6DAEE");

                entity.Property(e => e.WrappingId)
                    .HasColumnName("WrappingID")
                    .HasMaxLength(5);

                entity.Property(e => e.BurialWrapping1)
                    .HasColumnName("BurialWrapping")
                    .HasMaxLength(124);
            });

            modelBuilder.Entity<CarbonDated>(entity =>
            {
                entity.Property(e => e.CarbonDatedId)
                    .HasColumnName("CarbonDatedID")
                    .ValueGeneratedNever();

                entity.Property(e => e.C14sample).HasColumnName("C14Sample");

                entity.Property(e => e.Calibrated95CalendarDateAvg).HasColumnName("Calibrated_95_CalendarDateAvg");

                entity.Property(e => e.Calibrated95CalendarDateMax).HasColumnName("Calibrated_95_CalendarDateMax");

                entity.Property(e => e.Calibrated95CalendarDateMin).HasColumnName("Calibrated_95_CalendarDateMin");

                entity.Property(e => e.Calibrated95CalendarDateSpan).HasColumnName("Calibrated_95_CalendarDateSpan");

                entity.Property(e => e.Category).HasMaxLength(124);

                entity.Property(e => e.ConventionalAgeBp)
                    .HasColumnName("ConventionalAgeBP")
                    .HasMaxLength(124);

                entity.Property(e => e.Location).HasMaxLength(124);

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.MummyId).HasColumnName("MummyID");

                entity.Property(e => e.Questions).HasMaxLength(124);
            });

            modelBuilder.Entity<Cluster>(entity =>
            {
                entity.Property(e => e.ClusterId)
                    .HasColumnName("ClusterID")
                    .ValueGeneratedNever();

                entity.Property(e => e.LocationId).HasColumnName("LocationID");
            });

            modelBuilder.Entity<GenderCode>(entity =>
            {
                entity.HasKey(e => e.GenderId)
                    .HasName("PK__GenderCo__4E24E817A52EDF7D");

                entity.Property(e => e.GenderId)
                    .HasColumnName("GenderID")
                    .HasMaxLength(5);

                entity.Property(e => e.GenderType).HasMaxLength(5);
            });

            modelBuilder.Entity<HairColor>(entity =>
            {
                entity.HasKey(e => e.HairId)
                    .HasName("PK__HairColo__29636C0855AF31E9");

                entity.Property(e => e.HairId)
                    .HasColumnName("HairID")
                    .HasMaxLength(5);

                entity.Property(e => e.HairColor1)
                    .HasColumnName("HairColor")
                    .HasMaxLength(124);
            });

            modelBuilder.Entity<Images>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__Images__7516F4EC5A0B70B8");

                entity.Property(e => e.ImageId)
                    .HasColumnName("ImageID")
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.Area).HasMaxLength(124);

                entity.Property(e => e.BurialLocationEw)
                    .HasColumnName("BurialLocationEW")
                    .HasMaxLength(5);

                entity.Property(e => e.BurialLocationNs)
                    .HasColumnName("BurialLocationNS")
                    .HasMaxLength(5);

                entity.Property(e => e.BurialSituation).HasMaxLength(124);

                entity.Property(e => e.HighPairEw).HasColumnName("HighPairEW");

                entity.Property(e => e.HighPairNs).HasColumnName("HighPairNS");

                entity.Property(e => e.LowPairEw).HasColumnName("LowPairEW");

                entity.Property(e => e.LowPairNs).HasColumnName("LowPairNS");

                entity.Property(e => e.Subplot).HasMaxLength(5);
            });

            modelBuilder.Entity<Measurements>(entity =>
            {
                entity.HasKey(e => e.MeasurementId)
                    .HasName("PK__Measurem__85599F98DF9CE68E");

                entity.Property(e => e.MeasurementId)
                    .HasColumnName("MeasurementID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AgeAtDeath).HasMaxLength(55);

                entity.Property(e => e.AgeMethod).HasMaxLength(55);

                entity.Property(e => e.BurialIcon).HasMaxLength(25);

                entity.Property(e => e.BurialIcon2).HasMaxLength(25);

                entity.Property(e => e.Byusample).HasColumnName("BYUSample");

                entity.Property(e => e.DescriptionOfTaken).HasMaxLength(124);

                entity.Property(e => e.GeFunctionTotal).HasColumnName("GE_FunctionTotal");

                entity.Property(e => e.GenderBodyCol).HasMaxLength(5);

                entity.Property(e => e.GenderGe)
                    .HasColumnName("GenderGE")
                    .HasMaxLength(5);

                entity.Property(e => e.MedialIpramus).HasColumnName("MedialIPRamus");

                entity.Property(e => e.MetopicSuture).HasColumnName("Metopic Suture");

                entity.Property(e => e.MonthSkull).HasMaxLength(124);

                entity.Property(e => e.OsteologyUnknownComment).HasMaxLength(124);

                entity.Property(e => e.PoroticHyperostosis).HasMaxLength(124);

                entity.Property(e => e.PoroticHyperostosisLocation).HasMaxLength(124);

                entity.Property(e => e.PreservationIndex).HasMaxLength(10);

                entity.Property(e => e.Preveservation).HasMaxLength(124);

                entity.Property(e => e.RackAndShelf).HasMaxLength(124);

                entity.Property(e => e.Sample).HasMaxLength(124);

                entity.Property(e => e.Sex).HasMaxLength(5);

                entity.Property(e => e.SexMethod).HasMaxLength(124);

                entity.Property(e => e.SexSkull2018).HasMaxLength(5);

                entity.Property(e => e.Tmjosteoarthritis).HasColumnName("TMJOsteoarthritis");

                entity.Property(e => e.ToothAttrition).HasMaxLength(5);

                entity.Property(e => e.ToothEruption).HasMaxLength(30);
            });

            modelBuilder.Entity<Mummy>(entity =>
            {
                entity.HasIndex(e => e.MummyId)
                    .HasName("PK/FK");

                entity.Property(e => e.MummyId).HasColumnName("MummyID");

                entity.Property(e => e.AdultChild).HasMaxLength(5);

                entity.Property(e => e.AgeRange).HasMaxLength(5);

                entity.Property(e => e.Artifacts).HasMaxLength(124);

                entity.Property(e => e.BurialAgeAtDeath).HasMaxLength(124);

                entity.Property(e => e.BurialAgeMethod).HasMaxLength(124);

                entity.Property(e => e.BurialGenderMethod).HasMaxLength(124);

                entity.Property(e => e.BurialPreservation).HasMaxLength(124);

                entity.Property(e => e.BurialWrapping).HasMaxLength(5);

                entity.Property(e => e.ClusterId).HasColumnName("ClusterID");

                entity.Property(e => e.FaceBundle).HasMaxLength(5);

                entity.Property(e => e.Gender).HasMaxLength(5);

                entity.Property(e => e.HairColor).HasMaxLength(5);

                entity.Property(e => e.HeadDirection).HasMaxLength(5);

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.MeasurementId).HasColumnName("MeasurementID");

                entity.Property(e => e.SampleId).HasColumnName("SampleID");
            });

            modelBuilder.Entity<MummyImage>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => new { e.MummyId, e.ImageId })
                    .HasName("CPK");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.MummyId).HasColumnName("MummyID");
            });

            modelBuilder.Entity<Notes>(entity =>
            {
                entity.Property(e => e.NotesId)
                    .HasColumnName("NotesID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Category).HasMaxLength(124);

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.MeasurmentsId).HasColumnName("MeasurmentsID");

                entity.Property(e => e.SampleId).HasColumnName("SampleID");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasKey(e => new { e.MeasurementId, e.BookId })
                    .HasName("PK__Page__E68793BAB6DEF134");

                entity.Property(e => e.MeasurementId).HasColumnName("MeasurementID");

                entity.Property(e => e.BookId).HasColumnName("BookID");

                entity.Property(e => e.DataEntryCheckerInitials).HasMaxLength(124);

                entity.Property(e => e.DataEntryExpertInitials).HasMaxLength(124);
            });

            modelBuilder.Entity<Sample>(entity =>
            {
                entity.Property(e => e.SampleId)
                    .HasColumnName("SampleID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Area).HasMaxLength(124);

                entity.Property(e => e.Initials).HasMaxLength(10);

                entity.Property(e => e.PreviouslySampled).HasMaxLength(5);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
