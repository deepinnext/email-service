using Deepin.Email.API.Domain.Entities;
using Deepin.Email.API.Infrastructure.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Deepin.Email.API.Infrastructure;

public class EmailDbContext(DbContextOptions<EmailDbContext> options) : DbContext(options)
{
    public required DbSet<MailObject> MailObjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MailObjectEntityTypeConfiguration());
    }
}
