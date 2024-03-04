using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class First_ShowTitle : FirstRoomState
{
    public First_ShowTitle(FirstRoomStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        yield return new WaitForSeconds(0.5f);

        yield return _ctx.Title.transform.DOLocalMoveY(_ctx.TitleFinalPosition.y, 1).SetEase(Ease.InOutFlash);

        //Fade title
        yield return _ctx.Title.DOFade(0, 2).SetEase(Ease.InExpo).WaitForCompletion();

        _ctx.SwitchState(new Base_OpenDoors(_ctx));
    }


}
