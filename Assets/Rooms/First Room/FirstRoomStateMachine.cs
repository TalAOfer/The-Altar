using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRoomStateMachine : RoomStateMachine
{
    [SerializeField] private SpriteRenderer _title;
    public SpriteRenderer Title { get { return _title; } }


    [SerializeField] private Vector3 _titleFinalPosition = Vector3.zero;
    public Vector3 TitleFinalPosition { get { return _titleFinalPosition; } } 
    public override void InitializeStateMachine()
    {
        SwitchState(new First_ShowTitle(this));
    }
}
