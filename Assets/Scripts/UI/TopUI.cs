using System.Collections;
using System.Collections.Generic;
using Game.Level;
using UnityEngine;
using UnityEngine.UI;

public class TopUI : MonoInstance<TopUI>
{
    public Button StartButton;
    public Button RestartButton;
    public Button PromptButton;
    public Button ExitButton;
    public Image PromptImage;
    protected override void Awake()
    {
        base.Awake();
        StartButton.onClick.AddListener(StartGame);
        RestartButton.onClick.AddListener(RestartGame);
        PromptButton.onClick.AddListener(Prompt);
        ExitButton.onClick.AddListener(Exit);
    }

    private void StartGame()
    {
        LevelsManager.Instance.UnfreezePlayer();
        StartButton.gameObject.SetActive(false);
    }

    private void RestartGame()
    {
        LevelsManager.Instance.Restart();
    }
    public void ShowStartButton()
    {
        StartButton.gameObject.SetActive(true);
    }

    public void Exit()
    {
        GlobalSceneManager.Instance.LoadScene(SceneType.Menu);
    }
    private void Prompt()
    {
        if (!PromptImage.gameObject.activeSelf)
        {
            PromptImage.gameObject.SetActive(true);
        }
        else
        {
            PromptImage.gameObject.SetActive(false);
        }
    }
}
