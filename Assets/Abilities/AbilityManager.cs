using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private AbilityBlueprint blueprint;
    private Ability ability;
    private EventRegistry _events;

    private void Awake()
    {
        ability = new Ability(blueprint);

        _events = Locator.Events;
    }

    public void RaiseClickEvent()
    {
        _events.OnAbilityClicked.Raise(this, ability);
    }

}
