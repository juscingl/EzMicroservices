namespace AuthCenter.Api.Models.Authorization;

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
