using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float amount;
    [SerializeField] private Animator animator;
    [SerializeField] private float despawnTime;
    private float maxHealth;
    private static readonly int Hurt = Animator.StringToHash("hurt");
    private static readonly int Dead = Animator.StringToHash("dead");

    private void Start()
    {
        maxHealth = amount;
    }

    public float GetAmount() => amount;
    
    public void TakeDamage(float damage)
    {
        amount -= damage;
        if (amount <= 0) StartCoroutine(Die());
        else animator.SetTrigger(Hurt);
    }

    public void SetNewMaxHealth(float newValue)
    {
        maxHealth = newValue;
        amount = newValue;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public IEnumerator Die()
    {
        animator.SetBool(Dead, true);
        if (CompareTag("Player"))
        {
            GetComponent<PlayerController>().enabled = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        else if (CompareTag("Enemy"))
        {
            var script = GetComponent<Enemy>();
            Money.ChangeValue(script.moneyDropped);
            script.enabled = false;
        }
        
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }

    public void Heal(float hp)
    {
        amount += hp;
        if (amount > maxHealth) amount = maxHealth;
    }
}
