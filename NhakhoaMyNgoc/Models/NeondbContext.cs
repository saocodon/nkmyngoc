using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(Properties.Settings.Default.DatabaseConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customers_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Cid)
                .HasColumnType("character varying")
                .HasColumnName("CID");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.Name)
                .HasDefaultValueSql("'New Customer'::character varying")
                .HasColumnType("character varying");
            entity.Property(e => e.Phone).HasColumnType("character varying");
            entity.Property(e => e.Sex).HasDefaultValueSql("'2'::smallint");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("expenses_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasDefaultValueSql("'New address'::text");
            entity.Property(e => e.Amount).HasDefaultValueSql("'0'::bigint");
            entity.Property(e => e.CertificateId)
                .HasDefaultValueSql("'0'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("CertificateID");
            entity.Property(e => e.Content).HasDefaultValueSql("'New content'::text");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.Input).HasDefaultValue(true);
            entity.Property(e => e.Participant)
                .HasDefaultValueSql("'New participant'::character varying")
                .HasColumnType("character varying");
        });

        modelBuilder.Entity<Idn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("idn_pkey");

            entity.ToTable("IDN");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CertificateId)
                .HasDefaultValueSql("'0'::character varying")
                .HasColumnType("character varying")
                .HasColumnName("CertificateID");
            entity.Property(e => e.Correspondent)
                .HasDefaultValueSql("'New correspondent'::character varying")
                .HasColumnType("character varying");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.Input).HasDefaultValue(true);
        });

        modelBuilder.Entity<Idnitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("idnitems_pkey");

            entity.ToTable("IDNItems");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Demand).HasDefaultValue(1);
            entity.Property(e => e.IdnId).HasColumnName("IdnID");
            entity.Property(e => e.ItemId)
                .HasDefaultValueSql("'0'::bigint")
                .HasColumnName("ItemID");
            entity.Property(e => e.Price).HasDefaultValueSql("'0'::bigint");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Idn).WithMany(p => p.Idnitems)
                .HasForeignKey(d => d.IdnId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("idnitems_idnid_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.Idnitems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("idnitems_itemid_fkey");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("images_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.CustomerId)
                .HasDefaultValueSql("'0'::bigint")
                .HasColumnName("CustomerID");
            entity.Property(e => e.Deleted).HasDefaultValue(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Images)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("images_customerid_fkey");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invoices_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CustomerId)
                .HasDefaultValueSql("'0'::bigint")
                .HasColumnName("CustomerID");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.Remaining).HasDefaultValueSql("'0'::bigint");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoices_customerid_fkey");
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("invoiceitems_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Discount).HasDefaultValue(0L);
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.Price).HasDefaultValue(0L);
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoiceitems_invoiceid_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.InvoiceItems)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invoiceitems_serviceid_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("products_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.Name)
                .HasDefaultValueSql("'New product'::character varying")
                .HasColumnType("character varying");
            entity.Property(e => e.Price).HasDefaultValueSql("'0'::bigint");
            entity.Property(e => e.Quantity).HasDefaultValue(0);
            entity.Property(e => e.Total).HasDefaultValueSql("'0'::bigint");
            entity.Property(e => e.Unit).HasColumnType("character varying");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("services_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Deleted).HasDefaultValue(false);
            entity.Property(e => e.Name)
                .HasDefaultValueSql("'New service'::character varying")
                .HasColumnType("character varying");
            entity.Property(e => e.Price).HasDefaultValueSql("'0'::bigint");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
