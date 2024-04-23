using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace game.thealtar
{
    public class MapBuilder : MonoBehaviour
    {
        [Header("Setup")]
        public UIMapMark prefabMark;
        public Sprite lockedIcon; 
        public Sprite unlockedIcon;

        [Header("Config")]
        //config directly alters the properties of AddMark button
        public Vector2 markSize = new Vector2(20,20);
        public MapMarkType type;
        [OnValueChanged("FocusMark")]
        public bool focusOnAddMark;

        [OnCollectionChanged("OnMarkListUpdate", "OnMarkListUpdateAfter")]
        public List<MapMark> marks = new List<MapMark>();

        [SerializeField]
        AMapHandler handler;

        private void Start()
        {
            InitCallbacks();
        }

        private void InitCallbacks()
        {
            for(int i = 0; i < marks.Count; i++)
            {
                var mark = marks[i];
                int index = i; //removing this will mess up memory for callbacks
                mark.uiMapMark.button.onClick.AddListener(() =>
                {
                    mark.onEnter?.Invoke();
                    handler?.OnEnter(index, mark);
                });
            }
            
        }

        [Sirenix.OdinInspector.Button]
        public void AddMark()
        {
            AddMark(type); 
        }

        public void OnMarkListUpdateAfter(CollectionChangeInfo info)
        {
            //do nothing 
        }

        public void OnMarkListUpdate(CollectionChangeInfo info)
        {
            if (info.ChangeType == CollectionChangeType.RemoveIndex || info.ChangeType == CollectionChangeType.RemoveValue)
            { 
                if (Application.isEditor)
                {
                    DestroyImmediate(marks[info.Index].uiMapMark.gameObject);
                }
                else
                {
                    Destroy(marks[info.Index].uiMapMark.gameObject);
                }
            }
        }

        public void FocusMark()
        {
#if UNITY_EDITOR
            if (focusOnAddMark)
            {
                UnityEditor.ActiveEditorTracker.sharedTracker.isLocked = true;
                Debug.Log("Locking Current Inspector");
            }
            else
            {
                UnityEditor.ActiveEditorTracker.sharedTracker.isLocked = false;
            } 
#endif
        }

        public void AddMark(MapMarkType type)
        {
            MapMark mark = new MapMark()
            {
                type = type
            };

            GameObject instance = Instantiate(prefabMark.gameObject);
            instance.transform.SetParent(this.gameObject.transform);
            instance.transform.localPosition = Vector3.zero;


            UIMapMark ui = instance.GetComponent<UIMapMark>();
            ui.Init(
                GetMarkSprite(type),
                markSize
            );
            

            mark.uiMapMark = ui;

            marks.Add(mark);

#if UNITY_EDITOR
            if (focusOnAddMark)
            {
                FocusMark();
                UnityEditor.Selection.activeGameObject = instance;
            }
#endif
        }

        private Sprite GetMarkSprite(MapMarkType type)
        {
            switch(type)
            {
                case MapMarkType.LOCKED:
                    return lockedIcon;
                case MapMarkType.UNLOCKED:
                    return unlockedIcon;
                default: return null;
            }
        }
    }
}