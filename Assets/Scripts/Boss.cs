using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] private float minimumRange;
    [SerializeField] private float meleeSpeed;
    [SerializeField] private float rangedSpeed;
    [SerializeField] private float meleeDamage;
    [SerializeField] private float meleeCooldown;
    [SerializeField] private float rangedCooldown;
    [SerializeField] private float meleeRange;
    [SerializeField] private float laserDuration;
    [SerializeField] private Transform target;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackPointExtension;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject laser;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float timeToDamage;
    [SerializeField] private float decisionTime;
    [SerializeField] private int meleeModeChance;
    [SerializeField] private int laserAttackChance;
    [SerializeField] private float laserRotationSpeed;

    [SerializeField] private float despawnTime;
    [SerializeField] private float health;
    [SerializeField] private float healAmount;
    [SerializeField] private Slider healthbar;
    [SerializeField] private int shieldChance;
    [SerializeField] private int shieldDuration;
    [SerializeField] private float shieldAbsorb;
    
    [SerializeField] private float secondPhaseHp;
    [SerializeField] private float secondPhaseHeal;
    [SerializeField] private int secondPhaseMeleeChance;
    [SerializeField] private float secondPhaseDamage;
    [SerializeField] private float secondPhaseSpeed;
    [SerializeField] private float secondPhaseAttackCooldown;
    
    private bool meleeMode;
    private Collider2D[] hits;
    private Vector2 direction;
    private Rigidbody2D _rigidbody;
    
    private float distanceToTarget;
    private bool shielded;
    private bool attacking;
    private bool readyToAttack = true;
    private bool secondPhase;
    
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int Phase = Animator.StringToHash("SecondPhase");
    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");
    private static readonly int Attacking = Animator.StringToHash("attacking");

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(ChangeAttackDecision());
    }
    
    private void Update()
    {
        FollowTarget();
        Animate();
        MoveAttackPoint();
        Heal();
        if (!meleeMode) RotateLaser();
    }

    private IEnumerator Shield()
    {
        Debug.Log("shield");
        shielded = true;
        yield return new WaitForSeconds(shieldDuration);
        shielded = false;
    }
    
    public void TakeDamage(float amount)
    {
        if (Random.Range(0, 100) < shieldChance && !shielded) StartCoroutine(Shield()); 
        health -= shielded ? amount * shieldAbsorb : amount;
        if (!secondPhase && health <= secondPhaseHp) SecondPhase();
        if (health <= 0) StartCoroutine(Die());
    }

    private void SecondPhase()
    {
        meleeDamage = secondPhaseDamage;
        meleeModeChance = secondPhaseMeleeChance;
        meleeSpeed = secondPhaseSpeed;
        meleeCooldown = secondPhaseAttackCooldown;
        healAmount = secondPhaseHeal;
        secondPhase = true;
    }
    
    private IEnumerator Die()
    {
        animator.SetBool(Dead, true);
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }

    private void Heal() => health += healAmount;

    private IEnumerator ChangeAttackDecision()
    {
        meleeMode = Random.Range(0, 100) < meleeModeChance;
        yield return new WaitForSeconds(decisionTime);
        StartCoroutine(ChangeAttackDecision());
    }

    private void FixedUpdate()
    {
        FollowTarget();
        if (distanceToTarget <= meleeRange && readyToAttack && meleeMode) StartCoroutine(PerformMeleeAttack());
        else if (readyToAttack)
        {
            StartCoroutine(Random.Range(0, 100) < laserAttackChance ? PerformLaserAttack() : PerformRangedAttack());
        }
    }

    private void FollowTarget()
    {
        direction = target.position - transform.position;
        distanceToTarget = direction.magnitude;
        if (distanceToTarget < minimumRange) return;
        direction = direction.normalized;
        var move = direction * (meleeMode ? meleeSpeed : rangedSpeed * Time.fixedDeltaTime); 
        _rigidbody.velocity = move;
    }

    private void Animate()
    {
        animator.SetFloat(X, direction.x);
        animator.SetFloat(Y, direction.y);
        animator.SetBool(Phase, secondPhase);
    }

    private void RotateLaser()
    {
        var position = laser.transform.position;
        var dirToPlayer = new Vector2(target.position.x - position.x,
            target.position.y - position.y);
        if (dirToPlayer.magnitude < minimumRange) return;
        var angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        laser.transform.rotation = Quaternion.Lerp(laser.transform.rotation, rotation, laserRotationSpeed * Time.fixedDeltaTime);
    }
    
    public void MoveAttackPoint()
    {
        var position = transform.position;
        attackPoint.position = new Vector2(
                position.x + direction.x * attackPointExtension, 
                position.y + direction.y * attackPointExtension);
    }

    private IEnumerator PerformLaserAttack()
    {
        Debug.Log("laser");
        readyToAttack = false;
        laser.SetActive(true);
        yield return new WaitForSeconds(laserDuration);
        laser.SetActive(false);
        readyToAttack = true;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator PerformMeleeAttack()
    {
        Debug.Log("melee");
        readyToAttack = false;
        animator.SetTrigger(Attacking);
        yield return new WaitForSeconds(timeToDamage);
        hits = Physics2D.OverlapCircleAll(attackPoint.position, meleeRange, targetLayer);
        foreach (var hit in hits)
        {
            if (!hit.isTrigger) continue;
            hit.GetComponent<Health>().TakeDamage(meleeDamage);
        }
        yield return new WaitForSeconds(meleeCooldown);
        readyToAttack = true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator PerformRangedAttack()
    {
        Debug.Log("ranged");
        readyToAttack = false;
        animator.SetTrigger(Attacking);
        var p = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(timeToDamage);
        p.GetComponent<BoxCollider2D>().isTrigger = false;
        p.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
        yield return new WaitForSeconds(rangedCooldown);
        readyToAttack = true;
    }
}
