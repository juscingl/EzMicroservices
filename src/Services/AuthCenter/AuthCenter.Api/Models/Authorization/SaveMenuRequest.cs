namespace AuthCenter.Api.Models.Authorization;

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
    string? Description);
