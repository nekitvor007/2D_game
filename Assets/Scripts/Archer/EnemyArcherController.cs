using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArcherController : EnemyGoblinController
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private float _arrowSpeed;
    [SerializeField] private float _angerRange;

    private bool _isAngry;
    protected bool _attacking;

    protected Player_controller _player;

    protected override void Start()
    {
        base.Start();
        _player = FindObjectOfType<Player_controller>();
        StartCoroutine(ScanForPlayer());
    }

    protected override void Update()
    {
        if (_isAngry)
            return;
        base.Update();
    }
    protected void Shoot()
    {
        GameObject arrow = Instantiate(_projectilePrefab, _shootPoint.position, Quaternion.identity);
        arrow.GetComponent<Rigidbody2D>().velocity = transform.right * _arrowSpeed;
        arrow.GetComponent<SpriteRenderer>().flipX = !_faceRight;
        Destroy(arrow, 5f);
    }

    protected IEnumerator ScanForPlayer()
    {
        while(true)
        {
            CheckPlayerInRange();
            yield return new WaitForSeconds(1f);
        }
    }

    protected void CheckPlayerInRange()
    {
        if(_player == null || _attacking)
        {
            return;
        }

        if (Vector2.Distance(transform.position, _player.transform.position) < _angerRange)
        {
            _isAngry = true;
            TurnToPlayer();
            ChangeState(EnemyState.Shoot);
        }
        else
            _isAngry = false;
    }

    protected void TurnToPlayer()
    {
        if (_player.transform.position.x - transform.position.x > 0 && !_faceRight)
            Flip();
        else if (_player.transform.position.x - transform.position.x < 0 && _faceRight)
            Flip();
    }

    protected override void ChangeState(EnemyState state)
    {
        base.ChangeState(state);
        switch (state)
        {
            case EnemyState.Shoot:
                _attacking = true;
                _enemyRb.velocity = Vector2.zero;
                break;
        }
    }

    protected void EndState()
    {
        switch (_currentState)
        {
            case EnemyState.Shoot:
                _attacking = false;
                break;
        }
        if(!_isAngry)
            ChangeState(EnemyState.Idle);
    }

    protected void DoStateAction()
    {
        switch(_currentState)
        {
            case EnemyState.Shoot:
                Shoot();
                break;
        }
    }

}
