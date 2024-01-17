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
    }

    public IEnumerator Trigger()
    {
        yield return new WaitForSeconds(predelay);

        yield return Apply();

        yield return new WaitForSeconds(postdelay);
    }

    public virtual IEnumerator Apply()
    {
        List<Card> targets = GetTarget();
        foreach (var target in targets)
        {
            yield return StartCoroutine(applier.Apply(target, data));
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
                targets.Add(data.GetRandomEnemyCard(parentCard));
                break;
            case EffectTarget.RandomCardFromHand:
                targets.Add(data.GetRandomPlayerCard(parentCard));
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
