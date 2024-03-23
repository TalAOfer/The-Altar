using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEventData : IEventData
{
    public Card Emitter {  get; private set; }
    public NormalEventData(Card emitter)
    {
        Emitter = emitter;
    }
}
