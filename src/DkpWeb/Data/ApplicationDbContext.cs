﻿using Austin.DkpLib;
using DkpWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DkpWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder.Properties<Money>().HaveConversion<MoneyConverter>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BillSplit>(entity =>
            {
                entity.Property(e => e.Id)
                      .HasColumnName("ID")
                      .UseIdentityColumn()
                      .HasIdentityOptions(startValue: 2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Ignore(e => e.PrettyName);
                entity.Ignore(e => e.TotalAmount);
            });

            modelBuilder.Entity<PaymentIdentity>(entity =>
            {
                entity.Property(e => e.Id)
                      .UseIdentityColumn()
                      .HasIdentityOptions(startValue: 200);

                entity.Property(e => e.PaymentMethId).HasColumnName("PaymentMethID");

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.PaymentMeth)
                    .WithMany(p => p.PaymentIdentity)
                    .HasForeignKey(d => d.PaymentMethId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PaymentIdentity_PaymentMethod");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PaymentIdentity)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PaymentIdentity_Person");
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.Property(e => e.Id)
                      .UseIdentityColumn()
                      .HasIdentityOptions(startValue: 4);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Ignore(e => e.HasPayLink);
                entity.Ignore(e => e.HasRequestMoneyLink);
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.Id)
                      .HasColumnName("ID")
                      .UseIdentityColumn()
                      .HasIdentityOptions(startValue: 200);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50);

                entity.Ignore(e => e.FullName);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.BillId).HasColumnName("BillID");

                entity.Property(e => e.CreditorId).HasColumnName("CreditorID");

                entity.Property(e => e.DebtorId).HasColumnName("DebtorID");

                entity.Property(e => e.Created).HasConversion<UtcDateTimeConverter>();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.BillId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Transaction_BillSplit");

                entity.HasOne(d => d.Creditor)
                    .WithMany(p => p.TransactionCreditor)
                    .HasForeignKey(d => d.CreditorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Transaction_Person_Creditor");

                entity.HasOne(d => d.Debtor)
                    .WithMany(p => p.TransactionDebtor)
                    .HasForeignKey(d => d.DebtorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Transaction_Person_Debtor");

                entity.Ignore(e => e.PrettyDescription);

            });
        }

        public virtual DbSet<BillSplit> BillSplit { get; set; }
        public virtual DbSet<PaymentIdentity> PaymentIdentity { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethod { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }

        public IQueryable<Person> ActivePeopleOrderedByName
        {
            get
            {
                return Person
                    .Where(p => !p.IsDeleted)
                    .OrderBy(p => p.FirstName)
                    .ThenBy(p => p.LastName);
            }
        }
    }
}
