using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float health;
    public Weapon weapon1;
    public Weapon weapon2;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Load()
    {
        var weaponHandler = FindObjectOfType<WeaponHandler>();
        weaponHandler.weapon1.weapon = weapon1;
        weaponHandler.weapon2.weapon = weapon2;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().ChangeAmount(health);
    }

    public void Save()
    {
        var weaponHandler = FindObjectOfType<WeaponHandler>();
        weapon1 = weaponHandler.weapon1.weapon.prefab.GetComponent<Weapon>();
        weapon2 = weaponHandler.weapon2.weapon.prefab.GetComponent<Weapon>();
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().GetAmount();
    }
}
