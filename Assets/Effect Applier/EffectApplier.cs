using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectApplier : MonoBehaviour
{
    public EffectNode RootEffectNode { get; private set; }

    public delegate void EffectsCompletedHandler();
    public event EffectsCompletedHandler OnEffectsCompleted;

    private readonly Stack<EffectNode> effectNodeStack = new();

    // Call this to start the DFS from the root
    public IEnumerator InitializeEffectSequence(EffectNode effectNode)
    {
        //Create an empty root node
        RootEffectNode = new EffectNode(null, null);

        //Add first child and execute the root node
        //(so that if an event is fired closely, it will be considered a sibling of the first effect and not parent)
        RootEffectNode.AddChild(effectNode);

        yield return ExecuteEffectSequence(RootEffectNode);

        OnEffectsCompleted?.Invoke();
        RootEffectNode = null;
    }

    private IEnumerator ExecuteEffectSequence(EffectNode effectNode)
    {
        if (effectNode == null) yield break;
        if (effectNode.Effect != null && effectNode.Effect.ParentCard.DESTROYING) yield break;

        effectNodeStack.Push(effectNode);

        if (effectNode.Effect != null)
        {
            if (effectNode.PredefinedTarget != null)
            {
                yield return effectNode.Effect.Trigger(new List<Card> { effectNode.PredefinedTarget }, effectNode.EventData);
            }

            else
            {
                yield return effectNode.Effect.Trigger(null, effectNode.EventData);
            }
        }

        yield return Tools.GetWaitRealtime(0.25f);

        effectNode.SortChildrenBySpawnOrder();

        // Recursively visit all children
        foreach (var child in effectNode.Children)
        {
            yield return ExecuteEffectSequence(child);
        }

        // After processing all children, pop the node from the stack
        // This step is crucial for backtracking
        effectNodeStack.Pop();
    }


    public void OnEffectTriggered(Component sender, object data)
    {
        EffectNode effectNode = (EffectNode)data;

        if (effectNode.Effect.EffectTrigger.TriggerType is TriggerType.GlobalDeath)
        {
            Debug.Log("Got to applier");
        }

        EffectNode currentNode = effectNodeStack.Count > 0 ? effectNodeStack.Peek() : null;

        if (currentNode != null)
        {
            currentNode.AddChild(effectNode);
        }
        else
        {
            StartCoroutine(InitializeEffectSequence(effectNode));
        }
    }
}