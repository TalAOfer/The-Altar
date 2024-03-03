using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private EventRegistry events;
    //private FloorManager floorManager;

    [Title("Gate")]
    public GameObject gateGO;
    [SerializeField] private CustomAnimator gateAnimator;

    [Title("Opening")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highlight;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D coll;

    private bool didFinishAnimation;
    private bool didClickDoor;

    //public void Initialize(FloorManager floorManager)
    //{
    //    this.floorManager = floorManager;
    //}

    public void OpenDoor()
    {
        coll.enabled = true;
        gateAnimator.PlayAnimation("Open");
        Tools.PlaySound("Door_Open", transform);
        float animationDuration = gateAnimator.GetAnimationDuration("Open");
        StartCoroutine(WaitForAnimationEnd(animationDuration));
    }

    public IEnumerator WaitForAnimationEnd(float animationDuration)
    {
        yield return Tools.GetWait(animationDuration + 0.1f);
        gateGO.SetActive(false);
        events.ShakeScreen.Raise(this, CameraShakeTypes.Classic);
        didFinishAnimation = true;
        //if (didClickDoor) floorManager.NextRoom();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!didClickDoor)
        {
            didClickDoor = true;
            //if (didFinishAnimation) floorManager.NextRoom();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sr.color = highlight;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sr.color = defaultColor;
    }
}
