using System.Collections;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _dashForce;
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _dashAmount;
    [SerializeField] private Transform _hand;
    [SerializeField] private float _minimumDistance;
    [SerializeField] private float _rotationSpeed;

    public float DashAmount
    {
        get => _dashAmount;
        set => _dashAmount = Mathf.Clamp(value, 0, MaxDashes);
    }

    public float MaxDashes { get; set; }

    private float dashedTimes;
    private Rigidbody2D _rigidbody;
    private Vector2 direction;
    private bool dashing;
    private bool readyToDash = true;
    private Camera _camera;
    private Vector3 mousePosition;


    private void Awake()
    {
        MaxDashes = DashAmount;
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
    }

    private void GetInput()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        dashing = Input.GetKey(KeyCode.Space);
        mousePosition = Input.mousePosition;
    }
    
    private void Update()
    {
        GetInput();
        Debug.Log($"{DashAmount} {readyToDash}");
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
        DashAmount--;
        readyToDash = false;
        _rigidbody.AddForce(direction * _dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
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
