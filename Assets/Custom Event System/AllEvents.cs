using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AllEvents")]
public class AllEvents : ScriptableObject
{
    [Title("Active Choice")]
    public GameEvent WaitForActiveChoice;
    public GameEvent GetRandomCardFromHand;
    public GameEvent SpawnCardToHand;
    public GameEvent DrawCardToHand;

    [Title("Map")]
    public GameEvent OnMapCardDied;
    public GameEvent OnMapSlotClicked;
    public GameEvent OnFinishedXAnimation;

    [Title("Card Interaction")]
    public GameEvent OnCardDropOnCard;
    public GameEvent OnHandCardStartDrag;
    public GameEvent OnHandCardDroppedNowhere;
    public GameEvent OnDraggedCardHoveredOverHandCard;

    [Title("Global Card Events")]
    public GameEvent OnGlobalCardDeath;
}
