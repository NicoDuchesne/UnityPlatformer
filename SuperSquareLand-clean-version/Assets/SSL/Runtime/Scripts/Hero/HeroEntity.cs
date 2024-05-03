using UnityEngine;
using UnityEngine.Serialization;

public class HeroEntity : MonoBehaviour
{
    //Variables and headers
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [FormerlySerializedAs("_movementsSettings")]
    [SerializeField] private HeroHorizontalMovementsSettings _groundHorizontalMovementsSettings;
    [SerializeField] private HeroHorizontalMovementsSettings _airHorizontalMovementsSettings;
    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Dash")]
    [SerializeField] private HeroDashSettings _dashSettings;
    [SerializeField] private float _dashCooldown = 2f;
    //private bool isDashing = false;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    [Header("Vertical Movements")]
    private float _verticalSpeed = 0f;

    [Header("Fall")]
    [SerializeField] private HeroFallSettings _fallSettings;

    [Header("Ground")]
    [SerializeField] private GroundDetector _groundDetector;
    public bool IsTouchingGround { get; private set; }

    [Header("Jump")]
    [SerializeField] private HeroJumpSettings _jumpSettings;
    [SerializeField] private HeroFallSettings _jumpFallSettings;

    enum JumpState
    {
        NotJumping,
        JumpImpulsion,
        Falling
    }
    private JumpState _jumpState = JumpState.NotJumping;
    private float _jumpTimer = 0f;


    //Public functions
    public bool IsJumpMinDurationReached => _jumpTimer >= _jumpSettings.jumpMinDuration;
    public bool IsJumpImpulsing => _jumpState == JumpState.JumpImpulsion;
    public void StopJumpImpulsion()
    {
        _jumpState = JumpState.Falling;
    }
    public bool IsJumping => _jumpState != JumpState.NotJumping;
    public void JumpStart()
    {
        _jumpState = JumpState.JumpImpulsion;
        _jumpTimer = 0f;
    }

    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }

    //Fixed Update
    private void FixedUpdate()
    {
        //Added code for dash
        //if (_dashCooldown > 0f)
        //{
        //    _dashCooldown -= Time.fixedDeltaTime;
        //}

        //if (_dashCooldown <= 0f && Input.GetKey(KeyCode.E))
        //{
        //    if (_dashSettings.duration > 0f)
        //    {
        //        _horizontalSpeed = 40f;
        //        _dashSettings.duration -= Time.fixedDeltaTime;
        //        isDashing = true;
        //    }
        //    else
        //    {
        //        isDashing = false;
        //        _horizontalSpeed = 0f;
        //        _dashCooldown = 2f;
        //        _dashSettings.duration = 0.1f;
        //    }
        //}


        //if (!isDashing)
        //{

            //Partie du code du cours à pas executer

        //}

        _ApplyGroundDetection();

        HeroHorizontalMovementsSettings _heroHorizontalMovementsSettings = _GetCurrentHorizontalMovementsSettings();

        if (_AreOrientAndMovementOpposite())
        {
            _TurnBack(_heroHorizontalMovementsSettings);
        }
        else
        {
            _UpdateHorizontalSpeed(_heroHorizontalMovementsSettings);
            _ChangeOrientFromHorinzontalMovement();
        }

        if (IsJumping)
        {
            _UpdateJump();
        }
        else
        {
            if (!IsTouchingGround)
            {
                _ApplyFallGravity(_fallSettings);
            }
            else
            {
                _ResetVerticalSpeed();
            }
        }

        _ApplyHorizontalSpeed();
        _ApplyVerticalSpeed();
    }

    //Functions for air control
    

    //Functions for Jump
    private void _UpdateJumpStateImpulsion()
    {
        _jumpTimer += Time.fixedDeltaTime;
        if (_jumpTimer < _jumpSettings.jumpMaxDuration)
        {
            _verticalSpeed = _jumpSettings.jumpSpeed;
        } else
        {
            _jumpState = JumpState.Falling;
        }
    }

    private void _UpdateJumpStateFalling()
    {
        if(!IsTouchingGround)
        {
            _ApplyFallGravity(_jumpFallSettings);
        } else
        {
            _ResetVerticalSpeed();
            _jumpState = JumpState.NotJumping;
        }
    }

    private void _UpdateJump()
    {
        switch (_jumpState)
        {
            case JumpState.JumpImpulsion:
                _UpdateJumpStateImpulsion();
                break;

            case JumpState.Falling:
                _UpdateJumpStateFalling();
                break;
        }
    }

    //Functions for vertical movements
    private void _ResetVerticalSpeed()
    {
        _verticalSpeed = 0f;
    }
    private void _ApplyGroundDetection()
    {
        IsTouchingGround = _groundDetector.DetectGroundNearBy();
    }
    private void _ApplyFallGravity(HeroFallSettings settings)
    {
        _verticalSpeed -= settings.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -settings.fallSpeedMax)
        {
            _verticalSpeed = -settings.fallSpeedMax;
        }
    }

    private void _ApplyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }


    //Functions for horizontal movements

    private HeroHorizontalMovementsSettings _GetCurrentHorizontalMovementsSettings()
    {
        return IsTouchingGround ? _groundHorizontalMovementsSettings : _airHorizontalMovementsSettings;
    }
    private void _UpdateHorizontalSpeed(HeroHorizontalMovementsSettings settings)
    {
        if (_moveDirX != 0f)
        {
            _Accelerate(settings);
        } else
        {
            _Decelerate(settings);
        }
    }

    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }

    private void _TurnBack(HeroHorizontalMovementsSettings settings)
    {
        _horizontalSpeed -= settings.turnBackFrictions * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorinzontalMovement();
        }
    }

    private void _Decelerate(HeroHorizontalMovementsSettings settings)
    {
        _horizontalSpeed -= settings.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
        }
    }

    private void _Accelerate(HeroHorizontalMovementsSettings settings)
    {
        _horizontalSpeed += settings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > settings.speedMax)
        {
            _horizontalSpeed = settings.speedMax;
        }
    }

    private void _ChangeOrientFromHorinzontalMovement()
    {
        if (_moveDirX == 0f) return;
        _orientX = Mathf.Sign(_moveDirX);
    }

    private void _ApplyHorizontalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.x = _horizontalSpeed * _orientX;
        _rigidbody.velocity = velocity;
    }
    

    //Update
    private void Update()
    {
        _UpdateOrientVisual();
    }

    private void _UpdateOrientVisual()
    {
        Vector3 newScale = _orientVisualRoot.localScale;
        newScale.x = _orientX;
        _orientVisualRoot.localScale = newScale;
    }

    //Debug layout
    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        if (IsTouchingGround)
        {
            GUILayout.Label("OnGround");
        } else
        {
            GUILayout.Label("InAir");
        }
        GUILayout.Label($"Jump State = {_jumpState}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.Label($"Dash Cooldown = {_dashCooldown}");
        GUILayout.EndVertical();
    }
}