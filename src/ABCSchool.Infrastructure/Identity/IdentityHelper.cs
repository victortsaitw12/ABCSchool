using System;
using Microsoft.AspNetCore.Identity;

namespace ABCSchool.Infrastructure.Identity;

internal static class IdentityHelper
{
    internal static List<string> GetIdentityResultErrorDescriptions(IdentityResult result)
    {
        var errorDescriptions = new List<string>();
        foreach (var error in result.Errors)
        {
            errorDescriptions.Add(error.Description);
        }
        return errorDescriptions;
    }
}
