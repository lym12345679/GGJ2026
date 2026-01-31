using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class BagItem : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public event Action<int> OnPointerEnterEvent;
    public event Action<int> OnPointerExitEvent;
    public event Action OnBeginDragEvent;
    public event Action OnEndDragEvent;
    public RectTransform SelfRT;
    public TextMeshProUGUI LevelText;
    [HideInInspector]
    public int level;
    public Image Image;
    
    public Canvas canvas;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private int originalSiblingIndex;
    private Vector2 originalAnchoredPos;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetData(int index,Sprite sprite)
    {
        level = index;
        Image.sprite = sprite;
        LevelText.text = "Level " + (level + 1);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(SelfRT.GetSiblingIndex());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke(SelfRT.GetSiblingIndex());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 可用于记录按下时的状态（可选）
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragEvent?.Invoke();
        SelfRT.DOKill(true);
        originalParent = SelfRT.parent;
        originalSiblingIndex = SelfRT.GetSiblingIndex();
        originalAnchoredPos = SelfRT.anchoredPosition;

        // 把拖拽物放到 Canvas 最顶层，方便显示
        SelfRT.SetParent(canvas.transform, true);
        SelfRT.SetAsLastSibling();

        // 允许穿透（不阻挡 Drop 区）
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransform canvasRT = canvas.transform as RectTransform;
        // 将屏幕点转换为 Canvas 本地坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, 
                eventData.position, 
                eventData.pressEventCamera, 
                out localPoint))
        {
            SelfRT.anchoredPosition = localPoint+new Vector2(Screen.width/2f,-Screen.height/2f);
            //SelfRT.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 恢复父物体和位置（若需要放到某个 Drop 区，请在 Drop 处理里设置父物体）
        SelfRT.SetParent(originalParent, true);
        SelfRT.SetSiblingIndex(originalSiblingIndex);
        SelfRT.anchoredPosition = originalAnchoredPos;
        canvasGroup.blocksRaycasts = true;
        // BagPanelUIController.Instance.Items.Insert(level,this);
        BagPanelUIController.Instance.Items.Insert(originalSiblingIndex,this);
        OnEndDragEvent?.Invoke();
    }
}
