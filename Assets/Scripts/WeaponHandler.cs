using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    private PlayerController player;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        Debug.Log(player.weapon);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
