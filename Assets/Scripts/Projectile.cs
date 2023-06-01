using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player")) col.gameObject.GetComponent<Health>().TakeDamage(damage);
        if (col.gameObject.CompareTag("Enemy")) col.gameObject.GetComponent<Health>().TakeDamage(damage);
        if (col.gameObject.CompareTag("Boss")) col.gameObject.GetComponent<Boss>().TakeDamage(damage);
        Destroy(gameObject);
    }
}
