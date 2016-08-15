using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DkpWeb.Data;

namespace DkpWeb.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160815070657_sequence")]
    partial class sequence
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("Relational:Sequence:.billsplit_id_seq", "'billsplit_id_seq', '', '1', '1', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.paymentidentity_id_seq", "'paymentidentity_id_seq', '', '1', '1', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.paymentmethod_id_seq", "'paymentmethod_id_seq', '', '1', '1', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.person_id_seq", "'person_id_seq', '', '1', '1', '', '', 'Int64', 'False'");

            modelBuilder.Entity("DkpWeb.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("DkpWeb.Models.BillSplit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasDefaultValueSql("nextval('billsplit_id_seq')");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");

                    b.ToTable("BillSplit");
                });

            modelBuilder.Entity("DkpWeb.Models.PaymentIdentity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("nextval('paymentidentity_id_seq')");

                    b.Property<int>("PaymentMethId")
                        .HasColumnName("PaymentMethID");

                    b.Property<int>("PersonId")
                        .HasColumnName("PersonID");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");

                    b.HasIndex("PaymentMethId");

                    b.HasIndex("PersonId");

                    b.ToTable("PaymentIdentity");
                });

            modelBuilder.Entity("DkpWeb.Models.PaymentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("nextval('paymentmethod_id_seq')");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nchar(50)");

                    b.Property<string>("PayLinkFormat");

                    b.Property<string>("RequestMoneyLinkFormat");

                    b.HasKey("Id");

                    b.ToTable("PaymentMethod");
                });

            modelBuilder.Entity("DkpWeb.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasDefaultValueSql("nextval('person_id_seq')");

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("DkpWeb.Models.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnName("ID");

                    b.Property<int>("Amount");

                    b.Property<int?>("BillId")
                        .HasColumnName("BillID");

                    b.Property<DateTime>("Created");

                    b.Property<int>("CreditorId")
                        .HasColumnName("CreditorID");

                    b.Property<int>("DebtorId")
                        .HasColumnName("DebtorID");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");

                    b.HasIndex("BillId");

                    b.HasIndex("CreditorId");

                    b.HasIndex("DebtorId");

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("DkpWeb.Models.PaymentIdentity", b =>
                {
                    b.HasOne("DkpWeb.Models.PaymentMethod", "PaymentMeth")
                        .WithMany("PaymentIdentity")
                        .HasForeignKey("PaymentMethId")
                        .HasConstraintName("FK_PaymentIdentity_PaymentMethod");

                    b.HasOne("DkpWeb.Models.Person", "Person")
                        .WithMany("PaymentIdentity")
                        .HasForeignKey("PersonId")
                        .HasConstraintName("FK_PaymentIdentity_Person");
                });

            modelBuilder.Entity("DkpWeb.Models.Transaction", b =>
                {
                    b.HasOne("DkpWeb.Models.BillSplit", "Bill")
                        .WithMany("Transaction")
                        .HasForeignKey("BillId")
                        .HasConstraintName("FK_Transaction_BillSplit")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DkpWeb.Models.Person", "Creditor")
                        .WithMany("TransactionCreditor")
                        .HasForeignKey("CreditorId")
                        .HasConstraintName("FK_Transaction_Person_Creditor");

                    b.HasOne("DkpWeb.Models.Person", "Debtor")
                        .WithMany("TransactionDebtor")
                        .HasForeignKey("DebtorId")
                        .HasConstraintName("FK_Transaction_Person_Debtor");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("DkpWeb.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("DkpWeb.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DkpWeb.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
