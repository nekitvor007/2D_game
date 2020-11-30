using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]

public class Movement_controller : MonoBehaviour
{
    public event Action<bool> OnGetHurt = delegate { };
    private Rigidbody2D _playerRB;
    private Animator _playerAnimator;
    private Player_controller _playerController;

    [Header("Horizontal movement")]
    [SerializeField] private float _speed;

    private bool _faceRight = true;
    private bool _canMove = true;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _radius;
    [SerializeField] private bool _airControll;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _whatIsGround;
    private bool _grounded;

    [Header("Crawling")]
    [SerializeField] private Transform _cellCheck;
    [SerializeField] private Collider2D _headCollider;
    private bool _canStand;

    [Header("Casting")]
    [SerializeField] private GameObject _knife;
    [SerializeField] private Transform _knifePoint;
    [SerializeField] private float _knifeSpeed;
    [SerializeField] private int _castCost;
    private bool _isCasting;

    [Header("Strike")]
    [SerializeField] private Transform _strikePoint;
    [SerializeField] private int _damage;
    [SerializeField] private float _strikeRange;
    [SerializeField] private LayerMask _enemies;
    private bool _isStriking;

    [Header("PowerStrike")]
    [SerializeField] private float _chargeTime;
    public float ChargeTime => _chargeTime;
    [SerializeField] private float _powerStrikeSpeed;
    [SerializeField] private Collider2D _strikeCollider;
    [SerializeField] private int _powerStrikeDamage;
    [SerializeField] private int _powerStrikeCost;
    private List<EnemiesController> _damageEnemies = new List<EnemiesController>();

    [SerializeField] private float _pushForce;
    private float _lastHurtTime;

    void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _playerController = GetComponent<Player_controller>();
    }

    private void FixedUpdate()
    {
        _grounded = Physics2D.OverlapCircle(_groundCheck.position, _radius, _whatIsGround);
        if (_playerAnimator.GetBool("Hurt") && _grounded && Time.time - _lastHurtTime > 0.5f)
            EndHurt();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundCheck.position, _radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_cellCheck.position, _radius);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(_strikePoint.position, _strikeRange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!_strikeCollider.enabled)
        {
            return;
        }

        EnemiesController enemy = collision.collider.GetComponent<EnemiesController>();
        if (enemy == null || _damageEnemies.Contains(enemy))
            return;

        enemy.TakeDamage(_powerStrikeDamage);
        _damageEnemies.Add(enemy);

    }

    void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }

    public void Move(float move, bool jump, bool crawling)
    {
        if (!_canMove)
            return;

        #region Movement

        if (move != 0 && (_grounded || _airControll))
            _playerRB.velocity = new Vector2(_speed * move, _playerRB.velocity.y);

        if (move > 0 && !_faceRight)
        {
            Flip();
        }
        else if (move < 0 && _faceRight)
        {
            Flip();
        }
        #endregion

        #region Jumping
        _grounded = Physics2D.OverlapCircle(_groundCheck.position, _radius, _whatIsGround);
        if (jump && _grounded)
        {
            _playerRB.AddForce(Vector2.up * +_jumpForce);
            jump = false;
        }
        #endregion

        #region Crawling
        _canStand = !Physics2D.OverlapCircle(_cellCheck.position, _radius, _whatIsGround);
        if (crawling)
        {
            _headCollider.enabled = false;
        }
        else if (!crawling && _canStand)
        {
            _headCollider.enabled = true;
        }
        #endregion

        #region Animation
        _playerAnimator.SetFloat("Speed", Mathf.Abs(move));
        _playerAnimator.SetBool("Jump", !_grounded);
        _playerAnimator.SetBool("Crouch", !_headCollider.enabled);
        #endregion
    }

    public void StartCasting()
    {
        if (_isCasting || !_playerController.ChangeMP(-_castCost))
            return;
        _isCasting = true;
        _playerAnimator.SetBool("Casting", true);
    }

    private void CastKnife()
    {
        GameObject knife = Instantiate(_knife, _knifePoint.position, Quaternion.identity); //important
        knife.GetComponent<Rigidbody2D>().velocity = transform.right * _knifeSpeed;
        knife.GetComponent<SpriteRenderer>().flipX = !_faceRight;
        Destroy(knife, 5f);
    }

    private void EndCasting()
    {
        _isCasting = false;
        _playerAnimator.SetBool("Casting", false);
    }

    public void StartStrike(float holdtime)
    {
        if (_isStriking)
            return;

        if(holdtime >= _chargeTime)
        {
            if (!_playerController.ChangeMP(-_powerStrikeCost))
                return;
            _playerAnimator.SetBool("PowerStrike", true);
            _canMove = false;
        }
        else
        {
            _playerAnimator.SetBool("Strike", true);
        }

        _isStriking = true;
    }

    public void GetHurt(Vector2 position)
    {
        _lastHurtTime = Time.time;
        _canMove = false;
        OnGetHurt(false);
        Vector2 pushDirection = new Vector2();
        pushDirection.x = position.x > transform.position.x ? -1 : 1;
        pushDirection.y = 1;
        _playerAnimator.SetBool("Hurt", true);
        _playerRB.AddForce(pushDirection * _pushForce, ForceMode2D.Impulse);
    }

    private void EndHurt()
    {
        _canMove = true;
        _playerAnimator.SetBool("Hurt", false);
        OnGetHurt(true);

    }

    private void StartPowerStrike()
    {
        _playerRB.velocity = transform.right * _powerStrikeSpeed;
        _strikeCollider.enabled = true;
    }

    private void DisablePowerStrike()
    {
        _playerRB.velocity = Vector2.zero;
        _strikeCollider.enabled = false;
        _damageEnemies.Clear();
    }

    private void EndPowerStrike()
    {
        _playerAnimator.SetBool("PowerStrike", false);
        _canMove = true;
        _isStriking = false;
    }

    private void Strike()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(_strikePoint.position, _strikeRange, _enemies);
        for(int i = 0; i < enemies.Length; i++)
        {
            EnemiesController enemy = enemies[i].GetComponent<EnemiesController>();
            enemy.TakeDamage(_damage);
        }
    }

    private void EndStrike()
    {
        _playerAnimator.SetBool("Strike", false);
        _isStriking = false;
    }
}
