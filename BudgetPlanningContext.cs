using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanningWebApplication;

public partial class BudgetPlanningContext : DbContext
{
    public BudgetPlanningContext()
    {
    }

    public BudgetPlanningContext(DbContextOptions<BudgetPlanningContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Active> Actives { get; set; }

    public virtual DbSet<Balance> Balances { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<FinancialPlan> FinancialPlans { get; set; }

    public virtual DbSet<MandatoryPayment> MandatoryPayments { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TypeOfTransaction> TypeOfTransactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS01;Database=BudgetPlanning;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Active>(entity =>
        {
            entity.ToTable("active");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Balance>(entity =>
        {
            entity.ToTable("balance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActiveId).HasColumnName("active_id");
            entity.Property(e => e.Sum).HasColumnName("sum");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Active).WithMany(p => p.Balances)
                .HasForeignKey(d => d.ActiveId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_balance_active");

            entity.HasOne(d => d.User).WithMany(p => p.Balances)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_balance_users");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.TypeOfTransactionId).HasColumnName("typeOfTransaction_id");

            entity.HasOne(d => d.TypeOfTransaction).WithMany(p => p.Categories)
                .HasForeignKey(d => d.TypeOfTransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_category_typeOfTransaction");
        });

        modelBuilder.Entity<FinancialPlan>(entity =>
        {
            entity.ToTable("financialPlan");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.EndDate)
                .HasColumnType("date")
                .HasColumnName("endDate");
            entity.Property(e => e.Limit).HasColumnName("limit");
            entity.Property(e => e.MandatoryPaymentId).HasColumnName("mandatoryPayment_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.StartDate)
                .HasColumnType("date")
                .HasColumnName("startDate");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Category).WithMany(p => p.FinancialPlans)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_financialPlan_category");

            entity.HasOne(d => d.MandatoryPayment).WithMany(p => p.FinancialPlans)
                .HasForeignKey(d => d.MandatoryPaymentId)
                .HasConstraintName("FK_financialPlan_mandatoryPayment");

            entity.HasOne(d => d.User).WithMany(p => p.FinancialPlans)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_financialPlan_users");
        });

        modelBuilder.Entity<MandatoryPayment>(entity =>
        {
            entity.ToTable("mandatoryPayment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("comment");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Sum).HasColumnName("sum");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("transactions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActiveId).HasColumnName("active_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("comment");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Sum).HasColumnName("sum");
            entity.Property(e => e.TypeOfTransactionId).HasColumnName("typeOfTransaction_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Active).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ActiveId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_transactions_active");

            entity.HasOne(d => d.Category).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_transactions_category");

            entity.HasOne(d => d.TypeOfTransaction).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TypeOfTransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_transactions_typeOfTransaction");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_transactions_users");
        });

        modelBuilder.Entity<TypeOfTransaction>(entity =>
        {
            entity.ToTable("typeOfTransaction");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("datetime")
                .HasColumnName("dateOfBirth");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstName");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.SecondName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("secondName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
