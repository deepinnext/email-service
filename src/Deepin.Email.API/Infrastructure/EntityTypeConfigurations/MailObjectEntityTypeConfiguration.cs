using Deepin.Email.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deepin.Email.API.Infrastructure.EntityTypeConfigurations;

public class MailObjectEntityTypeConfiguration : IEntityTypeConfiguration<MailObject>
{
    public void Configure(EntityTypeBuilder<MailObject> builder)
    {
        builder.ToTable("mail_objects");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
        builder.Property(s => s.From).HasColumnName("from").IsRequired();
        builder.Property(s => s.To).HasColumnName("to").IsRequired();
        builder.Property(s => s.Subject).HasColumnName("subject").IsRequired();
        builder.Property(s => s.Body).HasColumnName("body").IsRequired();
        builder.Property(s => s.CC).HasColumnName("cc");
        builder.Property(s => s.IsBodyHtml).HasColumnName("is_body_html").IsRequired();
    }
}
