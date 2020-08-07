using Assets.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assets.Models.Configurations
{
    public class AssetConfigurations : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> entityBuilder)
        {
            entityBuilder.Property(x => x.AssetId).IsRequired();
            entityBuilder.Property(x => x.AssetName).IsRequired();
            entityBuilder.Property(x => x.AssetNumber).IsRequired();
            entityBuilder.Property(x => x.DateOfPurchase).IsRequired();
            entityBuilder.Property(x => x.PurchaseCostOfAsset).IsRequired();
            entityBuilder.Property(x => x.MonthsToDepreciation).IsRequired();
            entityBuilder.Property(x => x.ToolType).IsRequired();
            entityBuilder.Property(x => x.AssetStatus).IsRequired().HasConversion<int>();

            entityBuilder.HasMany(x => x.Repairs).WithOne(r => r.Asset).HasForeignKey(x => x.AssetId)
                .OnDelete(DeleteBehavior.SetNull);


            entityBuilder.HasMany(x => x.Repositions).WithOne(r => r.Asset).HasForeignKey(x => x.AssetId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}