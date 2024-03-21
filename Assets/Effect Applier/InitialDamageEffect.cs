using System.Collections;
using UnityEngine;

public class InitialDamageEffect : Effect
{
    private readonly CardData _cardData;
    public InitialDamageEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
        _cardData = Locator.CardData;
    }

    private IEnumerator Headbutt(Card target)
    {
        //Readying
        Vector3 targetPos = ParentCard.transform.position;
        targetPos.y -= _cardData.readyingDistance;
        Tools.PlaySound("Card_Attack_Prepare", ParentCard.transform);
        yield return ParentCard.movement.MoveCard(ParentCard.transform, targetPos, Vector3.one, null, _cardData.readyingSpeed, _cardData.readyingCurve);


        Tools.PlaySound("Card_Attack_Woosh", ParentCard.transform);
        Vector2 enemyCardClosestCollPos = target.movement.GetClosestCollPosToOtherCard(ParentCard.transform.position);
        yield return ParentCard.movement.MoveCard(ParentCard.transform, enemyCardClosestCollPos, Vector3.one, null, _cardData.headbuttSpeed, _cardData.headbuttCurve);

        target.TakeDamage(ParentCard.attackPoints.value, target);
        ParentCard.TakeDamage(target.attackPoints.value, ParentCard);

        TriggerOnImpactCosmeticEffects();

        yield return Tools.GetWait(_cardData.impactFreezeDuration);

        ParentCard.visualHandler.SetSortingOrder(ParentCard.index);
        Tools.PlaySound("Card_Attack_Backoff", ParentCard.transform);
        yield return ParentCard.movement.TransformCardUniformlyToHoveredPlaceholder(_cardData.backOffSpeed, _cardData.backoffCurve);
        ParentCard.visualHandler.SetSortingLayer(GameConstants.PLAYER_CARD_LAYER);
    }
    private void TriggerOnImpactCosmeticEffects()
    {
        Tools.PlaySound("Card_Attack_Impact", ParentCard.transform);
        _events.ShakeScreen.Raise(ParentCard, CameraShakeTypes.Classic);
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        yield return Headbutt(target);
    }
}
