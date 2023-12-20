namespace Sharecode.Backend.Domain.Base.Primitive;

public abstract class ListQuery
{
    public string SearchQuery { get; set; } = string.Empty;
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public bool IncludeDeleted { get; set; } = false;
}