namespace Domain.Common.Base;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;

    public static bool TryParse(string value, out TId id)
    {
        var guid = Guid.Parse(value);
        id = (TId)Activator.CreateInstance(typeof(TId), guid)!;
        return true;
    }
}
