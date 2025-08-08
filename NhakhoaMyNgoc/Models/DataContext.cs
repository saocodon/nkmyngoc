using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NhakhoaMyNgoc.Utilities;

namespace NhakhoaMyNgoc.Models;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Idn> Idns { get; set; }

    public virtual DbSet<Idnitem> Idnitems { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={Config.full_path}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).UseCollation("NOCASE");
            entity.Property(e => e.Cid).HasColumnName("CID");
            entity.Property(e => e.Name)
                .HasDefaultValue("Chưa rõ")
                .UseCollation("NOCASE");
            entity.Property(e => e.Sex).HasDefaultValue(2);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasDefaultValue("Chưa rõ");
            entity.Property(e => e.CertificateId).HasColumnName("CertificateID");
            entity.Property(e => e.Content).HasDefaultValue("Chưa rõ");
            entity.Property(e => e.Date).HasDefaultValue(new DateTime(1970, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified));
            entity.Property(e => e.Input).HasDefaultValue(1);
            entity.Property(e => e.Participant).HasDefaultValue("Chưa rõ");
        });

        modelBuilder.Entity<Idn>(entity =>
        {
            entity.ToTable("IDN");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CertificateId)
                .HasDefaultValue(-1)
                .HasColumnName("CertificateID");
            entity.Property(e => e.Correspondent).HasDefaultValue("Chưa rõ");
            entity.Property(e => e.Date).HasDefaultValue(new DateTime(1970, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified));
            entity.Property(e => e.Input).HasDefaultValue(1);
        });

        modelBuilder.Entity<Idnitem>(entity =>
        {
            entity.ToTable("IDNItems");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Demand).HasDefaultValue(1);
            entity.Property(e => e.IdnId).HasColumnName("IdnID");
            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Idn).WithMany(p => p.Idnitems)
                .HasForeignKey(d => d.IdnId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Item).WithMany(p => p.Idnitems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Images)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Date).HasDefaultValue(new DateTime(1970, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified));

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Total).HasComputedColumnSql();

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Service).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasDefaultValue("1970-01-01");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasDefaultValueSql("\"Chưa rõ\"");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasDefaultValue("Chưa rõ");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
