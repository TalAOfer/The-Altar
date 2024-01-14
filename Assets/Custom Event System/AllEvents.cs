using Sirenix.OdinInspector;
using UnityEngine;

//[CreateAssetMenu(menuName ="AllEvents")]
public class AllEvents : ScriptableObject
{
    [Title("Decks")]
    public GameEvent DrawPlayerCardToHand;
    public GameEvent DrawEnemyCardToMapIndex;

    [Title("Active Choice")]
    public GameEvent WaitForPlayerSelection;
    public GameEvent GetAllCardsFromHand;
    public GameEvent SpawnCardToHand;
    public GameEvent GetXAmount;
    public GameEvent GetRevealedEnemyCards;

    [Title("Map")]
    public GameEvent OnNewRoom;

    public GameEvent OnEnemyDeathMarked;
    public GameEvent OnMapCardDied;
    public GameEvent OnMapSlotClicked;
    public GameEvent OnFinishedXAnimation;
    public GameEvent OnPlayerDrewCardMidGame;

    [Title("Card Interaction")]
    public GameEvent OnCardDropOnCard;
    public GameEvent OnHandCardStartDrag;
    public GameEvent OnHandCardDroppedNowhere;
    public GameEvent OnDraggedCardHoveredOverHandCard;
    public GameEvent OnCardClicked;

    [Title("Global Card Events")]
    public GameEvent OnGlobalCardDeath;


    [Title("Global Handling")]
    public GameEvent ShowTooltip;
    public GameEvent HideTooltip;
    public GameEvent AddLogEntry;
    public GameEvent ToggleCurtain;

    public GameEvent SetGameState;
    public GameEvent OnGameStateChange;
}
