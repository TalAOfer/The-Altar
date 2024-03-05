using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CodexUICard : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _digit;
    [SerializeField] private Image _symbol;

    [SerializeField] private CardArchetype _archetype;
    public CardArchetype Archetype { get { return _archetype; } }

    private CardBlueprint _mask;
    public CardBlueprint Mask { get { return _mask; } }

    public void ManuallyInitialize(string goName, CardArchetype archetype, Color color, Sprite icon, Sprite digit, Sprite symbol)
    {
        gameObject.name = goName;
        _archetype = archetype;

        _icon.sprite = icon;
        _digit.sprite = digit;
        _symbol.sprite = symbol;

        _icon.color = color;
        _digit.color = color;
        _symbol.color = color;
    }

    public void UpdateCardUI(CardBlueprint newMask)
    {
        _mask = newMask;
        _icon.sprite = _mask.cardSprite;
    }
}
