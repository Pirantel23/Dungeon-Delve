using System;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject prefab;
    public SpriteRenderer sprite;
    public int id;
    public Sprite icon;
    public float range;
    public float cooldown;
    public float damage;
    public float timeToDamage;
    public bool splashDamage;
    public bool ranged;
    public GameObject projectile;
    public float speed;
    public bool inUI;
    public bool buyable;
    public int cost;

    public GameObject buymenu;
    public GameObject text;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!buyable)
        {
            if (!other.CompareTag("Player") || !Input.GetKey(KeyCode.E) || inUI) return;
            FindObjectOfType<WeaponHandler>().TryChangeWeapon(this);
        }
        else
        {
            if (!other.CompareTag("Player") || !Input.GetKey(KeyCode.E) || inUI) return;
            var money = Money.GetAmount();
            if (money >= cost)
            {
                Money.ChangeValue(-cost);
                FindObjectOfType<WeaponHandler>().TryChangeWeapon(this);
                Destroy(buymenu);
                Destroy(text);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) buymenu.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) buymenu.SetActive(false);
    }
}
