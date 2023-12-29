namespace Sharecode.Backend.Domain.Base.Primitive;

public class LiveEvent<TLiveEvent>(TLiveEvent @event)
{

    public static LiveEvent<TLiveEvent> Of(TLiveEvent @event)
    {
        return new LiveEvent<TLiveEvent>(@event);
    }
    
    public static LiveEvent<object> Of(object @event)
    {
        return new LiveEvent<object>(@event);
    }
    
    public readonly string EventType = @event?.GetType().Name ?? throw new NullReferenceException($@"event should be never null!");
    public readonly DateTime InitiatedOn = DateTime.UtcNow;
    public TLiveEvent Event { get; init; } = @event;
}