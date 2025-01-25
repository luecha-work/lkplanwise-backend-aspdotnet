using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Entities
{
    public class PlanWiseDbContext
        : IdentityDbContext<
            Account,
            Roles,
            Guid,
            IdentityUserClaim<Guid>,
            AccountRoles,
            IdentityUserLogin<Guid>,
            IdentityRoleClaim<Guid>,
            IdentityUserToken<Guid>
        >
    {
        public PlanWiseDbContext(DbContextOptions options)
            : base(options) { }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Roles> Role { get; set; }
        public virtual DbSet<AccountRoles> AccountsRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.HasDefaultSchema("pims");
            //modelBuilder.HasPostgresExtension("uuid-ossp");

            #region CustomEntitysIdentity
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("accounts");

                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

                //entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd(); //ValueGeneratedOnAdd
                entity.Property(e => e.Firstname).HasColumnName("firstname").HasMaxLength(255);
                entity.Property(e => e.Lastname).HasColumnName("lastname").HasMaxLength(255);
                entity.Property(e => e.UserName).HasColumnName("username").HasMaxLength(255);
                entity.Property(e => e.Active).HasColumnName("active");
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
                entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
                entity.Property(e => e.PhoneNumber).HasColumnName("phonenumber").HasMaxLength(20);
                entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
                entity.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
                entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(255);
                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(255);
                entity.Ignore(e => e.NormalizedUserName);
                entity.Ignore(e => e.PhoneNumberConfirmed);
                entity.Ignore(e => e.TwoFactorEnabled);
                entity.Ignore(e => e.NormalizedEmail);
                entity.Ignore(e => e.EmailConfirmed);
                entity
                    .Property(e => e.Language)
                    .HasColumnType("character varying")
                    .HasColumnName("language");
                entity
                    .Property(e => e.ProfileImageName)
                    .HasColumnType("character varying")
                    .HasColumnName("profile_image_name");
                entity
                    .Property(e => e.ProfileImageUrl)
                    .HasColumnType("character varying")
                    .HasColumnName("profile_image_url");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.ToTable("roles");

                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

                entity.Property(r => r.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
                entity
                    .Property(e => e.RoleCode)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''")
                    .HasColumnName("role_code");
                entity
                    .Property(e => e.NormalizedName)
                    .HasColumnName("normalized_name")
                    .HasMaxLength(255);
                entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
                entity.Property(e => e.Active).HasColumnName("active");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            });

            modelBuilder.Entity<AccountRoles>(entity =>
            {
                entity.ToTable("accounts_role");

                entity.HasIndex(e => e.RoleId, "IX_Accounts_Roles_role_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity
                    .HasOne(d => d.Role)
                    .WithMany(p => p.AccountRoles)
                    .HasForeignKey(d => d.RoleId);

                entity
                    .HasOne(d => d.Account)
                    .WithMany(p => p.AccountsRole)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("account_claim");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claim");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("account_login");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("account_token");
            #endregion
        }
    }

    public class CMSDevDbContextFactory : IDesignTimeDbContextFactory<PlanWiseDbContext>
    {
        public PlanWiseDbContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PlanWiseDbContext>();
            var conn = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(conn);
            return new PlanWiseDbContext(optionsBuilder.Options);
        }
    }
}
