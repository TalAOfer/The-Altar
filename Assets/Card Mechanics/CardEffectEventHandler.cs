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

    #region Damage Events
    public void OnDamage(Component sender, object data)
    {
        if (data is not AmountEventData damageData)
        {
            Debug.LogError("Got the wrong data type from: " + sender.gameObject.name);
            return;
        }

        if (_card == damageData.Reciever)
        {
            HandleSelfDamage(damageData);
        }
        else
        {
            HandleGlobalDamage(damageData);
        }
    }

    private void HandleSelfDamage(AmountEventData damageData)
    {
        _handler.TriggerEffects(TriggerType.SelfDamage, damageData);

        if (_card.PENDING_DESTRUCTION)
        {
            _handler.TriggerEffects(TriggerType.SelfLethalDamage, damageData);
        }
        else
        {
            _handler.TriggerEffects(TriggerType.Retaliate, damageData);
            _handler.TriggerEffects(TriggerType.SelfNonLethalDamage, damageData);
        }
    }

    private void HandleGlobalDamage(AmountEventData damageData)
    {
        _handler.TriggerEffects(TriggerType.GlobalDamage, damageData);

        if (_card.PENDING_DESTRUCTION)
        {
            _handler.TriggerEffects(TriggerType.GlobalLethalDamage, damageData);
        }
        else
        {
            _handler.TriggerEffects(TriggerType.GlobalNonLethalDamage, damageData);
        }
    }

    #endregion

    #region Heal Events
    public void OnHeal(Component sender, object data)
    {
        if (data is not AmountEventData healData)
        {
            Debug.LogError("Got the wrong data type from: " + sender.gameObject.name);
            return;
        }

        if (_card == healData.Reciever)
        {
            _handler.TriggerEffects(TriggerType.SelfHeal, healData);
        }
        else
        {
            _handler.TriggerEffects(TriggerType.GlobalHeal, healData);
        }
    }
    #endregion

    #region Summon Events

    public void OnSummon(Component sender, object data)
    {
        if (data is not NormalEventData spawnData)
        {
            Debug.LogError("Got the wrong data type from: " + sender.gameObject.name);
            return;
        }

        _handler.TriggerEffects(TriggerType.GlobalSummon, spawnData);
    }

    #endregion

    #region Death Events

    public void OnDeath(Component sender, object data)
    {
        if (data is not NormalEventData deathData)
        {
            Debug.LogError("Got the wrong data type from: " + sender.gameObject.name);
            return;
        }

        _handler.TriggerEffects(TriggerType.GlobalDeath, deathData);
    }

    #endregion
}
