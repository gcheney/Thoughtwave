using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Thoughtwave.Models;

namespace Thoughtwave.Data
{
    public class ThoughtwaveDbContext : IdentityDbContext<User>
    {
        public ThoughtwaveDbContext(DbContextOptions<ThoughtwaveDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./Thoughtwave.db");
        }

        public DbSet<Thought> Thoughts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Thought>().ToTable("Thoughts");
            modelBuilder.Entity<Comment>().ToTable("Comments");

        #region User configuration

            modelBuilder.Entity<User>()
                .Property(u => u.Avatar)
                .IsRequired()
                .HasDefaultValue("/dist/images/generic-user.jpg");

            modelBuilder.Entity<User>()
                .Property(u => u.SignUpDate)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);
                
            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Anonymous");

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("User");

            modelBuilder.Entity<User>()
                .Property(u => u.Bio)
                .IsRequired()
                .HasMaxLength(500)
                .HasDefaultValue("This user hasn't filled out their Bio yet.");

            modelBuilder.Entity<User>()
                .Property(u => u.IsBanned)
                .IsRequired()
                .HasDefaultValue(false);
        #endregion

        #region Thought Configuration
            modelBuilder.Entity<Thought>()
                .HasOne(t => t.Author)
                .WithMany(u => u.Thoughts)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Thought>()
                .Property(t => t.CreatedOn)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Thought>()
                .Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Thought>()
                .Property(t => t.Content)
                .IsRequired()
                .HasMaxLength(10000);

            modelBuilder.Entity<Thought>()
                .Property(t => t.Category)
                .IsRequired()
                .HasDefaultValue(Category.Personal);

            modelBuilder.Entity<Thought>()
                .Property(t => t.DisableComments)
                .IsRequired()
                .HasDefaultValue(false);

        #endregion

        #region Comment Configuration
        
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Thought)
                .WithMany(t => t.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(3000);

            modelBuilder.Entity<Comment>()
                .Property(c => c.CreatedOn)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);
        #endregion
        }
    }
}
