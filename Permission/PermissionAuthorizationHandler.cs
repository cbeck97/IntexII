using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace BYUFagElGamous1_5.Permission
{
    //an authorization handler that verifies if a user has the needed permission to access the resource
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        
        public PermissionAuthorizationHandler()
        {

        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return;
            }
            //Gets all the Claims of the User of Type ‘Permission’ and checks if anyone matches the required permission.
            var permissions = context.User.Claims.Where(x => x.Type == "Permission" &&
                                                             x.Value == requirement.Permission &&
                                                             x.Issuer == "LOCAL AUTHORITY");
            //If there is a match, the user is allowed to access the protected resource.
            //Else, the user will be presented with an Access Denied Page.
            if (permissions.Any())
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}