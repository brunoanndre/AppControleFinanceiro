using Dima.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.Api.Data.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired(true).HasColumnType("NVARCHAR(80)");
            builder.Property(x => x.Description).IsRequired(false).HasColumnType("NVARCHAR(255)");
            builder.Property(x => x.Price).IsRequired(true).HasColumnType("MONEY");
            builder.Property(x => x.IsActive).IsRequired(true).HasColumnType("BIT");
            builder.Property(x => x.Slug).IsRequired(true).HasColumnType("VARCHAR(80)");
        }
    }
}
