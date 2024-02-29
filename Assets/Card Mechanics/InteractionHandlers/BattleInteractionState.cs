using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInteractionState : CardInteractionState
{
    public BattleInteractionState(CardInteractionStateMachine stateMachine, EventRegistry events) : base(stateMachine, events)
    {
    }
}
