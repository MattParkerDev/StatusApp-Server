namespace Domain.Common.Base;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;
}
