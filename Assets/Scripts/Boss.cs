using System.Collections;
using System.Collections.Generic;
using Codice.Client.ChangeTrackerService;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] private float minimumRange;
    [SerializeField] private float meleeSpeed;
    [SerializeField] private float rangedSpeed;
    [SerializeField] private float meleeDamage;
    [SerializeField] private float laserDamage;
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
    [SerializeField] private float timeToMeleeDamage;
    [SerializeField] private float timeToRangedDamage;
    [SerializeField] private float timeToLaserDamage;
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

    [SerializeField] private float invincibilityDuration;
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
    private Animator laserAnimator;
    
    private float distanceToTarget;
    private float maxHealth;
    private bool shielded;
    private bool attacking;
    private bool readyToAttack = true;
    private bool secondPhase;
    private bool transforming;

    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int Phase = Animator.StringToHash("SecondPhase");
    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");
    private static readonly int Melee = Animator.StringToHash("Melee");
    private static readonly int Ranged = Animator.StringToHash("Ranged");
    private static readonly int Laser1 = Animator.StringToHash("Laser");
    private static readonly int Transforming = Animator.StringToHash("Transforming");
    private static readonly int Armor = Animator.StringToHash("Armor");

    private void Start()
    {
        maxHealth = health;
        _rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(ChangeAttackDecision());
        StartCoroutine(Heal());
        laserAnimator = laser.GetComponent<Animator>();
    }
    
    private void Update()
    {
        Animate();
        MoveAttackPoint();
        UpdateHealthbar();
        if (!meleeMode) RotateLaser();
    }

    private IEnumerator Shield()
    {
        Debug.Log("shield");
        shielded = true;
        yield return new WaitForSeconds(shieldDuration);
        shielded = false;
    }

    private void UpdateHealthbar() => healthbar.value = health / maxHealth;
    
    public void TakeDamage(float amount)
    {
        if (transforming) return;
        Debug.Log($"damage {amount}");
        if (Random.Range(0, 100) < shieldChance && !shielded) StartCoroutine(Shield()); 
        health -= shielded ? amount * shieldAbsorb : amount;
        if (!secondPhase && health <= secondPhaseHp) StartCoroutine(SecondPhase());
        if (health <= 0) StartCoroutine(Die());
    }

    private IEnumerator SecondPhase()
    {
        var bodyType = _rigidbody.bodyType;
        _rigidbody.velocity = Vector2.zero;
        transforming = true;
        secondPhase = true;
        meleeDamage = secondPhaseDamage;
        meleeModeChance = secondPhaseMeleeChance;
        meleeSpeed = secondPhaseSpeed;
        meleeCooldown = secondPhaseAttackCooldown;
        healAmount = secondPhaseHeal;
        yield return new WaitForSeconds(invincibilityDuration);
        transforming = false;
        bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.bodyType = bodyType;
    }
    
    private IEnumerator Die()
    {
        animator.SetBool(Dead, true);
        enabled = false;
        _rigidbody.velocity = Vector2.zero;
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }

    private IEnumerator Heal()
    {
        health += healAmount;
        if (health > maxHealth) health = maxHealth;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Heal());
    }

    private IEnumerator ChangeAttackDecision()
    {
        meleeMode = Random.Range(0, 100) < meleeModeChance;
        yield return new WaitForSeconds(decisionTime);
        StartCoroutine(ChangeAttackDecision());
    }

    private void FixedUpdate()
    {
        if (transforming) return;
        FollowTarget();
        if (readyToAttack)
        {
            StartCoroutine(meleeMode ? PerformMeleeAttack() :
                Random.Range(0, 100) < laserAttackChance ? PerformLaserAttack() : PerformRangedAttack());
        }
    }

    private void FollowTarget()
    {
        direction = target.position - transform.position;
        distanceToTarget = direction.magnitude;
        if (distanceToTarget < minimumRange) return;
        direction = direction.normalized;
        var move = direction * (meleeMode ? meleeSpeed * Time.fixedDeltaTime : rangedSpeed * Time.fixedDeltaTime);
        _rigidbody.velocity = move;
    }

    private void Animate()
    {
        animator.SetFloat(X, direction.x);
        animator.SetFloat(Y, direction.y);
        animator.SetBool(Phase, secondPhase);
        animator.SetBool(Transforming, transforming);
        animator.SetBool(Armor, shielded);
    }

    private void RotateLaser()
    {
        var position = laser.transform.position;
        var dirToPlayer = new Vector2(target.position.x - position.x,
            target.position.y - position.y);
        if (dirToPlayer.magnitude < minimumRange) return;
        var angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        laser.transform.rotation = Quaternion.Slerp(laser.transform.rotation, rotation, laserRotationSpeed * Time.fixedDeltaTime);
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
        var l = laser.GetComponent<Laser>();
        l.damage = 0;
        animator.SetTrigger(Laser1);
        readyToAttack = false;
        Debug.Log("laser");
        yield return new WaitForSeconds(timeToLaserDamage);
        laser.SetActive(true);
        laserAnimator.SetFloat("SecondPhase", secondPhase ? 1f : 0f);
        yield return new WaitForSeconds(0.5f);
        l.damage = laserDamage;
        yield return new WaitForSeconds(laserDuration);
        laser.SetActive(false);
        yield return new WaitForSeconds(rangedCooldown);
        readyToAttack = true;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator PerformMeleeAttack()
    {
        if (distanceToTarget > meleeRange) yield break;
        animator.SetTrigger(Melee);
        Debug.Log("melee");
        readyToAttack = false;
        yield return new WaitForSeconds(timeToMeleeDamage);
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
        animator.SetTrigger(Ranged);
        Debug.Log("ranged");
        readyToAttack = false;
        yield return new WaitForSeconds(0.7f);
        var p = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(timeToRangedDamage);
        p.GetComponent<BoxCollider2D>().isTrigger = false;
        p.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
        p.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI + 180f);
        yield return new WaitForSeconds(rangedCooldown);
        readyToAttack = true;
    }
}
