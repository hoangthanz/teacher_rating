using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using teacher_rating.Models.Identity;

namespace teacher_rating.Common;

public class AccountService : IAccountService
{
    UserManager<ApplicationUser> userManager;
    RoleManager<ApplicationRole> roleManager;

    public AccountService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }
    public async Task<IEnumerable<Claim>> GetClaimsByUser(TokenValidatedContext context)
    {
        var authClaims = new List<Claim>();
        string userName = context.Principal.Identities.First().Name;

        ApplicationUser applicationUser = await userManager.FindByNameAsync(userName);
        if (null == applicationUser)
            return authClaims;
        var roleDatas = await userManager.GetRolesAsync(applicationUser);
        foreach (var role in roleDatas)
        {
            var roleData = await roleManager.FindByNameAsync(role);
            if (roleData != null)
            {
                var roleClaims = await roleManager.GetClaimsAsync(roleData);
                foreach (Claim claim in roleClaims)
                {
                    authClaims.Add(claim);
                }
            }             
        }
        return authClaims;
    }
}