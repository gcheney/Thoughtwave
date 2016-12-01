using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Thoughtwave.Models;

namespace Thoughtwave.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./Thoughtwave.db");
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Article>().ToTable("Articles");
            modelBuilder.Entity<Comment>().ToTable("Comments");

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

            // Comment Configuration
            modelBuilder.Entity<Comment>()
                .HasOne(a => a.Article)
                .WithMany(q => q.Comments);

            modelBuilder.Entity<Comment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Comments);

            modelBuilder.Entity<Comment>()
                .Property(a => a.Content)
                .IsRequired()
                .HasMaxLength(3000);

            modelBuilder.Entity<Comment>()
                .Property(a => a.CreatedOn)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);


            // Article Configuration
            modelBuilder.Entity<Article>()
                .HasOne(q => q.User)
                .WithMany(u => u.Articles);

            modelBuilder.Entity<Article>()
                .Property(q => q.CreatedOn)
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Article>()
                .Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Article>()
                .Property(q => q.Content)
                .IsRequired()
                .HasMaxLength(1000);
        }
    }
}
