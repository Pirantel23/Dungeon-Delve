using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [SerializeField] private GameObject[] mobs;
    public int minAmount;
    public int maxAmount;
    public Vector2 topRightCorner;


    public void Start()
    {
        var player = GameObject.FindWithTag("Player").transform;
        var numMobs = Random.Range(minAmount, maxAmount + 1);
        for (var _ = 0; _ < numMobs; _++)
        {
            var position = transform.position +
                           new Vector3(Random.Range(0, topRightCorner.x), Random.Range(0, topRightCorner.y));
            var mob = Instantiate(mobs[Random.Range(0, mobs.Length)], position, Quaternion.identity);
            if (mob.CompareTag("Boss")) mob.GetComponent<Boss>().target = player;
            else if (mob.CompareTag("Enemy")) mob.GetComponent<Enemy>().target = player;
        }
    }
}