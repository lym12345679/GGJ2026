using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GeneralBtn : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Image BaseImg,SelectedImg;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectedImg.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SelectedImg.enabled = false;
    }
}
