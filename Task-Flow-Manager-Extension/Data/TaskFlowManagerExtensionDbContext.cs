using Microsoft.EntityFrameworkCore;
using Task_Flow_Manager_Extension.Models;

namespace Task_Flow_Manager_Extension.Data;

public partial class TaskFlowManagerExtensionDbContext : DbContext
{
    public TaskFlowManagerExtensionDbContext()
    {
    }

    public TaskFlowManagerExtensionDbContext(DbContextOptions<TaskFlowManagerExtensionDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Clients> Clients { get; set; }

    public virtual DbSet<Invoices> Invoices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql(
            "server=localhost;port=3306;user=root;password=root;database=TaskFlowManagerExtension",
            ServerVersion.Parse("8.0.40-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Clients>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
        });

        modelBuilder.Entity<Invoices>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.ClientId, "FK_Invoice_Client");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
            entity.Property(e => e.Notes).HasColumnType("text");

            entity.HasOne(d => d.Client).WithMany(p => p.invoices)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Invoice_Client");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}