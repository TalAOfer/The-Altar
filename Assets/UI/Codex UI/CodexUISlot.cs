using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexUISlot : MonoBehaviour
{
    [SerializeField] private SpriteMask _mask;
    [SerializeField] private SpriteRenderer _placeHolderSR;

    public void SetPlaceholderSprite(Sprite sprite)
    {
        _placeHolderSR.sprite = sprite;
    }

    public void SetSortingOrder(int i)
    {
        _mask.frontSortingOrder = i * 7 + 6;
    }
}
