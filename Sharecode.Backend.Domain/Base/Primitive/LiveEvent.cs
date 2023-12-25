namespace Sharecode.Backend.Domain.Base.Primitive;

public abstract class LiveEvent<TLiveEvent>(TLiveEvent @event)
{
    public readonly string EventType = @event?.GetType().Name ?? throw new NullReferenceException($@"event should be never null!");
    public readonly DateTime InitiatedOn = DateTime.UtcNow;
    public TLiveEvent Event { get; init; } = @event;
}