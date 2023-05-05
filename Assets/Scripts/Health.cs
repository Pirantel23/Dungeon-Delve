using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float amount;
    private float maxHealth;
    
    private void Start()
    {
        maxHealth = amount;
    }

    public void TakeDamage(float damage)
    {
        amount -= damage;
        if (amount <= 0) Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
