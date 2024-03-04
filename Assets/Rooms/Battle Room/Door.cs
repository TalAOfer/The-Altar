using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private EventRegistry events;
    private FloorManager _floorManager;
    private Room _leadsTo;

    [Title("Gate")]
    public GameObject gateGO;

    [Title("Opening")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color highlight;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D coll;


    //private bool didFinishAnimation;
    private bool didClickDoor;

    public void Initialize(FloorManager floorManager, Room leadsTo)
    {
        _floorManager = floorManager;
        _leadsTo = leadsTo;
    }

    public IEnumerator OpenDoorRoutine()
    {
        Tools.PlaySound("Door_Open", transform);

        yield return gateGO.transform.DOLocalMoveY(2, 0.5f).SetEase(Ease.InExpo).WaitForCompletion();
        gateGO.gameObject.SetActive(false);

        coll.enabled = true;
        events.ShakeScreen.Raise(this, CameraShakeTypes.Classic);

        if (didClickDoor) _floorManager.NextRoom(_leadsTo);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!didClickDoor)
        {
            didClickDoor = true;
            _floorManager.NextRoom(_leadsTo);
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
