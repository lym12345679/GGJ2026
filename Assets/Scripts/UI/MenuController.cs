using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public RectTransform Mask,Panel;
    public Button StartButton,ExitButton;
    private Vector2 centerPos;
    private void Start()
    {
        centerPos=new Vector2(Screen.width/2f,Screen.height/2f);
        StartButton.onClick.AddListener(OnStart);
        ExitButton.onClick.AddListener(OnExit);
    }

    private void Update()
    {
        Vector3 mousePos=Input.mousePosition;
        Mask.anchoredPosition = mousePos;
        Panel.anchoredPosition =new Vector2(centerPos.x-mousePos.x+Mask.rect.width/2,centerPos.y-mousePos.y+Mask.rect.height/2); ;
    }

    private void OnStart()
    {
        GlobalSceneManager.Instance.LoadScene(SceneType.LevelScene);
    }
    private void OnExit()
    {
        Application.Quit();
    }
}
