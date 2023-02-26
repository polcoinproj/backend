using Microsoft.EntityFrameworkCore;
using backend.Models;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<VerifyEntry> VerifyEntries { get; set; } = null!;
    public DbSet<School> Schools { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<Achievement> Achievements { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<School>().HasData(
                new School { Id = 1, Name = @"МБОУ ""Политехнический лицей №182"" Кировского района г. Казани", DateCreated = DateTimeOffset.UtcNow }
        );
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var insertedEntries = this.ChangeTracker.Entries()
                               .Where(x => x.State == EntityState.Added)
                               .Select(x => x.Entity);

        foreach (var insertedEntry in insertedEntries)
        {
            var auditableEntity = insertedEntry as Auditable;
            if (auditableEntity != null)
            {
                auditableEntity.DateCreated = DateTimeOffset.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}