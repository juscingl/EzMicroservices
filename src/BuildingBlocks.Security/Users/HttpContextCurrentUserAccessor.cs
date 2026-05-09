using System.Security.Claims;
using BuildingBlocks.Auditing;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace BuildingBlocks.Security.Users;

/// <summary>
/// 基于 HttpContext 的当前用户访问器实现。
/// </summary>
public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    /// <summary>
    /// 当前用户标识。优先读取 OpenIddict 的 subject 声明，回退到 NameIdentifier。
    /// </summary>
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

    /// <summary>
    /// 当前用户名。优先读取 Identity.Name，回退到 OpenIddict 的 name 声明。
    /// </summary>
    public string? UserName => httpContextAccessor.HttpContext?.User?.Identity?.Name
        ?? httpContextAccessor.HttpContext?.User?.FindFirstValue(OpenIddictConstants.Claims.Name);
}
