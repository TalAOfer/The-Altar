using System;
using UnityEngine;
using UnityEngine.Events;

namespace game.thealtar
{
    [Serializable]
    public class MapMark
    {
        public MapMarkType type;
        public UIMapMark uiMapMark;
        public UnityEvent onEnter;
    }
}