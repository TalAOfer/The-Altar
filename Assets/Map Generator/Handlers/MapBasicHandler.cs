using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.thealtar
{
    public class MapBasicHandler : AMapHandler
    {
        
        public override void OnEnter(int index, MapMark item)
        {
            Debug.Log($"Item Clicked: {index} {item.uiMapMark.name}");
        }
    }
}
