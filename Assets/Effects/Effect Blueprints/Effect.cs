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
    protected int amountOfTargets;
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
        amountOfTargets = blueprint.amountOfTargets;
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
                List<Card> randomEnemyCards = data.GetRandomEnemyCards(amountOfTargets, parentCard);
                if (randomEnemyCards != null) targets = randomEnemyCards;
                break;
            case EffectTarget.RandomCardFromHand:
                List<Card> randomPlayerCards = data.GetRandomPlayerCards(amountOfTargets, parentCard);
                if (randomPlayerCards != null) targets = randomPlayerCards;
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
