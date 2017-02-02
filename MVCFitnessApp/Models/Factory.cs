using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MVCFitnessApp.Models
{
    public class Factory : DbContext
    {
        public Factory()
        {
            Database.SetInitializer(new ShopInitializer());
        }

        // create database tables
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Coach> Coachs { get; set; }
        public DbSet<FilePath> FilePaths { get; set; }
        public DbSet<FilePathMember> FilePathMembers { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // create many-to-many relation table ActivityCoach (between Coach and Activity)
            modelBuilder.Entity<Activity>()
             .HasMany(c => c.Coachs).WithMany(i => i.Activities)
             .Map(t => t.MapLeftKey("ActivityID")
                 .MapRightKey("CoachID")
                 .ToTable("ActivityCoach"));

            // create many-to-many relation table ActivityMember (between Member and Activity)
            modelBuilder.Entity<Activity>()
             .HasMany(m => m.Members).WithMany(a => a.Activities)
             .Map(t => t.MapLeftKey("ActivityID")
                 .MapRightKey("MemberID")
                 .ToTable("ActivityMember"));
        }
    }

    public class ShopInitializer : DropCreateDatabaseIfModelChanges<Factory>
    {
        protected override void Seed(Factory context)
        {
            // Activity, Coach and Member tables with data
            context.Activities.Add(new Activity() { ActivityID = 1, Name = "Dance", Description = "Dance or die ..", PictureFilename = "dance.jpg" });
            context.Activities.Add(new Activity() { ActivityID = 2, Name = "Zumba", Description = "Zumba or die ..", PictureFilename = "zumba.jpg" });
            context.Activities.Add(new Activity() { ActivityID = 3, Name = "Spinning", Description = "Spin or die ..", PictureFilename = "spinning.jpg" });
            context.SaveChanges();

            context.Members.Add(new Member() { MemberID = 1, LastName = "Hansen", FirstName = "Hans", EnrollmentDate = DateTime.Parse("2015-09-01") });
            context.Members.Add(new Member() { MemberID = 2, LastName = "Jensen", FirstName = "Jens", EnrollmentDate = DateTime.Parse("2015-10-01") });
            context.Members.Add(new Member() { MemberID = 3, LastName = "Petersen", FirstName = "Peter", EnrollmentDate = DateTime.Parse("2015-10-10") });
            context.Members.Add(new Member() { MemberID = 4, LastName = "Hansen", FirstName = "Kim", EnrollmentDate = DateTime.Parse("2015-11-10") });
            context.Members.Add(new Member() { MemberID = 5, LastName = "Christensen", FirstName = "Dorthe", EnrollmentDate = DateTime.Parse("2016-01-01") });
            context.Members.Add(new Member() { MemberID = 6, LastName = "Petersen", FirstName = "Jytte", EnrollmentDate = DateTime.Parse("2015-11-10") });
            context.SaveChanges();

            context.Coachs.Add(new Coach() { CoachID = 1, LastName = "Knudsen", FirstName = "Knud", HireDate = DateTime.Parse("01/09/2014") });
            context.Coachs.Add(new Coach() { CoachID = 2, LastName = "Bentsen", FirstName = "Bente", HireDate = DateTime.Parse("2015-10-01") });
            context.Coachs.Add(new Coach() { CoachID = 3, LastName = "Hansen", FirstName = "Hansine", HireDate = DateTime.Parse("2013-10-10") });
            context.SaveChanges();

            context.FilePaths.Add(new FilePath() { FilePathID = 1, FileName = "avatar2.jpg", FileType = "Photo", CoachID = 1 });
            context.FilePaths.Add(new FilePath() { FilePathID = 2, FileName = "avatar2.jpg", FileType = "Photo", CoachID = 2 });
            context.FilePaths.Add(new FilePath() { FilePathID = 3, FileName = "avatar2.jpg", FileType = "Photo", CoachID = 3 });
            context.SaveChanges();

            context.FilePathMembers.Add(new FilePathMember() { FilePathMemberID = 1, FileName = "avatar.jpg", FileType = "Photo", MemberID = 1 });
            context.FilePathMembers.Add(new FilePathMember() { FilePathMemberID = 2, FileName = "avatar.jpg", FileType = "Photo", MemberID = 2 });
            context.FilePathMembers.Add(new FilePathMember() { FilePathMemberID = 3, FileName = "avatar.jpg", FileType = "Photo", MemberID = 3 });
            context.FilePathMembers.Add(new FilePathMember() { FilePathMemberID = 4, FileName = "avatar.jpg", FileType = "Photo", MemberID = 4 });
            context.FilePathMembers.Add(new FilePathMember() { FilePathMemberID = 5, FileName = "avatar.jpg", FileType = "Photo", MemberID = 5 });
            context.FilePathMembers.Add(new FilePathMember() { FilePathMemberID = 6, FileName = "avatar.jpg", FileType = "Photo", MemberID = 6 });
            context.SaveChanges();
        }
    }
}