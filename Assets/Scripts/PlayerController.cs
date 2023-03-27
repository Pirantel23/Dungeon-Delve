using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform body;
    [SerializeField] private Transform weapon;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float minimumDistance;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrel;
    [SerializeField] private float bulletSpeed;
    private Rigidbody2D rigidbody;
    private Vector2 direction;
    private Vector2 mousePosition;
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void GetInput()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        mousePosition = Input.mousePosition;
    }
    
    private void Update()
    {
        GetInput();
        if (Input.GetMouseButtonDown(0)) Shoot();
    }

    private void FixedUpdate()
    {
        Movement();
        WeaponRotation();
    }

    private void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, barrel.position, weapon.rotation);
        var bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = weapon.right * bulletSpeed; // Change 10f to adjust the bullet speed
    }

    private void Movement()
    {
        var move = direction.normalized * (speed * Time.fixedDeltaTime);
        rigidbody.velocity = move;
    }

    private void WeaponRotation()
    {
        var mouseWorldPosition = camera.ScreenToWorldPoint(mousePosition);
        var position = weapon.position;
        var direction = new Vector2(mouseWorldPosition.x - position.x,
                                    mouseWorldPosition.y - position.y);
        if (direction.magnitude < minimumDistance) return;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        weapon.rotation = Quaternion.Lerp(weapon.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
    }
}
