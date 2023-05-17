using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float detectionRange;
    [SerializeField] private float minimumRange;
    [SerializeField] private float speed;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform target;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackPointExtension;
    [SerializeField] private LayerMask targetLayer;
    private Collider2D[] hits;
    private Vector2 direction;
    private Rigidbody2D _rigidbody;
    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");
    private float distanceToTarget;
    private bool colliding;
    private bool attacking;
    private bool readyToAttack = true;
    private Vector2 normal;
    private static readonly int Attacking = Animator.StringToHash("attacking");

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        FollowTarget();
        Animate();
        MoveAttackPoint();
    }

    private void FixedUpdate()
    {
        FollowTarget();
        if (distanceToTarget <= attackRange && readyToAttack) StartCoroutine(PerformAttack());
    }

    private void FollowTarget()
    {
        direction = target.position - transform.position;
        distanceToTarget = direction.magnitude;
        if (distanceToTarget > detectionRange || distanceToTarget < minimumRange) return;
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
    
    private void MoveAttackPoint()
    {
        var position = transform.position;
        attackPoint.position = new Vector2(
                position.x + direction.x * attackPointExtension, 
                position.y + direction.y * attackPointExtension);
    }

    private IEnumerator PerformAttack()
    {
        readyToAttack = false;
        animator.SetTrigger(Attacking);
        yield return new WaitForSeconds(attackCooldown / 2);
        hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, targetLayer);
        foreach (var hit in hits)
        {
            if (!hit.isTrigger) continue;
            hit.GetComponent<Health>().TakeDamage(attackDamage);
        }
        yield return new WaitForSeconds(attackCooldown);
        readyToAttack = true;
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
