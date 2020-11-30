using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : BaseGameMenuController
{
    [SerializeField] private Button _chooseLvl;
    [SerializeField] private Button _reset;

    [SerializeField] private GameObject _lvlMenu;
    [SerializeField] private Button _closeLvlMenu;

    private int _lvl = 1;

    protected override void Start()
    {
        base.Start();
        _chooseLvl.onClick.AddListener(UseLvlMenu);
        _closeLvlMenu.onClick.AddListener(UseLvlMenu);

        if (PlayerPrefs.HasKey(GamePrefs.LastPlayedLvl.ToString()))
        {
            _play.GetComponentInChildren<TMP_Text>().text = "Resume";
            _lvl = PlayerPrefs.GetInt(GamePrefs.LastPlayedLvl.ToString());
        }
        _play.onClick.AddListener(Play);
        _reset.onClick.AddListener(OnResetClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _chooseLvl.onClick.RemoveListener(UseLvlMenu);
        _closeLvlMenu.onClick.RemoveListener(UseLvlMenu);
        _play.onClick.RemoveListener(Play);
        _reset.onClick.RemoveListener(_serviceManager.ResetProgress);
    }

    private void UseLvlMenu()
    {
        _lvlMenu.SetActive(!_lvlMenu.activeInHierarchy);
        ChangeMenuStatus();
    }

    private void Play()
    {
        _serviceManager.ChangeLvl(_lvl);
    }
    
    private void OnResetClicked()
    {
        _play.GetComponentInChildren<TMP_Text>().text = "Play";
        _serviceManager.ResetProgress();
    }
}
