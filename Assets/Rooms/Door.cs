using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private EventRegistry _events;
    private RoomBlueprint _leadsTo;

    [Title("Gate")]
    public GameObject gateGO;

    [Title("Opening")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highlight;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D coll;


    //private bool didFinishAnimation;
    private bool didClickDoor;

    private void Awake()
    {
        _events = Locator.Events;
    }
    public void Initialize(RoomBlueprint leadsTo)
    {
        _leadsTo = leadsTo;
    }

    public void ToggleDoor(bool enable)
    {
        transform.parent.gameObject.SetActive(enable);
    }

    public IEnumerator OpenDoorRoutine()
    {
        Tools.PlaySound("Door_Open", transform);

        yield return gateGO.transform.DOLocalMoveY(2, 0.5f).SetEase(Ease.InExpo).WaitForCompletion();
        gateGO.SetActive(false);

        coll.enabled = true;
        _events.ShakeScreen.Raise(this, CameraShakeTypes.Classic);

        if (didClickDoor) _events.OnDoorClicked.Raise(this, _leadsTo);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!didClickDoor)
        {
            didClickDoor = true;
            _events.OnDoorClicked.Raise(this, _leadsTo);
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
