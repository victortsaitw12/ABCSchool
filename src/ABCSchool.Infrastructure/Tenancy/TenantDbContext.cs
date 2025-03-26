using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;

namespace ABCSchool.Infrastructure.Tenancy;

public class TenantDbContext(DbContextOptions<TenantDbContext> options): EFCoreStoreDbContext<ABCSchoolTenantInfo>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ABCSchoolTenantInfo>()
            .ToTable("Tenants", "MultiTenancy");
    }
}
