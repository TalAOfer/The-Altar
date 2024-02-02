using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FallingDamage : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float force;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float gravityAmount;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CustomAnimator customAnimator;

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

        customAnimator.PlayAnimation("Fadeout");
        StartCoroutine(DespawnSelfInTime());
    }

    public IEnumerator DespawnSelfInTime()
    {
        yield return new WaitForSeconds(2.5f);
        Pooler.Despawn(gameObject);
    }
}
