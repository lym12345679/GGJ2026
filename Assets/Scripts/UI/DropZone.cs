using System;
using Game.Level;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag;
        var bagItem = dragged.GetComponent<BagItem>();
        LevelsManager.Instance.SetMask(bagItem.level);
        Destroy(bagItem.gameObject); 
        BagPanelUIController.Instance.EndDrag();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = new Color(image.color.r, image.color.g,image.color.b,0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = new Color(image.color.r, image.color.g,image.color.b,0);
    }
}