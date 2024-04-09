using Sirenix.OdinInspector;
using UnityEngine;

//[CreateAssetMenu(menuName ="AllEvents")]
public class EventRegistry : ScriptableObject
{
    [Title("Effects")]
    public GameEvent OnDamage;
    public GameEvent OnDeath;
    public GameEvent OnSummon;
    public GameEvent OnHeal;

    public GameEvent OnEffectTriggered;
    public GameEvent OnTurnEnd;
   
    [Title("Interaction")]
    public GameEvent OnCardPointerEnter;
    public GameEvent OnCardPointerExit;
    public GameEvent OnCardBeginDrag;
    public GameEvent OnCardDrag;
    public GameEvent OnCardEndDrag;
    public GameEvent OnCardClicked;
    public GameEvent OnCursorEnterHand;
    public GameEvent OnCursorExitHand;
    public GameEvent OnAbilityClicked;
    public GameEvent OnRoomButtonClicked;
    public GameEvent OnDoorClicked;

    [Title("Battle")]
    public GameEvent EnableBezierArrow;
    public GameEvent DisableBezierArrow;
    public GameEvent OnFinishedHandFill;
    public GameEvent OnEffectApplied;

    [Title("Battle Selection")]
    public GameEvent WaitForPlayerSelection;

    [Title("Room")]
    public GameEvent OnNewRoom;

    [Title("Hand")]
    public GameEvent OnDraggedCardHoveredOverHandCard;

    [Title("Health")]
    public GameEvent UpdateHealth;

    [Title("Global Handling")]
    public GameEvent ShowTooltip;
    public GameEvent HideTooltip;
    public GameEvent AddLogEntry;
    public GameEvent ToggleCurtain;
    public GameEvent ShakeScreen;

    public GameEvent SetGameState;

    public GameEvent LoadScene;
    public GameEvent OnLose;
    public GameEvent EnableMask;
}
