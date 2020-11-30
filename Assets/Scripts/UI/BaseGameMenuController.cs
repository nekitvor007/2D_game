using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseGameMenuController : MonoBehaviour
{
    protected ServiceManager _serviceManager;

    [SerializeField] protected GameObject _menu;

    [Header("MainButtons")]
    [SerializeField] protected Button _play;
    [SerializeField] protected Button _settings;
    [SerializeField] protected Button _exit;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _serviceManager = ServiceManager.Instanse;

        _exit.onClick.AddListener(_serviceManager.Exit);
    }

    protected virtual void OnDestroy()
    {
        _exit.onClick.RemoveListener(_serviceManager.Exit);
    }

    protected virtual void Update() { }

    protected virtual void ChangeMenuStatus()
    {
        _menu.SetActive(!_menu.activeInHierarchy);
    }
}
