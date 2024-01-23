using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BezierArrowManager : MonoBehaviour
{
    [SerializeField] private Transform arrowRootTransform;
    [SerializeField] private float yOffset = 2.9f;
    private Vector3 temp = new();

    private void Awake()
    {
        arrowRootTransform.GetComponent<BezierArrow>().Initialize();
    }
    public void EnableArrow(Component sender, object data)
    {
        Card card = (Card)data;

        temp = card.transform.position;
        temp.y += yOffset;
        arrowRootTransform.transform.position = temp;
        arrowRootTransform.gameObject.SetActive(true);
    }

    public void DisableArrow()
    {
        arrowRootTransform.gameObject.SetActive(false);
    }
}
