using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sophophile.Models;

namespace Sophophile.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./Sophophile.db");
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Question>().ToTable("Questions");
            modelBuilder.Entity<Answer>().ToTable("Answers");

            // ApplicationUser Configuration
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Avatar)
                .IsRequired()
                .HasDefaultValue("/dist/images/generic-user.jpg");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.SignUpDate)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);
                
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasDefaultValue("Anonymous");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasDefaultValue("User");

            // Answer Configuration
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.User)
                .WithMany(u => u.Answers);

            modelBuilder.Entity<Answer>()
                .Property(a => a.Content)
                .IsRequired()
                .HasMaxLength(3000);

            modelBuilder.Entity<Answer>()
                .Property(a => a.CreatedOn)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);


            // Question Configuration
            modelBuilder.Entity<Question>()
                .HasOne(q => q.User)
                .WithMany(u => u.Questions);

            modelBuilder.Entity<Question>()
                .Property(q => q.CreatedOn)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Question>()
                .Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Question>()
                .Property(q => q.Content)
                .IsRequired()
                .HasMaxLength(1000);
        }
    }
}
