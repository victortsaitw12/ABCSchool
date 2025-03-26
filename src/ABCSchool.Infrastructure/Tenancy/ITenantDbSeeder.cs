using System;

namespace ABCSchool.Infrastructure.Tenancy;

public interface ITenantDbSeeder
{
    Task InitializeDatabaseAsync(CancellationToken cancellationToken);
}
