using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Configuration;
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
        public PlanWiseDbContext(DbContextOptions<PlanWiseDbContext> options)
            : base(options) { }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Roles> Role { get; set; }
        public virtual DbSet<AccountRoles> AccountsRole { get; set; }
        public virtual DbSet<BlockBruteForce> BlockBruteforces { get; set; }
        public virtual DbSet<PlanWiseSession> PlanWiseSession { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.ApplyConfiguration(new RoleConfiguration());
            //modelBuilder.ApplyConfiguration(new AccountsConfiguration());
            //modelBuilder.HasDefaultSchema("pims");

            #region CustomEntitysIdentity
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Accounts");

                entity.Property(e => e.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWID()");
                entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
                entity.Property(e => e.Active).HasColumnName("active");
                entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(255)
                    .HasColumnName("created_by");
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");
                entity.Property(e => e.Firstname)
                    .HasMaxLength(255)
                    .HasColumnName("firstname");
                entity.Property(e => e.Language)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("language");
                entity.Property(e => e.Lastname)
                    .HasMaxLength(255)
                    .HasColumnName("lastname");
                entity.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
                entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.NormalizedEmail).HasColumnName("normalized_email");
                entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
                entity.Property(e => e.NormalizedUserName).HasColumnName("normalized_userName");
                entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("phonenumber_confirmed");
                entity.Property(e => e.TwoFactorEnabled).HasColumnName("twofactorEnabled");
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .HasColumnName("phonenumber");
                entity.Property(e => e.ProfileImageName)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("profile_image_name");
                entity.Property(e => e.ProfileImageUrl)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("profile_image_url");
                entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(255)
                    .HasColumnName("updated_by");
                entity.Property(e => e.UserName)
                    .HasMaxLength(255)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.ToTable("Roles");

                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

                entity.Property(r => r.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWID()");
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
                entity.ToTable("AccountsRole");

                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.ToTable("AccountsRole");

                entity.HasIndex(e => e.RoleId, "IX_Accounts_Roles_role_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.Role).WithMany(p => p.AccountRoles).HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.Account).WithMany(p => p.AccountRoles).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AccountClaim");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaim");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AccountLogin");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("AccountToken");
            #endregion

            modelBuilder.Entity<PlanWiseSession>(entity =>
            {
                entity.HasKey(e => e.SessionId).HasName("cms_session_pk");

                entity.ToTable("PlanWiseSession");

                entity.Property(e => e.SessionId)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("session_id");
                entity.Property(e => e.AccountId).HasColumnName("account_id");
                entity.Property(e => e.Browser)
                    .HasMaxLength(50)
                    .HasColumnName("browser");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.ExpirationTime).HasColumnName("expiration_time");
                entity.Property(e => e.IssuedTime).HasColumnName("issued_time");
                entity.Property(e => e.LoginAt).HasColumnName("login_at");
                entity.Property(e => e.LoginIp)
                    .HasMaxLength(50)
                    .HasColumnName("login_ip");
                entity.Property(e => e.Os)
                    .HasMaxLength(50)
                    .HasColumnName("os");
                entity.Property(e => e.Platform)
                    .HasMaxLength(50)
                    .HasColumnName("platform");
                entity.Property(e => e.RefreshTokenAt).HasColumnName("refresh_token_at");
                entity.Property(e => e.SessionStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("A")
                    .HasComment("B (Blocked): Session ยังไม่ได้ใช้งาน\r\nA (Active): Session กำลังใช้งานอยู่\r\nE (Expired): Session หมดอายุแล้ว")
                    .HasColumnName("session_status");
                entity.Property(e => e.Token)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("token");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UpdatedBy).HasColumnName("updated_By");
            });

            modelBuilder.Entity<BlockBruteForce>(entity =>
            {
                entity.HasKey(e => e.BlockForceId).HasName("block_bruteforce_pk");

                entity.ToTable("Block_BruteForce");

                entity.Property(e => e.BlockForceId)
                    .HasDefaultValueSql("(newid())")
                    .HasColumnName("blockforce_id");
                entity.Property(e => e.Count).HasColumnName("count");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.Email)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("email");
                entity.Property(e => e.LockedTime).HasColumnName("locked_time");
                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValue("A")
                    .HasComment("L (Locked): ถูกล็อก\r\nU (UnLock): ไม่ล็อก")
                    .HasColumnName("status");
                entity.Property(e => e.UnLockTime).HasColumnName("unlock_time");
                entity.Property(e => e.UpdatedAt).HasColumnName("update_at");
                entity.Property(e => e.UpdatedBy).HasColumnName("update_by");
            });

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
