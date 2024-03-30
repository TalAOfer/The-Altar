using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class First_ShowTitle : BaseRoomState
{

    public First_ShowTitle(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject TitleGO = _sm.InstantiatePrefab(Prefabs.Title, Prefabs.Title.transform.position, Quaternion.identity, _sm.transform);
        SpriteRenderer TitleSR = TitleGO.GetComponent<SpriteRenderer>();

        yield return TitleSR.transform.DOLocalMoveY(7.5f, 1).SetEase(Ease.InOutFlash);

        //Fade title
        yield return TitleSR.DOFade(0, 2).SetEase(Ease.InExpo).WaitForCompletion();

        SwitchTo(States.SpawnTreasure());
    }


}
