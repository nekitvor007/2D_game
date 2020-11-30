using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Movement_controller))]

public class PC_inputController : MonoBehaviour
{
    Movement_controller _playerMovement;
    DateTime _strikeClickTime;

    float _move;
    bool _jump;
    bool _crawling;
    bool _canAttack;

    private void Start()
    {
        _playerMovement = GetComponent<Movement_controller>();
    }
    
    void Update()
    {
        _move = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonUp("Jump"))
        {
            _jump = true;
        }
        _crawling = Input.GetKey(KeyCode.C);

        if (Input.GetKey(KeyCode.E))
            _playerMovement.StartCasting();
        if(Input.GetButtonDown("Fire1"))
        {
            _strikeClickTime = DateTime.Now;
            _canAttack = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            float holdTime = (float)(DateTime.Now - _strikeClickTime).TotalSeconds;
            if (_canAttack)
                _playerMovement.StartStrike(holdTime);
            _canAttack = false;
        }
        if((DateTime.Now - _strikeClickTime).TotalSeconds >= _playerMovement.ChargeTime * 2 && _canAttack)
        {
            _playerMovement.StartStrike(_playerMovement.ChargeTime);
            _canAttack = false;
        }
            
    }

    private void FixedUpdate()
    {
        _playerMovement.Move(_move, _jump, _crawling);
        _jump = false;
    }
}
