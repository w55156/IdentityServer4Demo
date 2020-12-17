using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace clientHybrid.Auths
{
    public class SmithInSomewhereHandle:AuthorizationHandler<SmithInSomewhereRequire>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            SmithInSomewhereRequire requirement)
        {
            //var filtercontext = context.Resource as AuthorizationHandlerContext;
            //if (filtercontext==null)
            //{
            //    context.Fail();
            //    return Task.CompletedTask;
            //}
            var familyName = context.User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?.Value;
            var location = context.User.Claims.FirstOrDefault(c => c.Type == "location")?.Value;
            if (familyName=="Smith"&&location=="SomeWhere"&&context.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
