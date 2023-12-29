using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AllEvents")]
public class AllEvents : ScriptableObject
{

    public GameEvent OnCardDropOnCard;
    public GameEvent OnMapCardDead;
    public GameEvent WaitForActiveChoice;
    
    public GameEvent OnHandCardStartDrag;
    public GameEvent OnHandCardDroppedNowhere;
    public GameEvent OnDraggedCardHoveredOverHandCard;
}