using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Level;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LevelPanelUIController : MonoInstance<LevelPanelUIController>
{
    

    private int levelCount,currentFocusedLevel;
    public RectTransform Content;
    public RectTransform Tag;
    private RectTransform SelfPanel;
    private VerticalLayoutGroup contentGroup;
    private List<LevelPanelItem> levelTags;
    public SpriteAtlas LevelAtlas;
    private float tagHightCalculate => Tag.rect.height + contentGroup.spacing;
    protected override void Awake()
    {
        base.Awake();
        SelfPanel = GetComponent<RectTransform>();
        contentGroup = Content.GetComponent<VerticalLayoutGroup>();
    }
    private void Start()
    {
        levelCount = LevelsManager.Instance.LevelManagers.Count;
        CreateTags();
        UnlockTags(0);
    }
    /// <summary>
    /// 解锁标签
    /// </summary>
    /// <param name="index"></param>
    public void UnlockTags(int index)
    {
        for (int i = 0; i <= index; i++)
        {
            levelTags[i].LevelButton.interactable = true;
        }
        FocusTag(index);
    }
    private void CreateTags()
    {
        levelTags = new List<LevelPanelItem>();
        Tag.gameObject.SetActive(true);
        TextMeshProUGUI text = Tag.GetComponentInChildren<TextMeshProUGUI>();
        for (int i = 0; i < levelCount; i++)
        {
            var go= Instantiate(Tag, Content);
            LevelPanelItem levelPanelItem = go.GetComponent<LevelPanelItem>();
            levelPanelItem.LevelButton.interactable = false;
            Sprite sprite = LevelAtlas.GetSprite((i+1).ToString());
            levelPanelItem.SetData(i, sprite);
            levelPanelItem.LevelButton.onClick.AddListener((() =>
            {
                TextMeshProUGUI t = go.GetComponentInChildren<TextMeshProUGUI>();
                int num = int.Parse(t.text.Replace("Level ","").Trim());
                FocusTag(num-1);
            }));
            levelTags.Add(levelPanelItem);
        }
        Tag.gameObject.SetActive(false);
        float height= SelfPanel.rect.height + (levelCount-1) * tagHightCalculate;
        Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        FocusTag(0);
    }

    private void FocusTag(int levelIndex)
    {
        TransitionContent(levelIndex);
        currentFocusedLevel = levelIndex;
        if(levelIndex >= 0 && levelIndex < levelCount)
        {
            for (int i = 0; i < levelCount; i++)
            {
                var taget = levelTags[i].SelfRT;
                if (i == levelIndex)
                {
                    TransitionLevel1(taget);
                }
                else if (i == levelIndex - 1||i==levelIndex+1)
                {
                    TransitionLevel2(taget);
                }
                else if (i == levelIndex - 2||i==levelIndex+2)
                {
                    TransitionLevel3(taget);
                }
                else if (i == levelIndex - 3||i==levelIndex+3)
                {
                    TransitionLevel4(taget);
                }
                else
                {
                    TransitionLevel5(taget);
                }
            }
        }
    }
    #region 动画效果

    private void TransitionContent(int levelIndex)
    {
        Content.DOKill();
        float duration = 0.5f;
        Content.DOAnchorPos(new Vector2(Content.anchoredPosition.x, tagHightCalculate * levelIndex), duration)
            .SetEase(Ease.OutCubic);
    }
    public void TransitionLevel1(RectTransform target)
    {   
        if (target == null) return;
        // 结束已有 tween 避免冲突
        target.DOKill();
        // 缩放到 (1.33, 2, 1)，持续 0.5 秒，使用平滑缓动
        float duration = 0.5f;
        target.DOScale(new Vector3(1.33f, 2f, 1f), duration).SetEase(Ease.OutCubic);
    }
    public void TransitionLevel2(RectTransform target)
    {   
        if (target == null) return;
        // 结束已有 tween 避免冲突
        target.DOKill();
        // 缩放到 (1.33, 2, 1)，持续 0.5 秒，使用平滑缓动
        float duration = 0.5f;
        target.DOScale(new Vector3(1f, 1f, 1f), duration).SetEase(Ease.OutCubic);
    }

    public void TransitionLevel3(RectTransform target)
    {
        if (target == null) return;
        // 结束已有 tween 避免冲突
        target.DOKill();
        // 缩放到 (1.33, 2, 1)，持续 0.5 秒，使用平滑缓动
        float duration = 0.5f;
        target.DOScale(new Vector3(0.666f, 1f, 1f), duration).SetEase(Ease.OutCubic);
    }
    public void TransitionLevel4(RectTransform target)
    {
        if (target == null) return;
        // 结束已有 tween 避免冲突
        target.DOKill();
        // 缩放到 (1.33, 2, 1)，持续 0.5 秒，使用平滑缓动
        float duration = 0.5f;
        target.DOScale(new Vector3(0.333f, 1f, 1f), duration).SetEase(Ease.OutCubic);
    }

    public void TransitionLevel5(RectTransform target)
    {
        if (target == null) return;
        // 结束已有 tween 避免冲突
        target.DOKill();
        // 缩放到 (1.33, 2, 1)，持续 0.5 秒，使用平滑缓动
        float duration = 0.5f;
        target.DOScale(new Vector3(0f, 1f, 1f), duration).SetEase(Ease.OutCubic);
    }

    #endregion
    
}
