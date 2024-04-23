using UnityEngine;

namespace game.thealtar
{
    public abstract class AMapHandler : MonoBehaviour 
    {
        public abstract void OnEnter(int index, MapMark item);
    }
}