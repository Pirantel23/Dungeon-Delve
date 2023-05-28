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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !Input.GetKey(KeyCode.E) || inUI) return;
        FindObjectOfType<WeaponHandler>().TryChangeWeapon(this);
    }
}
