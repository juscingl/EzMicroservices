namespace AuthCenter.Api.Models.Authorization;

/// <summary>
/// 菜单节点响应模型。
/// </summary>
public sealed record MenuNodeResponse(
    Guid Id,
    string Code,
    string Name,
    string Route,
    string? Icon,
    string? Component,
    int Sort,
    bool IsVisible,
    bool IsEnabled,
    IReadOnlyCollection<MenuNodeResponse> Children);
