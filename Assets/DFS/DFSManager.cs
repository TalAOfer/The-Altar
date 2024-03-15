using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DFSManager : MonoBehaviour
{
    private DFSNode rootNode;

    private Stack<DFSNode> nodeStack = new Stack<DFSNode>();

    // Call this to start the DFS from the root
    public IEnumerator StartDFS(Effect rootEffect)
    {
        rootNode = new DFSNode(rootEffect);
        yield return (ExecuteDFS(rootNode));
    }

    private IEnumerator ExecuteDFS(DFSNode node)
    {
        if (node == null) yield break;

        // Before executing the current node, push it onto the stack
        nodeStack.Push(node);

        // Execute the action of the current node
        yield return node.Effect.Trigger();

        yield return Tools.GetWaitRealtime(0.25f);
        // After execution, sort children to prepare for child execution
        node.SortChildrenBySpawnOrder();

        // Recursively visit all children
        foreach (var child in node.Children)
        {
            yield return ExecuteDFS(child);
        }

        // After processing all children, pop the node from the stack
        // This step is crucial for backtracking
        nodeStack.Pop();

        // Optionally, handle any additional logic after completing this node and its children
    }

    // Call this when an effect is triggered dynamically to add a new node
    public void OnEffectTriggered(Component sender, object data)
    {
        Effect effect = (Effect) data;
        // Current node is the last one that was pushed onto the stack
        DFSNode currentNode = nodeStack.Count > 0 ? nodeStack.Peek() : null;
        if (currentNode != null)
        {
            DFSNode newNode = new(effect);
            currentNode.AddChild(newNode);
        }
        else
        {
            Debug.Log("No current node. " + effect + "couldn't be added to stack");
            // Handle the case where there's no current node (e.g., add to root or handle as an orphaned effect)
        }
    }
}