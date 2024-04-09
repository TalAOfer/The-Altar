using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FallingDamage : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float gravityAmount;
    
    private Rigidbody2D rb;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void Initialize(int damage)
    {
        bool isDamage = damage >= 0;
        int absDamage = Mathf.Abs(damage);
        string sign = isDamage ? "-" : "+";
        text.text = sign + absDamage;
        
        rb.gravityScale = gravityAmount;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        text.DOFade(0, 1.25f).SetEase(Ease.InQuad);
    }
}
