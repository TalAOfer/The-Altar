using UnityEngine;
using UnityEngine.EventSystems;

public class HandCollisionDetector : MonoBehaviour
{
    [SerializeField] private EventRegistry events;
    [SerializeField] private HandManager hand;
    private bool isIn;
    public Collider2D Collider {  get; private set; }
    public LayerMask layerMask; // Set this in the inspector to the layer your colliders are on

    void Update()
    {
        CheckCursorCollider();
    }

    private void CheckCursorCollider()
    {
        // Convert the mouse position to a Ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, layerMask);

        if (!isIn && hit.collider != null)
        {
            isIn = true;
            events.OnCursorEnterHand.Raise(this, null);
        }
        else if (isIn &&  hit.collider == null)
        {
            isIn = false;
            events.OnCursorExitHand.Raise(this, null);
        }
    }
}
