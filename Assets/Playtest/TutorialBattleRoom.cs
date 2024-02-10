using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBattleRoom : BattleRoom
{
    [SerializeField] private RoomData roomData;
    [SerializeField] private Image curtain;
    [SerializeField] private GraphicRaycaster canvasRaycaster;
    [SerializeField] private Button canvasInteractionButton;
    private int index = 0;

    [Title("Grid Tutorial")]
    [SerializeField] private SpriteRenderer gridGlow;
    [SerializeField] private GameObject gridText;
    [Title("Hand Tutorial")]
    [SerializeField] private SpriteRenderer handGlow;
    [SerializeField] private GameObject handText;
    [Title("Door Tutorial")]
    [SerializeField] private SpriteRenderer doorGlow;
    [SerializeField] private GameObject doorText;
    [Title("Attack Tutorial")]
    [SerializeField] private GameObject attackText;

    public void SetInteractability(bool enable) => canvasInteractionButton.interactable = enable;
    public void SetRaycastBlock(bool enable) => canvasRaycaster.enabled = enable;
    public override void OnRoomFinishedLerping()
    {
        base.OnRoomFinishedLerping();
        StartCoroutine(GridRoutine());
    }

    public void SwitchRoutines()
    {
        index++;

        switch(index)
        {
            case 1:
                StartCoroutine(HandRoutine());
                break;
            case 2:
                StartCoroutine(DoorRoutine());
                break;
            case 3:
                StartCoroutine(BattleRoutine());
                break;
        }
    }

    private IEnumerator GridRoutine()
    {
        yield return Tools.GetWait(1.5f);

        foreach (MapSlot slot in grid)
        {
            slot.SetSortingLayer("Top");
            slot.SetSortingOrder(-1);
        }

        foreach (Card card in activeEnemies)
        {
            card.visualHandler.SetSortingLayer("Top");
        }

        gridGlow.DOFade(1, 0.5f);
        FadeCurtain(true);

        yield return Tools.GetWait(0.5f);
        gridText.SetActive(true);
        SetInteractability(true);
    }

    private IEnumerator HandRoutine()
    {
        SetInteractability(false);
        gridGlow.DOFade(0, 0.5f);
        gridText.SetActive(false);

        foreach (MapSlot slot in grid)
        {
            slot.SetSortingLayer("Room");
            slot.SetSortingOrder(1);
        }

        foreach (Card card in activeEnemies)
        {
            card.visualHandler.SetSortingLayer(GameConstants.ENEMY_CARD_LAYER);
        }

        foreach (Card card in roomData.PlayerManager.ActiveCards)
        {
            card.visualHandler.SetSortingLayer("Top");
        }

        handGlow.DOFade(1, 0.5f);
        yield return Tools.GetWait(0.5f);
        handText.SetActive(true);
        SetInteractability(true);
    }

    private IEnumerator DoorRoutine()
    {
        SetInteractability(false);
        handGlow.DOFade(0, 0.5f);
        handText.SetActive(false);

        door.gateGO.GetComponent<SpriteRenderer>().sortingLayerName = ("Top");

        foreach (Card card in roomData.PlayerManager.ActiveCards)
        {
            card.visualHandler.SetSortingLayer(GameConstants.PLAYER_CARD_LAYER);
        }

        doorGlow.DOFade(1, 0.5f);
        yield return Tools.GetWait(0.5f);
        doorText.SetActive(true);
        SetInteractability(true);
    }

    private IEnumerator BattleRoutine()
    {
        canvasRaycaster.enabled = false;
        doorText.SetActive(false);
        door.gateGO.GetComponent<SpriteRenderer>().sortingLayerName = ("Room");
        doorGlow.DOFade(0, 0.5f);
        yield return Tools.GetWait(0.5f);


        attackText.SetActive(true);

        //Deal with player interactability
        for (int i = 0; i < roomData.PlayerManager.ActiveCards.Count; i++)
        {
            Card currentCard = roomData.PlayerManager.ActiveCards[i];
            if (i != 2)
            {
                currentCard.interactionHandler.SetInteractability(false);
            } 
            
            else
            {
                currentCard.visualHandler.SetSortingLayer("Top");
            }
        }

        //Deal with enemy interactability
        Card interactableEnemy = activeEnemies[0];
        interactableEnemy.visualHandler.SetSortingLayer("Top");
        activeEnemies[1].interactionHandler.SetInteractability(false);
    }

    public void FadeCurtain(bool fadeIn)
    {
        float fade = fadeIn ? 0.7f : 0f;
        curtain.DOFade(fade, 0.5f);
    }


}
