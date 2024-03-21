using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectEventHandler : MonoBehaviour
{
    private Card _card;
    private CardEffectHandler _handler;
    //private CardEffectBlueprint _effect;

    private void Awake()
    {
        _card = GetComponentInParent<Card>();
        _handler = GetComponent<CardEffectHandler>();
    }

    public void OnDamageEffect(Component sender, object data)
    {
        AmountEventData damageData = data as AmountEventData;

        //Retaliate
        if (_card == damageData.Reciever && !_card.IsDead)
        {
            _handler.TriggerEffects(TriggerType.Retaliate);
        }

        //This card has taken damage
        else if (_card == damageData.Reciever)
        {
            _handler.TriggerEffects(TriggerType.SelfDamage);
        }


    }
}
