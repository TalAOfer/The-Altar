using System.Collections.Generic;
using UnityEngine;

public class BezierArrow : MonoBehaviour
{
    #region Public Fields

    public bool initializeOnAwake;

    [Tooltip("Enable to use the debug target instead of the mouse position")]
    public bool isDebugging = false;

    [Tooltip("Target used for debugging")]
    public Transform debugTarget;

    [Tooltip("The prefab of arrow head")]
    public GameObject ArrowHeadPrefab;

    [Tooltip("The prefab of arrow node")]
    public GameObject ArrowNodePrefab;

    [Tooltip("The number of arrow nodes")]
    public int arrowNodeNum;

    [Tooltip("The scale multiplier for arrow nodes")]
    public float scaleFactor = 1f;

    [Tooltip("First control point to influence the shape of the Bezier curve")]
    public Transform controlPoint1;

    [Tooltip("Second control point to influence the shape of the Bezier curve")]
    public Transform controlPoint2;

    [SerializeField] private float arrowHeadAngleOffset;

    #endregion

    #region Private Fields

    private Transform origin;
    private List<Transform> arrowNodes = new List<Transform>();
    private List<Vector2> controlPoints = new List<Vector2>();
    
    [Tooltip("Factors to control the shape of the Bezier curve. Negative Y values will make the curve arc upwards.")]
    [SerializeField] // This attribute will expose the field in the Inspector even though it's private.

    #endregion

    private void Awake()
    {
        if (initializeOnAwake) Initialize();
    }

    public void Initialize()
    {
        origin = transform;

        for (int i = 0; i < arrowNodeNum; i++)
        {
            GameObject node = Instantiate(ArrowNodePrefab, transform.position, Quaternion.identity, transform);
            arrowNodes.Add(node.transform);
        }

        GameObject head = Instantiate(ArrowHeadPrefab, transform.position, Quaternion.identity, transform);
        arrowNodes.Add(head.transform);

        // Initially position the nodes offscreen or out of view
        arrowNodes.ForEach(a => a.position = new Vector3(-1000, -1000, 0));

        for (int i = 0; i < 4; i++)
        {
            controlPoints.Add(Vector2.zero);
        }
    }

    private void OnEnable()
    {
        ResetArrow();
    }

    public void ResetArrow()
    {
        // Ensure arrow nodes are initialized
        if (arrowNodes == null || arrowNodes.Count == 0) Initialize();

        // Position the nodes off-screen or in their initial positions
        arrowNodes.ForEach(a => a.position = new Vector3(-1000, -1000, 0));

        // Optionally, reset other components like rotations or scales if needed
        arrowNodes.ForEach(a => a.rotation = Quaternion.identity);
        arrowNodes.ForEach(a => a.localScale = Vector3.one * scaleFactor);
    }

    private void Update()
    {
        // Assuming origin is the position of this GameObject and it's already in world space
        Vector2 originPosition = transform.position;
        Vector2 targetPosition = !isDebugging ? Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)) : debugTarget.transform.position;

        // Use the positions of the control point Transforms directly
        Vector2 controlPoint1Position = controlPoint1.position;
        Vector2 controlPoint2Position = controlPoint2.position;

        // Position each node along the Bezier curve
        for (int i = 0; i < arrowNodes.Count; i++)
        {
            var t = i / (float)(arrowNodes.Count - 1);
            Vector2 position = CalculateCubicBezierPoint(t, originPosition, controlPoint1Position, controlPoint2Position, targetPosition);
            arrowNodes[i].position = new Vector3(position.x, position.y, origin.position.z);

            if (i > 0)
            {
                Vector2 direction = position - (Vector2)arrowNodes[i - 1].position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (i == arrowNodes.Count - 1)
                {
                    angle -= arrowHeadAngleOffset; // Compensate for the arrow sprite's tilt
                }
                arrowNodes[i].rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            var scale = scaleFactor * (1f - 0.03f * (arrowNodes.Count - 1 - i));
            arrowNodes[i].localScale = new Vector3(scale, scale, 1f);
        }
    }

    Vector2 CalculateCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 p = uuu * p0; //first term
        p += 3 * uu * t * p1; //second term
        p += 3 * u * tt * p2; //third term
        p += ttt * p3; //fourth term

        return p;
    }
}