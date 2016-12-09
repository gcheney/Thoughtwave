using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

            // User Configuration
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
                .HasDefaultValue("Anonymous");

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasDefaultValue("User");

            modelBuilder.Entity<User>()
                .Property(u => u.Bio)
                .IsRequired()
                .HasDefaultValue("This user hasn't filled out their Bio yet.");

            // Comment Configuration
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Thought)
                .WithMany(a => a.Comments);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments);

            modelBuilder.Entity<Comment>()
                .Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(3000);

            modelBuilder.Entity<Comment>()
                .Property(c => c.CreatedOn)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);


            // Thought Configuration
            modelBuilder.Entity<Thought>()
                .HasOne(a => a.Author)
                .WithMany(u => u.Thoughts);

            modelBuilder.Entity<Thought>()
                .Property(a => a.CreatedOn)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Thought>()
                .Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Thought>()
                .Property(a => a.Content)
                .IsRequired()
                .HasMaxLength(2000);

            modelBuilder.Entity<Thought>()
                .Property(a => a.Category)
                .IsRequired()
                .HasDefaultValue(Category.Personal);
        }
    }
}
