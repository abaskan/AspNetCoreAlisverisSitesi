using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using shoppingApp.Entity;

namespace shoppingApp.DataAccess.Configurations
{
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
           builder.HasKey(c => new { c.CategoryId, c.ProductId });
        }
    }
}