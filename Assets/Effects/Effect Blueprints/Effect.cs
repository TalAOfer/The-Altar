using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    protected bool isConditional;
    protected Decision decision;
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
        isConditional = blueprint.isConditional;
        decision = blueprint.decision;
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
        List<Card> validTargetCards = new(targetCards);
        List<Coroutine> applicationCoroutines = new();


        //Remove cards that don't fulfill the conditions of the effect
        foreach (Card targetCard in targetCards)
        {
            if (isConditional && !decision.Decide(targetCard, data.GetOpponent(targetCard)))
            {
                validTargetCards.Remove(targetCard);
            }
        }

        if (validTargetCards.Count > 0)
        {
            parentCard.visualHandler.Animate("Jiggle");
            yield return new WaitForSeconds(1f);
        }

        foreach (var targetCard in validTargetCards)
        {
            //Keep cards from applying support effects on themselves
            if (target is not EffectTarget.InitiatingCard)
            {
                if (parentCard == targetCard) continue;
                if (targetCard.IsDead) continue;
            }

            Coroutine coroutine = StartCoroutine(applier.Apply(targetCard));
            applicationCoroutines.Add(coroutine);
        }


        // Wait for all coroutines to finish
        foreach (var coroutine in applicationCoroutines)
        {
            yield return coroutine;
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
