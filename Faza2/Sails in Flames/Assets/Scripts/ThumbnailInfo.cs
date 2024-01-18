using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThumbnailInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    [SerializeField] private GameObject info;
    public void OnPointerEnter(PointerEventData eventData)
    {
        info.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        info.SetActive(false); // exodus
    }
}