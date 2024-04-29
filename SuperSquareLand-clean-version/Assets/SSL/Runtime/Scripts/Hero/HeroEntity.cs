using UnityEngine;

public class HeroEntity : MonoBehaviour
{
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

    public void SetMoveDirX(float dirX)
    {
        _moveDirX = dirX;
    }

    private void FixedUpdate()
    {
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

        

        _ApplyHorizontalSpeed();
    }

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

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(gameObject.name);
        GUILayout.Label($"MoveDirX = {_moveDirX}");
        GUILayout.Label($"OrientX = {_orientX}");
        GUILayout.Label($"Horizontal Speed = {_horizontalSpeed}");
        GUILayout.Label($"Dash Cooldown = {_dashCooldown}");
        GUILayout.EndVertical();
    }
}