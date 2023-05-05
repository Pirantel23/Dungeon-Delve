using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
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

    public void TakeDamage(float damage)
    {
        amount -= damage;
        if (amount <= 0) StartCoroutine(Die());
        else animator.SetTrigger(Hurt);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public IEnumerator Die()
    {
        animator.SetBool(Dead, true);
        if (CompareTag("Player")) GetComponent<PlayerController>().enabled = false;
        else if (CompareTag("Enemy")) GetComponent<Enemy>().enabled = false;
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}
