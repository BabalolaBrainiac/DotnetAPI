using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
    public class EntityFramewordDataContext : Microsoft.EntityFrameworkCore.DbContext
    {
       private readonly IConfiguration _config;

        public EntityFramewordDataContext(IConfiguration config)
        {
            _config = config;
        }

        public virtual Microsoft.EntityFrameworkCore.DbSet<User> Users {get; set;}

        public virtual Microsoft.EntityFrameworkCore.DbSet<UserSalary> UserSalary {get; set;}

        public virtual Microsoft.EntityFrameworkCore.DbSet<UserJobInfo> UserJobInfo {get; set;}


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured) 
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                optionsBuilder => optionsBuilder.EnableRetryOnFailure());
            }

            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema");

            modelBuilder.Entity<User>()
            .ToTable("Users", "TutorialAppSchema")
            .HasKey(u => u.UserId);

             modelBuilder.Entity<UserJobInfo>()
            .ToTable("UserJobInfo", "TutorialAppSchema")
            .HasKey(u => u.UserId);

              modelBuilder.Entity<UserSalary>()
            .ToTable("UserSalary", "TutorialAppSchema")
            .HasKey(u => u.UserId);
        }
    }
}