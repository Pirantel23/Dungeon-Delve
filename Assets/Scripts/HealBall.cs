using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBall : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        col.gameObject.GetComponent<Health>().Heal(40);
        Destroy(gameObject);
    }
}
