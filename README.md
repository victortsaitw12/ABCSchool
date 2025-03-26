dotnet ef migrations add InitialTenantDbGen --project ./src/ABCSchool.Infrastructure/ABCSchool.Infrastructure.csproj --startup-project ./src/ABCSchool.WebApi/ABCSchool.WebApi.csproj -c TenantDbContext

dotnet ef database update --project ./src/ABCSchool.Infrastructure/ABCSchool.Infrastructure.csproj --startup-project ./src/ABCSchool.WebApi/ABCSchool.WebApi.csproj -c TenantDbContext

dotnet ef migrations add InitialApplicationDbGen --project ./src/ABCSchool.Infrastructure/ABCSchool.Infrastructure.csproj --startup-project ./src/ABCSchool.WebApi/ABCSchool.WebApi.csproj -c ApplicationDbContext

dotnet ef database update --project ./src/ABCSchool.Infrastructure/ABCSchool.Infrastructure.csproj --startup-project ./src/ABCSchool.WebApi/ABCSchool.WebApi.csproj -c ApplicationDbContext

dotnet run --project=src/ABCSchool.WebApi/ABCSchool.WebApi.csproj
