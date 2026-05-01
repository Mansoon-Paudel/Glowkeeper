using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpLockTime = 0.2f;
    [SerializeField] private float airGravityScale = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private static readonly int RunHash = Animator.StringToHash("run");
    private static readonly int GroundedHash = Animator.StringToHash("grounded");
    private static readonly int JumpHash = Animator.StringToHash("jump");

    private Rigidbody2D _body;
    private Animator _anim;
    private BoxCollider2D _boxCollider;
    private Vector3 _originalScale;

    private float _wallJumpCooldown;
    private float _horizontalInput;
    private float _facingSign = 1f;

    private InputAction _moveAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();

        _moveAction = new InputAction(type: InputActionType.Value);
        _moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d");

        _jumpAction = new InputAction(type: InputActionType.Button);
        _jumpAction.AddBinding("<Keyboard>/space");
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _jumpAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _jumpAction.Disable();
    }

    private void Update()
    {
        _horizontalInput = _moveAction.ReadValue<float>();

        bool grounded = IsGrounded();
        bool touchingWall = OnWall();

        // Flip
        if (_horizontalInput > 0.01f)
        {
            _facingSign = 1f;
            transform.localScale = new Vector3(Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);
        }
        else if (_horizontalInput < -0.01f)
        {
            _facingSign = -1f;
            transform.localScale = new Vector3(-Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);
        }

        _anim.SetBool(RunHash, Mathf.Abs(_horizontalInput) > 0.01f);
        _anim.SetBool(GroundedHash, grounded);
        _anim.SetBool("Onwall", touchingWall && !grounded);
        if (touchingWall && !grounded )
        {
            _anim.SetTrigger("Wall");
        }

        if (_wallJumpCooldown > wallJumpLockTime)
        {
            // Movement
            if (touchingWall && !grounded)
                _body.linearVelocity = new Vector2(0, _body.linearVelocity.y);
            else
                _body.linearVelocity = new Vector2(_horizontalInput * speed, _body.linearVelocity.y);

            // Wall slide
            if (touchingWall && !grounded)
            {
                _body.gravityScale = 1f;

                if (_body.linearVelocity.y < -wallSlideSpeed)
                    _body.linearVelocity = new Vector2(_body.linearVelocity.x, -wallSlideSpeed);
            }
            else
            {
                _body.gravityScale = airGravityScale;
            }

            // Jump
            if (_jumpAction.WasPressedThisFrame())
                Jump(grounded, touchingWall);
        }
        else
        {
            _wallJumpCooldown += Time.deltaTime;
        }
    } // ✅ THIS BRACE WAS MISSING BEFORE

    private void Jump(bool grounded, bool touchingWall)
    {
        _anim.SetTrigger(JumpHash);

        if (grounded)
        {
            _body.linearVelocity = new Vector2(_body.linearVelocity.x, jumpPower);
        }
        else if (touchingWall)
        {
            float pushDirection = -_facingSign;

            _facingSign *= -1;
            transform.localScale = new Vector3(-transform.localScale.x, _originalScale.y, _originalScale.z);

            _body.linearVelocity = new Vector2(pushDirection * 10f, jumpPower);
            _wallJumpCooldown = 0;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            _boxCollider.bounds.center,
            _boxCollider.bounds.size,
            0,
            Vector2.down,
            0.1f,
            groundLayer
        );

        return hit.collider != null;
    }

    private bool OnWall()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            _boxCollider.bounds.center,
            _boxCollider.bounds.size,
            0,
            new Vector2(_facingSign, 0),
            0.1f,
            wallLayer
        );

        return hit.collider != null;
    }

    public bool CanAttack()
    {
        return Mathf.Abs(_horizontalInput) < 0.01f && IsGrounded() && !OnWall();
    }
}