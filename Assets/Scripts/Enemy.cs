using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private bool flying;
    [SerializeField] private float detectionRange;
    [SerializeField] private float minimumRange;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform target;
    [SerializeField] private Animator animator;
    private Vector2 direction;
    private Rigidbody2D _rigidbody;
    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");
    private bool colliding;
    private Vector2 normal;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        FollowTarget();
        Animate();
    }

    private void FixedUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        direction = target.position - transform.position;
        var t = direction.magnitude;
        if (t > detectionRange || t < minimumRange) return;
        direction = direction.normalized;
        var move = direction * (speed * Time.fixedDeltaTime);
        if (colliding)
        {
            var g = new Vector2(normal.y, -normal.x);
            move = g * (speed * Time.fixedDeltaTime);
        }
        _rigidbody.velocity = move;
    }

    private void Animate()
    {
        animator.SetFloat(X, direction.x);
        animator.SetFloat(Y, direction.y);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Level")) return;
        colliding = true;
        normal = col.contacts[0].normal;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        colliding = false;
    }
}
