using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public Sprite icon;
    public float range;
    public float cooldown;
    public float damage;
    public float timeToDamage;
    public bool splashDamage;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            var weaponHandler = FindObjectOfType<WeaponHandler>();
            if (weaponHandler.changingWeapon) return;
            StartCoroutine(weaponHandler.ChangeWeapon(this));
        }
    }
}
