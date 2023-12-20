namespace Sharecode.Backend.Domain.Base.Primitive;

public abstract class ListResponse<TQuery, TResponse>
{
    public TQuery Query { get; set; }
    public IReadOnlyCollection<TResponse> Entities { get; set; } = [];
    public long TotalCount { get; set; } = 0;

    protected void AddRecords(ICollection<TResponse> responses, bool setCollectionCount = false)
    {
        if(responses is IReadOnlyCollection<TResponse> collection)
            Entities = collection;
        else
        {
            List<TResponse> res = [..responses];
            Entities = res;
        }

        if (setCollectionCount)
            TotalCount = Entities.Count;
    }

    public ListResponse<TQuery, TResponse> SetQuery(TQuery query)
    {
        Query = query;
        return this;
    }
    
    public ListResponse<TQuery, TResponse> SetCount(long count)
    {
        TotalCount = count;
        return this;
    }
}