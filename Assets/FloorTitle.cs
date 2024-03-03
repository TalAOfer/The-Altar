using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTitle : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> sprites;
    [SerializeField] private RoomBlueprint blueprint;
    private NewFloorManager _floor;

    private void Awake()
    {
        _floor = GetComponentInParent<NewFloorManager>();
        Sequence fadeSequence = DOTween.Sequence();

        foreach (var sprite in sprites)
        {
            fadeSequence.Append(sprite.DOFade(0, 1).SetEase(Ease.InExpo));
        }

        fadeSequence.OnComplete(() => 
        {
            //_floor.NextRoom();
        });
    }


}
