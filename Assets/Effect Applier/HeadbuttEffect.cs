using System.Collections;
using UnityEngine;

public class HeadbuttEffect : Effect
{
    private readonly CardData _cardData;
    public HeadbuttEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
        _cardData = Locator.CardData;
    }

    private IEnumerator Headbutt(Card target)
    {
        ////Prepare
        //Tools.PlaySound("Card_Attack_Prepare", ParentCard.transform);
        //Vector3 targetPos = ParentCard.transform.position;
        //targetPos.y -= _cardData.readyingDistance;
        //yield return ParentCard.movement.MoveCard
        //    (ParentCard.transform, targetPos, Vector3.one, null, _cardData.readyingSpeed, _cardData.readyingCurve);

        //Pounce
        Tools.PlaySound("Card_Attack_Woosh", ParentCard.transform);
        Vector2 enemyCardClosestCollPos = target.movement.GetClosestCollPosToOtherCard(ParentCard.transform.position);
        yield return ParentCard.movement.MoveCard
            (ParentCard.transform, enemyCardClosestCollPos, Vector3.one, null, _cardData.headbuttSpeed, _cardData.headbuttCurve);

        //Impact
        target.TakeDamage(ParentCard.attackPoints.value, ParentCard);
        ParentCard.TakeDamage(target.attackPoints.value, target);
        Tools.PlaySound("Card_Attack_Impact", ParentCard.transform);
        _events.ShakeScreen.Raise(ParentCard, CameraShakeTypes.Classic);

        //Impact freeze
        yield return Tools.GetWait(_cardData.impactFreezeDuration);

        //Go back
        Tools.PlaySound("Card_Attack_Backoff", ParentCard.transform);
        ParentCard.visualHandler.SetSortingOrder(ParentCard.index);
        yield return ParentCard.movement.TransformCardUniformlyToPlaceholder(_cardData.backOffSpeed, _cardData.backoffCurve);
        ParentCard.visualHandler.SetSortingLayer(GameConstants.PLAYER_CARD_LAYER);
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        yield return Headbutt(target);
    }
}
