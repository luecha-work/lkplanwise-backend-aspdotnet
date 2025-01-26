using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models;

public partial class PrincipalsdbContext : DbContext
{
    public PrincipalsdbContext()
    {
    }

    public PrincipalsdbContext(DbContextOptions<PrincipalsdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountClaim> AccountClaims { get; set; }

    public virtual DbSet<AccountLogin> AccountLogins { get; set; }

    public virtual DbSet<AccountToken> AccountTokens { get; set; }

    public virtual DbSet<AccountsRole> AccountsRoles { get; set; }

    public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }

    public virtual DbSet<BlockBruteForce> BlockBruteForces { get; set; }

    public virtual DbSet<Counter> Counters { get; set; }

    public virtual DbSet<Hash> Hashes { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobParameter> JobParameters { get; set; }

    public virtual DbSet<JobQueue> JobQueues { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<LogsSystem> LogsSystems { get; set; }

    public virtual DbSet<PlanWiseSession> PlanWiseSessions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<Schema> Schemas { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<Set> Sets { get; set; }

    public virtual DbSet<State> States { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=principalsdb;User ID=sa;Password=P@ssw0rd123;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");

            entity.Property(e => e.Id).ValueGeneratedNever();
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
            entity.Property(e => e.Phonenumber)
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
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<AccountClaim>(entity =>
        {
            entity.ToTable("AccountClaim");

            entity.HasIndex(e => e.UserId, "IX_AccountClaim_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AccountClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AccountLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.ToTable("AccountLogin");

            entity.HasIndex(e => e.UserId, "IX_AccountLogin_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AccountLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AccountToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.ToTable("AccountToken");

            entity.HasOne(d => d.User).WithMany(p => p.AccountTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AccountsRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.ToTable("AccountsRole");

            entity.HasIndex(e => e.RoleId, "IX_Accounts_Roles_role_id");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Role).WithMany(p => p.AccountsRoles).HasForeignKey(d => d.RoleId);

            entity.HasOne(d => d.User).WithMany(p => p.AccountsRoles).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AggregatedCounter>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("PK_HangFire_CounterAggregated");

            entity.ToTable("AggregatedCounter", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<BlockBruteForce>(entity =>
        {
            entity.HasKey(e => e.BlockforceId).HasName("block_bruteforce_pk");

            entity.ToTable("Block_BruteForce");

            entity.Property(e => e.BlockforceId)
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
            entity.Property(e => e.UnlockTime).HasColumnName("unlock_time");
            entity.Property(e => e.UpdateAt).HasColumnName("update_at");
            entity.Property(e => e.UpdateBy).HasColumnName("update_by");
        });

        modelBuilder.Entity<Counter>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Id }).HasName("PK_HangFire_Counter");

            entity.ToTable("Counter", "HangFire");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Hash>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Field }).HasName("PK_HangFire_Hash");

            entity.ToTable("Hash", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Field).HasMaxLength(100);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_HangFire_Job");

            entity.ToTable("Job", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName").HasFilter("([StateName] IS NOT NULL)");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            entity.Property(e => e.StateName).HasMaxLength(20);
        });

        modelBuilder.Entity<JobParameter>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.Name }).HasName("PK_HangFire_JobParameter");

            entity.ToTable("JobParameter", "HangFire");

            entity.Property(e => e.Name).HasMaxLength(40);

            entity.HasOne(d => d.Job).WithMany(p => p.JobParameters)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_HangFire_JobParameter_Job");
        });

        modelBuilder.Entity<JobQueue>(entity =>
        {
            entity.HasKey(e => new { e.Queue, e.Id }).HasName("PK_HangFire_JobQueue");

            entity.ToTable("JobQueue", "HangFire");

            entity.Property(e => e.Queue).HasMaxLength(50);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FetchedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Id }).HasName("PK_HangFire_List");

            entity.ToTable("List", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<LogsSystem>(entity =>
        {
            entity.ToTable("logs_system");

            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
        });

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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([normalized_name] IS NOT NULL)");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NormalizedName)
                .HasMaxLength(255)
                .HasColumnName("normalized_name");
            entity.Property(e => e.RoleCode)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("role_code");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.ToTable("RoleClaim");

            entity.HasIndex(e => e.RoleId, "IX_RoleClaim_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Schema>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("PK_HangFire_Schema");

            entity.ToTable("Schema", "HangFire");

            entity.Property(e => e.Version).ValueGeneratedNever();
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_HangFire_Server");

            entity.ToTable("Server", "HangFire");

            entity.HasIndex(e => e.LastHeartbeat, "IX_HangFire_Server_LastHeartbeat");

            entity.Property(e => e.Id).HasMaxLength(200);
            entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
        });

        modelBuilder.Entity<Set>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Value }).HasName("PK_HangFire_Set");

            entity.ToTable("Set", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.HasIndex(e => new { e.Key, e.Score }, "IX_HangFire_Set_Score");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Value).HasMaxLength(256);
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.Id }).HasName("PK_HangFire_State");

            entity.ToTable("State", "HangFire");

            entity.HasIndex(e => e.CreatedAt, "IX_HangFire_State_CreatedAt");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.Reason).HasMaxLength(100);

            entity.HasOne(d => d.Job).WithMany(p => p.States)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_HangFire_State_Job");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
