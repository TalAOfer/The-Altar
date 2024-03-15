using System.Collections.Generic;
using System.Linq;

public class DFSNode
{
    public List<DFSNode> Children { get; private set; } = new List<DFSNode>();
    public Effect Effect { get; private set; }

    public DFSNode(Effect effect)
    {
        Effect = effect;

    }

    public void SortChildrenBySpawnOrder()
    {
        Children = Children.OrderBy(child => child.Effect.ParentCard.SpawnIndex).ToList();
    }

    public void AddChild(DFSNode child)
    {
        Children.Add(child);
    }

}