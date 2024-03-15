using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectEventHandler : MonoBehaviour
{
    private Card _card;
    private CardEffectHandler _handler;

    private void Awake()
    {
        _card = GetComponentInParent<Card>();
        _handler = GetComponent<CardEffectHandler>();
    }

    public void OnDamageEffect(Component sender, object data)
    {
        Card damagedCard = (Card)sender;
        if (_card == damagedCard && !_card.IsDead)
        {
         //   _handler.TriggerEffects(TriggerType.OnSurvive);
        }
    }
}
