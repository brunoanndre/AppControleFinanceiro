﻿using Dima.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.Api.Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Number).IsRequired().HasColumnType("CHAR(8)");
            builder.Property(x => x.ExternalReference).IsRequired(false).HasColumnType("NVARCHAR(60)");
            builder.Property(x => x.Gateway).IsRequired(true).HasColumnType("SMALLINT");
            builder.Property(x => x.CreatedAt).IsRequired(true).HasColumnType("DATETIME2");
            builder.Property(x => x.UpdatedAt).IsRequired(true).HasColumnType("DATETIME2");
            builder.Property(x => x.Status).IsRequired().HasColumnType("SMALLINT");
            builder.Property(x => x.UserId).IsRequired().HasColumnType("VARCHAR(160)");

            builder.HasOne(x => x.Product).WithMany();
            builder.HasOne(x => x.Voucher).WithMany();
        }
    }
}