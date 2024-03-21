using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectApplier : MonoBehaviour
{
    private EffectNode rootEffectNode;

    private readonly Stack<EffectNode> effectNodeStack = new();

    // Call this to start the DFS from the root
    public IEnumerator InitializeEffectSequence(Effect effect, Card target = null)
    {
        //Create an empty root node
        rootEffectNode = new EffectNode(null);
        EffectNode newNode = new(effect, target);

        //Add first child and execute the root node
        //(so that if an event is fired closely, it will be considered a sibling of the first effect and not parent)

        rootEffectNode.AddChild(newNode);

        yield return ExecuteEffectSequence(rootEffectNode);
    }

    private IEnumerator ExecuteEffectSequence(EffectNode effectNode)
    {
        if (effectNode == null) yield break;

        effectNodeStack.Push(effectNode);

        if (effectNode.Effect != null)
        {
            if (effectNode.PredefinedTarget != null)
            {
                yield return effectNode.Effect.Trigger(new List<Card> { effectNode.PredefinedTarget });
            }

            else
            {
                yield return effectNode.Effect.Trigger();
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
        Effect effect = (Effect)data;

        Debug.Log(effect);

        EffectNode currentNode = effectNodeStack.Count > 0 ? effectNodeStack.Peek() : null;

        if (currentNode != null)
        {
            EffectNode newNode = new(effect);
            currentNode.AddChild(newNode);
        }
        else
        {
            StartCoroutine(InitializeEffectSequence(effect));
        }
    }
}