using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(m=>m.CategoryId);

            builder.Property(m=>m.Name).IsRequired().HasMaxLength(100);
            
        }
    }
}