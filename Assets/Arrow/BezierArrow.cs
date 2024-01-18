using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierArrow : MonoBehaviour
{
    #region Public Fields

    [Tooltip("The prefab of arrow head")]
    public GameObject ArrowHeadPrefab;

    [Tooltip("The prefab of arrow node")]
    public GameObject ArrowNodePrefab;

    [Tooltip("The number of arrow nodes")]
    public int arrowNodeNum;

    [Tooltip("The scale multiplier for arrow nodes")]
    public float scaleFactor = 1f;

    #endregion

    #region Private Fields

    private RectTransform origin;
    private List<RectTransform> arrowNodes = new();
    private List<Vector2> controlPoints = new();
    private readonly List<Vector2> controlPointsFactor = new() { new Vector2(-0.3f, 0.8f), new Vector2(0.1f, 1.4f) };

    #endregion

    private void Awake()
    {
        origin = GetComponent<RectTransform>();

        for (int i = 0; i < arrowNodeNum; i++)
        {
            arrowNodes.Add(Instantiate(ArrowNodePrefab, transform).GetComponent<RectTransform>());
        }

        arrowNodes.Add(Instantiate(ArrowHeadPrefab, transform).GetComponent<RectTransform>());

        arrowNodes.ForEach(a => a.GetComponent<RectTransform>().position = new Vector2(-1000, -1000));

        for (int i = 0; i < 4; i++)
        {
            controlPoints.Add(Vector2.zero);
        }
    }

    private void Update()
    {
        controlPoints[0] = new Vector2(origin.position.x, origin.position.y);
        controlPoints[3] = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        controlPoints[1] = controlPoints[0] + (controlPoints[3] - controlPoints[0]) * controlPointsFactor[0];
        controlPoints[2] = controlPoints[0] + (controlPoints[3] - controlPoints[0]) * controlPointsFactor[1];

        for (int i = 0; i < arrowNodes.Count; i++)
        {
            var t = i / (float)(arrowNodes.Count - 1);

            // Calculate the position of the current node based on the Bezier curve formula
            Vector2 position =
                Mathf.Pow(1 - t, 3) * controlPoints[0] +
                3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1] +
                3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2] +
                Mathf.Pow(t, 3) * controlPoints[3];

            arrowNodes[i].position = position;

            if (i > 0)
            {
                var eulerZ = Vector2.SignedAngle(Vector2.up, arrowNodes[i].position - arrowNodes[i - 1].position);
                // Apply the 45-degree offset only to the last node (the arrowhead)
                if (i == arrowNodes.Count - 1)
                {
                    eulerZ -= 45f; // Compensate for the arrow sprite's tilt
                }

                var euler = new Vector3(0, 0, eulerZ);
                arrowNodes[i].rotation = Quaternion.Euler(euler);
            }

            var scale = scaleFactor * (1f - 0.03f * (arrowNodes.Count - 1 - i));
            arrowNodes[i].localScale = new Vector3(scale, scale, 1f);
        }

        arrowNodes[0].transform.rotation = arrowNodes[1].transform.rotation;
    }
}
