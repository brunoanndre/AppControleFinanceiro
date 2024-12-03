using Dima.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dima.Api.Data.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasColumnType("NVARCHAR(50)").IsRequired();
            builder.Property(x => x.Description).HasColumnType("NVARCHAR(255)").IsRequired(false);
            builder.Property(x => x.UserId).IsRequired(true).HasColumnType("VARCHAR(180)");
        }
    }
}
