using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LvlButtonsController : MonoBehaviour
{
    private Button _button;
    [SerializeField] private Scenes scene;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ChangeLvl);
        GetComponentInChildren<TMP_Text>().text = ((int)scene).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeLvl()
    {
        ServiceManager.Instanse.ChangeLvl((int)scene);
    }
}
