namespace Sharecode.Backend.Utilities.Extensions;

public static class SetExtensions
{
    public static void AddRange<TEntity>(this HashSet<TEntity> set, ICollection<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            set.Add(entity);
        }
    }
}