using Microsoft.EntityFrameworkCore;
using VoltFlow.Service.Core.Entities;

namespace VoltFlow.Service.Infrastructure.Data;

public partial class VoltFlowDbContext : DbContext
{
    public VoltFlowDbContext(DbContextOptions<VoltFlowDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> categories { get; set; }

    public virtual DbSet<Client> clients { get; set; }

    public virtual DbSet<ClientAddress> clientaddresses { get; set; }

    public virtual DbSet<Element> elements { get; set; }

    public virtual DbSet<ElementGroup> elementgroups { get; set; }

    public virtual DbSet<ErrorLog> errorlogs { get; set; }

    public virtual DbSet<EventJob> eventjobs { get; set; }

    public virtual DbSet<EventJobLog> eventjoblogs { get; set; }

    public virtual DbSet<Job> jobs { get; set; }

    public virtual DbSet<Role> roles { get; set; }

    public virtual DbSet<Stock> stocks { get; set; }

    public virtual DbSet<TaskEntity> tasks { get; set; }

    public virtual DbSet<Transaction> transactions { get; set; }

    public virtual DbSet<TransactionLog> transactionlogs { get; set; }

    public virtual DbSet<User> users { get; set; }

    public virtual DbSet<Warehouse> warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("categories_pkey");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("clients_pkey");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);
        });

        modelBuilder.Entity<ClientAddress>(entity =>
        {
            entity.HasKey(e => e.IdAddress).HasName("clientaddress_pkey");

            entity.ToTable("clientaddress");

            entity.Property(e => e.City).HasMaxLength(75);
            entity.Property(e => e.LocationNumber).HasMaxLength(15);
            entity.Property(e => e.NumberStreet).HasMaxLength(15);
            entity.Property(e => e.Street).HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(75);

            entity.HasOne(d => d.Client).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_address_client");
        });

        modelBuilder.Entity<Element>(entity =>
        {
            entity.HasKey(e => e.IdElement).HasName("element_pkey");

            entity.ToTable("element");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.ElementGroup).WithMany(p => p.Elements)
                .HasForeignKey(d => d.ElementGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_element_group");
        });

        modelBuilder.Entity<ElementGroup>(entity =>
        {
            entity.HasKey(e => e.IdElementGroup).HasName("elementgroup_pkey");

            entity.ToTable("elementgroup");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.ElementsGroups)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_elementgroup_category");
        });

        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.IdErrorLog).HasName("errorlog_pkey");

            entity.ToTable("errorlog");

            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Timestamp).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<EventJob>(entity =>
        {
            entity.HasKey(e => e.IdEventJob).HasName("eventjob_pkey");

            entity.ToTable("eventjob");

            entity.Property(e => e.EventDetails).HasMaxLength(255);

            entity.HasOne(d => d.Job).WithMany(p => p.EventJobs)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_eventjob_job");
        });

        modelBuilder.Entity<EventJobLog>(entity =>
        {
            entity.HasKey(e => e.IdEventJobLog).HasName("eventjoblog_pkey");

            entity.ToTable("eventjoblog");

            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Timestamp).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.EventJob).WithMany(p => p.Logs)
                .HasForeignKey(d => d.EventJobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_eventjoblog_eventjob");
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.IdJob).HasName("jobs_pkey");

            entity.Property(e => e.Date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Address).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_jobs_address");

            entity.HasOne(d => d.Client).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_jobs_client");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("roles_pkey");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.IdStock).HasName("stock_pkey");

            entity.ToTable("stock");

            entity.Property(e => e.LastUpdated).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Element).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.ElementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stock_element");

            entity.HasOne(d => d.Warehouses).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_stock_warehouse");
        });

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(e => e.IdTask).HasName("tasks_pkey");

            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasOne(d => d.Job).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tasks_job");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.IdTransaction).HasName("transaction_pkey");

            entity.ToTable("transaction");

            entity.Property(e => e.Date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Job).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("fk_transaction_job");

            entity.HasOne(d => d.Source).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("fk_transaction_source");

            entity.HasOne(d => d.Stock).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.StockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transaction_stock");

            entity.HasOne(d => d.Target).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TargetId)
                .HasConstraintName("fk_transaction_target");
        });

        modelBuilder.Entity<TransactionLog>(entity =>
        {
            entity.HasKey(e => e.IdTransactionLog).HasName("transactionlog_pkey");

            entity.ToTable("transactionlog");

            entity.Property(e => e.Details).HasMaxLength(255);
            entity.Property(e => e.Timestamp).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.EntityTransaction).WithMany(p => p.TransactionLogs)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transactionlog_transaction");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("users_pkey");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_role");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.IdWarehouse).HasName("warehouse_pkey");

            entity.ToTable("warehouse");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
