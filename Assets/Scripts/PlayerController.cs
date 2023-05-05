using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _dashForce;
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _dashAmount;
    [SerializeField] private Transform _hand;
    [SerializeField] private float _minimumDistance;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackDamage;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackPointExtension;

    public float DashAmount
    {
        get => _dashAmount;
        set => _dashAmount = Mathf.Clamp(value, 0, MaxDashes);
    }

    public float MaxDashes { get; set; }

    private float dashedTimes;
    private Rigidbody2D _rigidbody;
    private Vector2 direction;
    private Vector2 lastDirection;
    private bool walking;
    private bool dashing;
    private bool readyToDash = true;
    private bool readyToAttack = true;
    private bool attacking;
    private Camera _camera;
    private Vector3 mousePosition;
    private Collider2D[] enemyHits;
    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");
    private static readonly int LastDirX = Animator.StringToHash("lastDir_x");
    private static readonly int LastDirY = Animator.StringToHash("lastDir_y");
    private static readonly int Walking = Animator.StringToHash("walking");
    private static readonly int Dashing = Animator.StringToHash("dashing");
    private static readonly int Attacking = Animator.StringToHash("attacking");


    private void Awake()
    {
        MaxDashes = DashAmount;
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
    }

    private void GetInput()
    {
        walking = false;
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        mousePosition = Input.mousePosition;
        attacking = Input.GetMouseButton(0) && readyToAttack;
        if (direction.magnitude == 0) return;
        // Don't need this if player isn't moving
        lastDirection = direction;
        dashing = Input.GetKey(KeyCode.Space) && readyToDash && DashAmount > 0;
        walking = true;
    }

    private void Animate()
    {
        animator.SetFloat(X, direction.x);
        animator.SetFloat(Y, direction.y);
        animator.SetFloat(LastDirX, lastDirection.x);
        animator.SetFloat(LastDirY, lastDirection.y);
        animator.SetBool(Walking, walking);
        animator.SetBool(Dashing, dashing);
        animator.SetBool(Attacking, attacking);
    }

    private void Update()
    {
        GetInput();
        Animate();
        MoveAttackPoint();
    }

    private void FixedUpdate()
    {
        Movement();
        HandRotation();
        if (dashing && readyToDash && DashAmount > 0) StartCoroutine(PerformDash());
        if (attacking && readyToAttack) StartCoroutine(PerformAttack());
    }

    private void Movement()
    {
        var move = direction * (_speed * Time.fixedDeltaTime);
        _rigidbody.velocity = move;
    }
    

    private IEnumerator PerformDash()
    {
        DashAmount--;
        readyToDash = false;
        _rigidbody.AddForce(direction * _dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        readyToDash = true;
        yield return new WaitForSeconds(_dashCooldown);
        DashAmount++;
    }

    private void MoveAttackPoint()
    {
        var position = transform.position;
        attackPoint.position = new Vector2(
                position.x + lastDirection.x * attackPointExtension, 
                position.y + lastDirection.y * attackPointExtension);
    }

    private IEnumerator PerformAttack()
    {
        readyToAttack = false;
        enemyHits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (var hit in enemyHits)
        {
            if (!hit.isTrigger) continue;
            Debug.Log(hit);
            hit.GetComponent<Health>().TakeDamage(attackDamage);
        }
        yield return new WaitForSeconds(attackCooldown);
        readyToAttack = true;
    }
    
    private void HandRotation()
    {
        var mouseWorldPosition = _camera.ScreenToWorldPoint(mousePosition);
        var position = _hand.position;
        var directionToMouse = new Vector2(mouseWorldPosition.x - position.x,
            mouseWorldPosition.y - position.y);
        if (directionToMouse.magnitude < _minimumDistance) return;
        var angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        _hand.rotation = Quaternion.Lerp(_hand.rotation, rotation, _rotationSpeed * Time.fixedDeltaTime);
    }
}
