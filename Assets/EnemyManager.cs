using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private int xAmount = 0;
    private List<Card> activeEnemies;

    public void GetXAmount(Component sender, object data)
    {

    }

    public void AddEnemyToManager(Card card)
    {
        activeEnemies.Add(card);
    }

    public void OnFinishedXAnimation(Component sender, object data)
    {
        xAmount++;
    }


}
