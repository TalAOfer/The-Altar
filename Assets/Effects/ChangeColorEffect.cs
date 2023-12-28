using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorEffect : Effect
{
    public CardColor changeToThisColor;

    public void Initialize(CardColor changeToThisColor)
    {
        this.changeToThisColor = changeToThisColor;
    }

    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);

        context.InitiatingCard.visualHandler.SetSpritesColor(changeToThisColor);

        yield return new WaitForSeconds(postdelay);
    }
}
