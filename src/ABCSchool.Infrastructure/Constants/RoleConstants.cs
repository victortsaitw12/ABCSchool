using System.Collections.ObjectModel;

namespace ABCSchool.Infrastructure.Constants;

public class RoleConstants
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>([Admin, Basic]);

    public static bool IsDefaultRole(string roleName) => DefaultRoles.Contains(roleName);
}
