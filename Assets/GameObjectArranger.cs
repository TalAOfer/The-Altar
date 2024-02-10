using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectArranger : MonoBehaviour
{
    [SerializeField] private List<Transform> transforms = new();
    [SerializeField] private float yOffset;


    [Button]
    public void Arrange()
    {
        float totalHeight = (transforms.Count - 1) * yOffset;
        float startOffset = totalHeight / 2;

        for (int i = 0; i < transforms.Count; i++)
        {
            float yPos = i * -yOffset + startOffset;
            transforms[i].localPosition = new Vector3(0, yPos, 0);
        }
    } 
}
