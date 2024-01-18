using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class HandCollisionDetector : MonoBehaviour
{
    [SerializeField] private AllEvents events;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CardBoundry"))
        {
            Card card = collision.transform.parent.GetComponent<Card>();
            if (card != null)
            {
                events.OnCardTriggerEnter.Raise(this, card);
            }

            else
            {
                Debug.LogError("GO tagged card boundry but collision isn't a child of card");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CardBoundry"))
        {
            Card card = collision.transform.parent.GetComponent<Card>();
            if (card != null)
            {
                events.OnCardTriggerExit.Raise(this, card);
            }

            else
            {
                Debug.LogError("GO tagged card boundry but collision isn't a child of card");
            }
        }
    }
}
