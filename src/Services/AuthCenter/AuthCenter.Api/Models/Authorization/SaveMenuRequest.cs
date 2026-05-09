namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 新增或更新菜单请求模型。
/// </summary>
public sealed record SaveMenuRequest(
    string Code,
    string Name,
    Guid? ParentId,
    string Route,
    string? Icon,
    string? Component,
    int Sort,
    bool IsVisible,
    bool IsEnabled,
    bool IsExternal,
    string? LinkUrl,
    bool KeepAlive,
    bool HideInBreadcrumb,
    string? Description);
