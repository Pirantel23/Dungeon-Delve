using System.Collections;
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
    [SerializeField] private Sprite _dashSprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator animator;
    private Sprite originalSprite;

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
    private Camera _camera;
    private Vector3 mousePosition;
    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");
    private static readonly int LastDirX = Animator.StringToHash("lastDir_x");
    private static readonly int LastDirY = Animator.StringToHash("lastDir_y");
    private static readonly int Walking = Animator.StringToHash("walking");
    private static readonly int Dashing = Animator.StringToHash("dashing");


    private void Awake()
    {
        originalSprite = _spriteRenderer.sprite;
        MaxDashes = DashAmount;
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
    }

    private void GetInput()
    {
        walking = false;
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        dashing = Input.GetKey(KeyCode.Space) && readyToDash;
        mousePosition = Input.mousePosition;
        if (direction.magnitude == 0) return;
        lastDirection = direction;
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
    }

    private void Update()
    {
        GetInput();
        Animate();
    }

    private void FixedUpdate()
    {
        Movement();
        HandRotation();
        if (dashing && readyToDash && DashAmount > 0) StartCoroutine(PerformDash());;
    }

    private void Movement()
    {
        var move = direction * (_speed * Time.fixedDeltaTime);
        _rigidbody.velocity = move;
    }
    

    private IEnumerator PerformDash()
    {
        _spriteRenderer.sprite = _dashSprite;
        DashAmount--;
        readyToDash = false;
        _rigidbody.AddForce(direction * _dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        _spriteRenderer.sprite = originalSprite;
        readyToDash = true;
        yield return new WaitForSeconds(_dashCooldown);
        DashAmount++;
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
