using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGuardianToSelectedCard : ActiveEffect
{
    public override void SendEvent()
    {
        events.WaitForActiveChoice.Raise(this, this);
    }

    protected override IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        throw new System.NotImplementedException();
    }
}
