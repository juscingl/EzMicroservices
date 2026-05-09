namespace Orders.Application.Search;

/// <summary>
/// 订单搜索读取器。
/// </summary>
public interface IOrderSearchReader
{
    /// <summary>
    /// 按条件查询订单搜索结果。
    /// </summary>
    /// <param name="keyword">搜索关键字。</param>
    /// <param name="customerId">客户标识。</param>
    /// <param name="size">返回数量上限。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>订单搜索结果集合。</returns>
    Task<IReadOnlyCollection<OrderSearchResult>> SearchAsync(
        string? keyword,
        Guid? customerId,
        int size = 20,
        CancellationToken cancellationToken = default);
}
