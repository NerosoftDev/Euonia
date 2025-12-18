using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nerosoft.Euonia.Sample.Domain.Aggregates;

namespace Nerosoft.Euonia.Sample.Persist.Configurations;

[DbContext(typeof(SampleDataContext))]
public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRole>
{
	public void Configure(EntityTypeBuilder<UserRole> builder)
	{
		builder.ToTable("user_role");
		builder.HasKey(x => x.Id);

		builder.HasIndex(x => x.UserId)
			   .HasDatabaseName("user_role_idx_fk");

		builder.HasIndex(x => new { x.UserId, x.Name })
			   .HasDatabaseName("user_role_idx_unique")
			   .IsUnique();

		builder.ShortUniqueId();

		builder.Property(x => x.UserId)
			   .HasColumnName("user_id")
			   .IsRequired();

		builder.Property(x => x.Name)
			   .HasColumnName("name")
			   .HasMaxLength(100)
			   .IsRequired();

		builder.CreatedAtUtc();

		builder.HasOne(x => x.User)
			   .WithMany(x => x.Roles)
			   .HasForeignKey(x => x.UserId)
			   .OnDelete(DeleteBehavior.Cascade);
	}
}
