using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using Game.Level;
using TMPro;

public class TestBtn : MonoBehaviour
{
    public TextMeshProUGUI inputField;

    public void SetMask()
    {
        string mask = inputField.text;
        mask = mask.Trim();
        mask = mask.Replace("\u200b", "");
        int maskIndex = Int32.Parse(mask);
        LevelsManager.Instance.SetMask(maskIndex);
    }

    public void NextLevel()
    {
        LevelsManager.Instance.NextLevel();
    }
}
