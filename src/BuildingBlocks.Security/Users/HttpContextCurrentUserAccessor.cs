using System.Security.Claims;
using BuildingBlocks.Auditing;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace BuildingBlocks.Security.Users;

public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public Guid? UserId
    {
        get
        {
            var principal = httpContextAccessor.HttpContext?.User;
            var rawUserId = principal?.FindFirstValue(OpenIddictConstants.Claims.Subject)
                ?? principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(rawUserId, out var userId) ? userId : null;
        }
    }

    public string? UserName => httpContextAccessor.HttpContext?.User?.Identity?.Name
        ?? httpContextAccessor.HttpContext?.User?.FindFirstValue(OpenIddictConstants.Claims.Name);
}
