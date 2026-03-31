using System.Linq.Expressions;
using BuildingBlocks.Auditing;
using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BuildingBlocks.EntityFrameworkCore.Auditing;

public static class ModelBuilderExtensions
{
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
        entityType.SetQueryFilter(Expression.Lambda(compareExpression, parameter));
    }
}
