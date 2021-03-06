﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelelinkAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TelelinkAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbSet<Owner> Owners { get; set; }

        public DbSet<Model> Models { get; set; }
        public DbSet<OwnerModel> OwnerModels { get; set; }
        public DbSet<PendingRegistration> PendingRegistrations { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add many to many realtionship in the OwnerModel bridge table.
          
            modelBuilder.Entity<OwnerModel>()
                .HasOne(om => om.Owner)
                .WithMany(m => m.OwnerModels)
                .HasForeignKey(om => om.OwnerId);

            modelBuilder.Entity<OwnerModel>()
                .HasOne(om => om.Model)
                .WithMany(o => o.OwnerModels)
                .HasForeignKey(om => om.ModelId);

            // Make Owner & Model name fields unique
            modelBuilder.Entity<Owner>()
                .HasIndex(o => o.Name)
                .IsUnique();

            modelBuilder.Entity<Model>()
                .HasIndex(m => m.Name)
                .IsUnique();

            modelBuilder.Entity<PendingRegistration>()
                .HasIndex(pr => pr.UserName)
                .IsUnique();

            // Add one to one realtioship between IdentityUsers table and Owner table.
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(o => o.Owner)
                .WithOne(u => u.User)
                .HasForeignKey<Owner>(o => o.UserId);

            base.OnModelCreating(modelBuilder); /* Required to sucessfully create the Identity tables, otherwise this error shows:
            "The entity type 'IdentityUserLogin<int>' requires a primary key to be defined. If you intended to use a keyless entity type call 'HasNoKey()'." */

        }
    }
}
