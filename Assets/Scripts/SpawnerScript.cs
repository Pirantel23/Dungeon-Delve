using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject[] mobs;
    [SerializeField] private int minAmount;
    [SerializeField] private int maxAmount;
    

    public void Start()
    {
        var player = GameObject.FindWithTag("Player").transform;
        var numMobs = Random.Range(minAmount, maxAmount + 1);
        for (var _ = 0; _ < numMobs; _++)
        {
            var position = transform.position + new Vector3(Random.Range(0,0.5f), Random.Range(0,0.5f));
            var mob = Instantiate(mobs[Random.Range(0, mobs.Length)], position, Quaternion.identity);
            if (mob.CompareTag("Boss"))  mob.GetComponent<Boss>().target = player;
            else if (mob.CompareTag("Enemy")) mob.GetComponent<Enemy>().target = player;
        }
    }
}
