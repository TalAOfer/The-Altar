using Sirenix.OdinInspector;
using UnityEngine;

//[CreateAssetMenu(menuName ="AllEvents")]
public class AllEvents : ScriptableObject
{
    [Title("Active Choice")]
    public GameEvent WaitForActiveChoice;
    public GameEvent GetRandomCardFromHand;
    public GameEvent GetAllCardsFromHand;
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

    [Title("On Prebattle")]
    public GameEvent OnPrebattlePlayer;
    public GameEvent OnPrebattleEnemy;
}
