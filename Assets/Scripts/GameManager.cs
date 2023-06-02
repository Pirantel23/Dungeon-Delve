using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float health;
    public Weapon weapon1;
    public Weapon weapon2;
    public int coins;
    private GameManager instance;
    void Start()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void Load()
    {
        var weaponHandler = FindObjectOfType<WeaponHandler>();
        weaponHandler.weapon1.weapon = weapon1;
        weaponHandler.weapon2.weapon = weapon2;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().ChangeAmount(health);
        Money.SetAmount(coins);
    }

    public void Save()
    {
        var weaponHandler = FindObjectOfType<WeaponHandler>();
        try
        {
            weapon1 = weaponHandler.weapon1.weapon.prefab.GetComponent<Weapon>();
        }
        catch (NullReferenceException)
        {
            weapon1 = null;
        }
        
        try
        {
            weapon2 = weaponHandler.weapon2.weapon.prefab.GetComponent<Weapon>();
        }
        catch (NullReferenceException)
        {
            weapon2 = null;
        }
        
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().GetAmount();
        coins = Money.GetAmount();
    }
}
