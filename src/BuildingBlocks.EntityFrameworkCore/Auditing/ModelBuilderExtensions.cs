using System.Linq.Expressions;
using BuildingBlocks.Auditing;
using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BuildingBlocks.EntityFrameworkCore.Auditing;

/// <summary>
/// EF 模型构建扩展，统一应用领域事件忽略与软删除查询过滤等约定。
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// 应用平台统一模型约定。
    /// </summary>
    public static ModelBuilder ApplyPlatformConventions(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsOwned() || entityType.IsKeyless)
            {
                continue;
            }

            ConfigureDomainEventIgnore(modelBuilder, entityType);
            ConfigureSoftDelete(entityType);
        }

        return modelBuilder;
    }

    private static void ConfigureDomainEventIgnore(ModelBuilder modelBuilder, IMutableEntityType entityType)
    {
        if (!typeof(Entity).IsAssignableFrom(entityType.ClrType))
        {
            return;
        }

        // 领域事件仅用于内存处理，不参与数据库映射。
        modelBuilder.Entity(entityType.ClrType).Ignore(nameof(Entity.DomainEvents));
    }

    private static void ConfigureSoftDelete(IMutableEntityType entityType)
    {
        if (!typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
        {
            return;
        }

        var parameter = Expression.Parameter(entityType.ClrType, "entity");
        var isDeletedProperty = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            [typeof(bool)],
            parameter,
            Expression.Constant(nameof(ISoftDelete.IsDeleted)));
        var compareExpression = Expression.Equal(isDeletedProperty, Expression.Constant(false));
        // 对软删除实体自动追加 IsDeleted = false 的全局过滤条件。
        entityType.SetQueryFilter(Expression.Lambda(compareExpression, parameter));
    }
}
