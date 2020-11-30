using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour
{
    protected Rigidbody2D _enemyRb;
    protected Animator _enemyAnimator;
    protected Vector2 _startPoint;
    protected EnemyState _currentState;

    protected float _lastStateChange;
    protected float _timeToNextChange;

    [SerializeField] private float _maxStateTime;
    [SerializeField] private float _minStateTime;
    [SerializeField] private EnemyState[] _availableState;
    [SerializeField] private DamageType _collisionDamageType;

    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _range;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _whatIsGround;

    [SerializeField] protected int _collisionDamage;
    [SerializeField] protected float _collisionTimeDelay;
    private float _lastDamageTime;


    protected bool _faceRight = true;

    protected virtual void Start()
    {
        _startPoint = transform.position;
        _enemyRb = GetComponent<Rigidbody2D>();
        _enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (IsGroundEnding())
            Flip();
        if(_currentState == EnemyState.Move)
            Move();
    }

    protected virtual void Update()
    {
        if (Time.time - _lastStateChange > _timeToNextChange)
            GetRandomState();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        TryToDamage(collision.collider);
    }

    protected void TryToDamage(Collider2D enemy)
    {
        if ((Time.time - _lastDamageTime) < _collisionTimeDelay)
            return;

        Player_controller player = enemy.GetComponent<Player_controller>();
        if (player != null)
            player.TakeDamage(_collisionDamage, _collisionDamageType, transform);
    }

    protected virtual void Move()
    {
        _enemyRb.velocity = transform.right * new Vector2(_speed, _enemyRb.velocity.y);
    }

    protected void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_range * 2, 0.5f, 0));
    }

    private bool IsGroundEnding()
    {
        return !Physics2D.OverlapPoint(_groundCheck.position, _whatIsGround);
    }

    protected void GetRandomState()
    {
        int state = Random.Range(0, _availableState.Length);
        
        if (_currentState == EnemyState.Idle && _availableState[state] == EnemyState.Idle)
        {
            GetRandomState();
        }
        
        _timeToNextChange = Random.Range(_minStateTime, _maxStateTime);
        ChangeState(_availableState[state]);
    }

    protected virtual void ChangeState(EnemyState state)
    {
        if (_currentState != EnemyState.Idle)
            _enemyAnimator.SetBool(_currentState.ToString(), false);
        if(state != EnemyState.Idle)
            _enemyAnimator.SetBool(state.ToString(), true);

        _currentState = state;
        _lastStateChange = Time.time;
    }
}

public enum EnemyState
{
    Idle,
    Move,
    Shoot,
}