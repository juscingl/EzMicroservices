namespace Payments.Domain.Entities;

/// <summary>
/// 支付状态枚举。
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// 待处理。
    /// </summary>
    Pending,

    /// <summary>
    /// 成功。
    /// </summary>
    Succeeded,

    /// <summary>
    /// 失败。
    /// </summary>
    Failed
}
