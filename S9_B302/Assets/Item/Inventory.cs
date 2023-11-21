using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public PlayManager.ItemClass itemClass;
    public bool hasItem = false;
    public Image image;
    public bool isTower = false;

    void Awake()
    {
        image = GetComponent<Image>();
        itemClass = PlayManager.ItemClass.Empty;
    }

    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isTower && !hasItem && itemClass != PlayManager.ItemClass.Empty)
        {
            PlayManager.instance.OpenItemInfoUI(PlayManager.instance.itemObjectList[(int)itemClass]);
        }
    }
}
