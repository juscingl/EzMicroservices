namespace AuthCenter.Api.Authorization;

/// <summary>
/// 权限类型与作用域常量，供服务校验与种子初始化共享。
/// </summary>
public static class PlatformPermissionConstants
{
    public const string PermissionTypeMenu = "menu";
    public const string PermissionTypeScope = "scope";
    public const string PermissionTypePage = "page";
    public const string PermissionTypeAction = "action";
    public const string PermissionTypeButton = "button";

    public static readonly IReadOnlyCollection<string> PermissionTypes =
    [
        PermissionTypeMenu,
        PermissionTypeScope,
        PermissionTypePage,
        PermissionTypeAction,
        PermissionTypeButton
    ];

    public const string ScopeApi = "api";
    public const string ScopePage = "page";
    public const string ScopeMenu = "menu";
    public const string ScopeButton = "button";
    public const string ScopeScope = "scope";
    public const string ScopeAction = "action";

    public static readonly IReadOnlyCollection<string> Scopes =
    [
        ScopeApi,
        ScopePage,
        ScopeMenu,
        ScopeButton,
        ScopeScope,
        ScopeAction
    ];
}
