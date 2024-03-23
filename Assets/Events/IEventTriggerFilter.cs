public interface IEventTriggerFilter
{
    public bool Decide(Card triggerHolder, IEventData EventData);
}
