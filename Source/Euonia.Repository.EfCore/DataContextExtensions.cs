using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Repository.EfCore;

public static class DataContextExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetTableName(Type type)
    {
        return GetTableName(type.Name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entityName"></param>
    /// <returns></returns>
    public static string GetTableName(string entityName)
    {
        var words = Regex.Matches(entityName, @"((?:[A-Z])?[a-z0-9]+)").Select(t => t.Value.ToLower(CultureInfo.InvariantCulture));
        return string.Join("_", words);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entityName"></param>
    /// <returns></returns>
    public static string GetForeignKey(string entityName)
    {
        return $"{entityName}Id";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    public static void SetSoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            //other automated configurations left out
            if (typeof(ITombstone).IsAssignableFrom(entityType.ClrType))
            {
                entityType.SetSoftDeleteQueryFilter();
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="entity"></param>
    /// <remarks>
    /// https://www.thereformedprogrammer.net/ef-core-in-depth-soft-deleting-data-with-global-query-filters/
    /// </remarks>
    public static void SetSoftDeleteQueryFilter(this IMutableEntityType entity)
    {
        var methodToCall = typeof(DataContextExtensions)
                           .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
                           ?.MakeGenericMethod(entity.ClrType);
        if (methodToCall == null)
        {
            return;
        }

        var filter = methodToCall.Invoke(null, Array.Empty<object>());
        entity.SetQueryFilter((LambdaExpression)filter!);
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>()
        where TEntity : class, ITombstone
    {
        Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
        return filter;
    }
}