using System.Collections.Generic;
using System.Linq;

public class EffectNode
{
    public List<EffectNode> Children { get; private set; } = new List<EffectNode>();
    public Effect Effect { get; private set; }

    public IEventData EventData { get; private set; }
    public Card PredefinedTarget {  get; private set; }

    public EffectNode(Effect effect, IEventData eventData, Card predefinedTarget = null)
    {
        Effect = effect;
        EventData = eventData;
        PredefinedTarget = predefinedTarget;
    }

    public void SortChildrenBySpawnOrder()
    {
        Children = Children.OrderBy(child => child.Effect.ParentCard.SpawnIndex).ToList();
    }

    public void AddChild(EffectNode child)
    {
        Children.Add(child);
    }

}