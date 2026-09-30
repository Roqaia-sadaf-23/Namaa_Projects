using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "dbo", table => table.HasCheckConstraint("CK_Roles_Name_NotEmpty", "((len(ltrim(rtrim([Name])))>(0)))"));
        builder.HasKey(e => e.Id).HasName("PK_Roles");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("tinyint").UseIdentityColumn(1, 1);
        builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(250)").HasMaxLength(250).IsRequired(false);
        builder.HasAlternateKey(e => e.Name).HasName("UQ_Roles_Name");
    }
}
