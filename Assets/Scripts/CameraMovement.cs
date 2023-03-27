using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Update()
    {
        transform.Translate(target.position - transform.position);
    }
}
