using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter2D(Collision2D col)
    {
        try
        {
            col.gameObject.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject);
        }
        catch (NullReferenceException)
        {
            Destroy(gameObject);
        }
    }
}
