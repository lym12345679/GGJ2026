using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BagPanelUIController : MonoInstance<BagPanelUIController>
{
    private RectTransform bagPanel;
    public RectTransform Item;
    public RectTransform Content;
    public List<BagItem> Items = new List<BagItem>();
    public SpriteAtlas ItemAtlas;
    HorizontalLayoutGroup ContentGroup;
    private bool isDragging;
    public RectTransform DragZone;
    private static int cellSize = 64;
    private float cardExpandCalculate => Item.rect.width - cellSize;
    private float cardPosFixed => cellSize * (Items.Count-1);
    public int[] levelUsedIndex=new int[20];
    protected override void Awake()
    {
        base.Awake();
        bagPanel=GetComponent<RectTransform>();
    }

    public void UseMask(int index)
    {
        levelUsedIndex[index]++;
    }
    public void AddLevelItem(int levelIndex)
    {
        //如果该等级道具已经使用两次，则不再添加
        if(levelUsedIndex[levelIndex]>=2) return;
        Item.gameObject.SetActive(true);
        TextMeshProUGUI text = Item.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "Level " + (levelIndex + 1);
        var go= Instantiate(Item, Content);
        go.transform.SetSiblingIndex(Content.transform.childCount-2);
        BagItem bagItem = go.GetComponent<BagItem>();
        bagItem.OnPointerEnterEvent += Focus;
        bagItem.OnPointerExitEvent += UnFocus;
        bagItem.OnBeginDragEvent+=()=>StartDrag(bagItem);
        bagItem.OnEndDragEvent += EndDrag;
        Sprite itemSprite = ItemAtlas.GetSprite((levelIndex + 1).ToString());
        bagItem.SetData(levelIndex,itemSprite);
        // 确保索引在合法范围内：若大于等于 Count 则追加，若小于0 则插入到头部
        //int insertIndex = Mathf.Clamp(levelIndex, 0, Items.Count);
        Items.Add(bagItem);
        Item.gameObject.SetActive(false);
        TransitionContent();
        UnFocus(0);
    }

    private void TransitionContent()
    {
        Content.DOKill();
        Content.DOAnchorPosX( cellSize * Items.Count,0.3f);
    }

    private void DOKillAll()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].SelfRT.DOKill(true);
        }
    }
    public void Focus(int index)
    {
        if(isDragging) return;
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].SelfRT.DOKill();
            if (i <= index)
            {
                Items[i].SelfRT.DOAnchorPosX( (Items.Count-i-1) * cellSize + cardExpandCalculate-cardPosFixed, 0.3f); 
            }
            else
            {
                Items[i].SelfRT.DOAnchorPosX((Items.Count-i-1) * cellSize-cardPosFixed, 0.3f);
            }
        }
    }
    public void UnFocus(int index)
    {
        if(isDragging) return;
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].SelfRT.DOKill();
            Items[i].SelfRT.DOAnchorPosX((Items.Count-i-1) * cellSize-cardPosFixed, 0.3f);
        }
    }

    public void StartDrag(BagItem item)
    {
        DragZone.gameObject.SetActive(true);
        Items.Remove(item);
        isDragging = true;
        DOKillAll();
    }

    public void EndDrag()
    {
        DragZone.gameObject.SetActive(false);
        isDragging = false;
        UnFocus(0);
    }
}
