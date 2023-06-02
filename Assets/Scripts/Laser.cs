using System;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float damage;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
