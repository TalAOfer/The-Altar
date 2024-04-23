using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace game.thealtar
{
    public class UIMapMark : MonoBehaviour
    {
        public Image image;
        public Button button;
        public RectTransform rect;

        public void Init(Sprite sprite,Vector2 iconSize)
        {    
            image.sprite = sprite;
            rect.sizeDelta = iconSize;
        }

        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }

        [Button]
        public void QuickSetup()
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
            rect = GetComponent<RectTransform>();
        }
    }
}
