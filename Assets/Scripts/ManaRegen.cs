using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRegen : MonoBehaviour
{
    [SerializeField] private int _healMPValue;
    private void OnTriggerEnter2D(Collider2D info)
    {
        info.GetComponent<Player_controller>().RestoreMP(_healMPValue);
        Destroy(gameObject);
    }
}
