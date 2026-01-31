using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager
{
    public static GlobalSceneManager Instance=new GlobalSceneManager();
    public GlobalSceneManager()
    {
        
    }
    public void LoadScene(SceneType sceneType)
    { 
        string sceneName = sceneType.ToString().Trim();
        SceneManager.LoadScene(sceneName);
    }
}

public enum SceneType
{
    LevelScene,
    Menu
}
