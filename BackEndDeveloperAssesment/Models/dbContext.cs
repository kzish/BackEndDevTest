using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BackEndDeveloperAssesment.Models
{
    public partial class dbContext : DbContext
    {
        public dbContext()
        {
        }

        public dbContext(DbContextOptions<dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MAccount> MAccount { get; set; }
        public virtual DbSet<MDeposits> MDeposits { get; set; }
        public virtual DbSet<MTransactions> MTransactions { get; set; }
        public virtual DbSet<MWithdrawals> MWithdrawals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("server=localhost;database=AfrosoftTest;User Id=sa;Password=123abc!;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<MAccount>(entity =>
            {
                entity.ToTable("m_account");

                entity.HasIndex(e => e.AccountNumber)
                    .HasName("account_number_unique")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountName)
                    .IsRequired()
                    .HasColumnName("account_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AccountNumber)
                    .IsRequired()
                    .HasColumnName("account_number")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AccountType)
                    .IsRequired()
                    .HasColumnName("account_type")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Balance)
                    .HasColumnName("balance")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasColumnName("currency")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated)
                    .HasColumnName("date_created")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<MDeposits>(entity =>
            {
                entity.ToTable("m_deposits");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("account_id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");
            });

            modelBuilder.Entity<MTransactions>(entity =>
            {
                entity.ToTable("m_transactions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity.Property(e => e.DestinationAccountNumber)
                    .IsRequired()
                    .HasColumnName("destination_account_number")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Reference)
                    .HasColumnName("reference")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.SourceAccountNumber)
                    .IsRequired()
                    .HasColumnName("source_account_number")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MWithdrawals>(entity =>
            {
                entity.ToTable("m_withdrawals");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("account_id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");
            });
        }
    }
}
