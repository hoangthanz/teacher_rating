using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace teacher_rating.Common;

public interface IAccountService
{
    Task<IEnumerable<Claim>> GetClaimsByUser(TokenValidatedContext context);
    string GenerateRefreshToken();
}