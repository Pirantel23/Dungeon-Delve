using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float amount;
    [SerializeField] private Animator animator;
    [SerializeField] private float despawnTime;
    [SerializeField] private GameObject deadScreen;
    public float maxHealth;
    private static readonly int Hurt = Animator.StringToHash("hurt");
    private static readonly int Dead = Animator.StringToHash("dead");

    private void Start()
    {
        maxHealth = amount;
    }

    /// <summary>
    /// Return the current amount of health
    /// </summary>
    public float GetAmount() => amount;
    
    /// <summary>
    /// Subtracts damage value from amount
    /// </summary>
    public void TakeDamage(float damage)
    {
        amount -= damage;
        if (amount <= 0) StartCoroutine(Die());
        else animator.SetTrigger(Hurt);
    }
    
    /// <summary>
    /// Change max health and equate amount to it
    /// </summary>
    /// <param name="newValue"></param>
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
            deadScreen.SetActive(true);
            
        }
        else if (CompareTag("Enemy"))
        {
            var script = GetComponent<Enemy>();
            Money.ChangeValue(script.moneyDropped);
            PlayerPrefs.SetInt("HAY", PlayerPrefs.GetInt("HAY") + 1);
            script.enabled = false;
        }
        
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }

    public void ChangeAmount(float amount)
    {
        this.amount = amount;
    }
    
    /// <summary>
    /// Add heal value to amount
    /// </summary>
    public void Heal(float hp)
    {
        amount += hp;
        if (amount > maxHealth) amount = maxHealth;
    }
}
