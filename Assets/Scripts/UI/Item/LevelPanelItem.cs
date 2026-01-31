using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelPanelItem : MonoBehaviour
{
    [FormerlySerializedAs("SelfTF")] public RectTransform SelfRT;
    public Image IconImage;
    public TextMeshProUGUI LevelText;
    public Button LevelButton;
    public void SetData(int index,Sprite sprite)
    {
        IconImage.sprite = sprite;
        LevelText.text = "Level " + (index + 1);
    }
}
