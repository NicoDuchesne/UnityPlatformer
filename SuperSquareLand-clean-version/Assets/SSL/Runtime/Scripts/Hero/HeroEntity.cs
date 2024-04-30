using UnityEngine;

public class HeroEntity : MonoBehaviour
{
    //Variables and headers
    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody;

    [Header("Horizontal Movements")]
    [SerializeField] private HeroHorizontalMovementsSettings _movementsSettings;
    private float _horizontalSpeed = 0f;
    private float _moveDirX = 0f;

    [Header("Dash")]
    [SerializeField] private HeroDashSettings _dashSettings;
    private float _dashCooldown = 2f;
    private bool isDashing = false;

    [Header("Orientation")]
    [SerializeField] private Transform _orientVisualRoot;
    private float _orientX = 1f;

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = false;

    [Header("Vertical Movements")]
    private float _verticalSpeed = 0f;

    [Header("Fall")]
    [SerializeField] private HeroFallSettings _fallSettings;

    //Public functions
    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }

    //Fixed Update
    private void FixedUpdate()
    {
        //Added code for dash
        if (_dashCooldown > 0f)
        {
            _dashCooldown -= Time.fixedDeltaTime;
        }

        if (_dashCooldown <= 0f && Input.GetKey(KeyCode.E))
        {
            if (_dashSettings.duration > 0f)
            {
                _horizontalSpeed = 40f;
                _dashSettings.duration -= Time.fixedDeltaTime;
                isDashing = true;
            }
            else
            {
                isDashing = false;
                _horizontalSpeed = 0f;
                _dashCooldown = 2f;
                _dashSettings.duration = 0.1f;
            }
        }


        if (!isDashing)
        {
            //Code du cours
            if (_AreOrientAndMovementOpposite())
            {
                _TurnBack();
            }
            else
            {
                _UpdateHorizontalSpeed();
                _ChangeOrientFromHorinzontalMovement();
            }
        }

        _ApplyFallGravity();

        _ApplyHorizontalSpeed();
        _ApllyVerticalSpeed();
    }
    //Functions for vertical movements
    private void _ApplyFallGravity()
    {
        _verticalSpeed -= _fallSettings.fallGravity * Time.fixedDeltaTime;
        if (_verticalSpeed < -_fallSettings.fallSpeedMax)
        {
            _verticalSpeed = -_fallSettings.fallSpeedMax;
        }
    }

    private void _ApllyVerticalSpeed()
    {
        Vector2 velocity = _rigidbody.velocity;
        velocity.y = _verticalSpeed;
        _rigidbody.velocity = velocity;
    }

    //Functions for horizontal movements
    private void _UpdateHorizontalSpeed()
    {
        if (_moveDirX != 0f)
        {
            _Accelerate();
        } else
        {
            _Decelerate();
        }
    }

    private bool _AreOrientAndMovementOpposite()
    {
        return _moveDirX * _orientX < 0f;
    }

    private void _TurnBack()
    {
        _horizontalSpeed -= _movementsSettings.turnBackFrictions * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
            _ChangeOrientFromHorinzontalMovement();
        }
    }

    private void _Decelerate()
    {
        _horizontalSpeed -= _movementsSettings.deceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed < 0f)
        {
            _horizontalSpeed = 0f;
        }
    }

    private void _Accelerate()
    {
        _horizontalSpeed += _movementsSettings.acceleration * Time.fixedDeltaTime;
        if (_horizontalSpeed > _movementsSettings.speedMax)
        {
            _horizontalSpeed = _movementsSettings.speedMax;
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
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Vertical Speed = {_verticalSpeed}");
        GUILayout.Label($"Dash Cooldown = {_dashCooldown}");
        GUILayout.EndVertical();
    }
}