using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public AllEvents events;
    public RoomData data;

    public Card parentCard;
    public EffectApplier applier;
    public EffectApplicationType effectApplicationType;
    public EffectTarget target;
    protected float predelay;
    protected float postdelay;
    public void BaseInitialize(EffectApplier applier, Card parentCard, EffectBlueprint blueprint)
    {
        this.applier = applier;
        this.parentCard = parentCard;

        effectApplicationType = blueprint.applicationType;
        predelay = blueprint.predelay;
        postdelay = blueprint.postdelay;
        events = blueprint.events;
        data = blueprint.data;
        target = blueprint.target;
    }

    public IEnumerator Trigger()
    {
        yield return new WaitForSeconds(predelay);

        yield return Apply();

        yield return new WaitForSeconds(postdelay);
    }

    public virtual IEnumerator Apply()
    {
        List<Card> targetCards = GetTarget();

        foreach (var targetCard in targetCards)
        {
            //Keep cards from applying support effects on themselves
            if (target is not EffectTarget.InitiatingCard)
            {
                if (parentCard == targetCard) continue;
                if (targetCard.IsDead) continue;
            }

            yield return StartCoroutine(applier.Apply(targetCard, data));
        }
    }

    public List<Card> GetTarget()
    {
        List<Card> targets = new();

        switch (target)
        {
            case EffectTarget.InitiatingCard:
                targets.Add(parentCard);
                break;
            case EffectTarget.Oppnent:
                targets.Add(data.GetOpponent(parentCard));
                break;
            case EffectTarget.AllPlayerCards:
                targets = data.GetAllActivePlayerCards();
                break;
            case EffectTarget.AllEnemyCards:
                targets = data.GetAllActiveEnemies();
                break;
            case EffectTarget.AllCardsOnMap:
                targets = data.GetAllActiveEnemiesOnMap();
                break;
            case EffectTarget.AllCardsInHand:
                targets = data.GetAllCardsInHand();
                break;
            case EffectTarget.RandomCardOnMap:
                Card randomEnemyCard = data.GetRandomEnemyCard(parentCard);
                Debug.Log(randomEnemyCard);
                if (randomEnemyCard != null) targets.Add(randomEnemyCard);
                break;
            case EffectTarget.RandomCardFromHand:
                Card randomPlayerCard = data.GetRandomPlayerCard(parentCard);
                Debug.Log(randomPlayerCard);
                if (randomPlayerCard != null) targets.Add(randomPlayerCard);
                break;
            case EffectTarget.PlayerCardBattling:
                targets.Add(data.BattlingPlayerCard);
                break;
            case EffectTarget.EnemyCardBattling:
                targets.Add(data.BattlingEnemyCard);
                break;
        }

        return targets;
    }
}
