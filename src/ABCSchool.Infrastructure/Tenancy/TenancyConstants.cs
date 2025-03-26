using System;

namespace ABCSchool.Infrastructure.Tenancy;

public class TenancyConstants
{
    public const string TenantIdName = "tenant";
    public const string DefaultPassword = "P@ssw0rd@123";
    public const string FirstName = "Junior";
    public const string LastName = "Matlou";


    public static class Root
    {
        public const string Id = "root";
        public const string Name = "Root";
        public const string Email = "admin.root@abcschool.com";
    }
    
}
