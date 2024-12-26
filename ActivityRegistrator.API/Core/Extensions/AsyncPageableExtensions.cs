using Azure;
using Azure.Data.Tables;

namespace ActivityRegistrator.API.Core.Extensions;
public static class AsyncPageableExtensions
{
    /// <summary>
    /// Returns <see cref="List{T}"/> of an type <see cref="ITableEntity"/>
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public static async Task<List<Entity>> ToListAsync<Entity>(this AsyncPageable<Entity> query) where Entity : class, ITableEntity
    {
        List<Entity> result = new();

        await foreach (Entity item in query)
        {
            result.Add(item);
        }

        return result;
    }
}
