using Microsoft.AspNetCore.Authorization;

// class that will essentially help to evaluate permission
namespace BYUFagElGamous1_5.Permission
{
    internal class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; private set; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}