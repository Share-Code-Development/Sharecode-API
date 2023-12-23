namespace Sharecode.Backend.Domain.Base.Primitive;

public abstract class ListQuery
{
    protected ListQuery()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        OrderBy = DefaultOrderBy() ?? string.Empty;
    }

    public string? SearchQuery { get; set; } = string.Empty;
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public string? OrderBy { get; set; }
    public string? Order { get; set; } = "ASC";
    public bool IncludeDeleted { get; set; } = false;

    public abstract string? DefaultOrderBy();
}