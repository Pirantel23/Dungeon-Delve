using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float detectionRange;
    [SerializeField] public float minimumRange;
    [SerializeField] public float speed;
    [SerializeField] public float attackDamage;
    [SerializeField] public float attackCooldown;
    [SerializeField] public float attackRange;
    [SerializeField] public Transform target;
    [SerializeField] public Animator animator;
    [SerializeField] public Transform attackPoint;
    [SerializeField] public float attackPointExtension;
    [SerializeField] public LayerMask targetLayer;
    [SerializeField] private bool ranged;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float timeToDamage;
    public int moneyDropped;
    public Collider2D[] hits;
    public Vector2 direction;
    public Rigidbody2D _rigidbody;
    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");
    public float distanceToTarget;
    public bool colliding;
    public bool attacking;
    public bool readyToAttack = true;
    public Vector2 normal;
    public static readonly int Attacking = Animator.StringToHash("attacking");

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

    private IEnumerator PerformRangedAttack()
    {
        readyToAttack = false;
        animator.SetTrigger(Attacking);
        Instantiate(projectile, attackPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(timeToDamage);
        projectile.GetComponent<BoxCollider2D>().isTrigger = false;
        projectile.GetComponent<Projectile>().damage = attackDamage;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
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
