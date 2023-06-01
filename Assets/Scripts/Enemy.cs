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
    [SerializeField] private bool ranged;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float timeToDamage;
    public int moneyDropped;
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
        if (distanceToTarget <= attackRange && readyToAttack)
            StartCoroutine(ranged ? PerformRangedAttack() : PerformMeleeAttack());
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
    
    public void MoveAttackPoint()
    {
        var position = transform.position;
        attackPoint.position = new Vector2(
                position.x + direction.x * attackPointExtension, 
                position.y + direction.y * attackPointExtension);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator PerformMeleeAttack()
    {
        readyToAttack = false;
        animator.SetTrigger(Attacking);
        yield return new WaitForSeconds(timeToDamage);
        hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, targetLayer);
        foreach (var hit in hits)
        {
            if (!hit.isTrigger) continue;
            hit.GetComponent<Health>().TakeDamage(attackDamage);
        }
        yield return new WaitForSeconds(attackCooldown);
        readyToAttack = true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator PerformRangedAttack()
    {
        readyToAttack = false;
        animator.SetTrigger(Attacking);
        var p = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(timeToDamage);
        p.GetComponent<BoxCollider2D>().isTrigger = false;
        p.GetComponent<Projectile>().damage = attackDamage;
        p.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
        yield return new WaitForSeconds(attackCooldown);
        readyToAttack = true;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Level")) return;
        colliding = true;
        normal = col.contacts[0].normal;
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        colliding = false;
    }
}
