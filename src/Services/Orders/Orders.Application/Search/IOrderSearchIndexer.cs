using Orders.Domain.Entities;

namespace Orders.Application.Search;

/// <summary>
/// 订单搜索索引写入器。
/// </summary>
public interface IOrderSearchIndexer
{
    /// <summary>
    /// 将订单写入搜索索引。
    /// </summary>
    /// <param name="order">订单实体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步任务。</returns>
    Task IndexAsync(Order order, CancellationToken cancellationToken = default);
}
