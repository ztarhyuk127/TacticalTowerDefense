using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SynergyInfoUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Sprite sprite;
    public string synergyName;
    [TextArea(3,5)]
    public string synergyDescription;
    [TextArea(3,5)]
    public string[] synergyText;
    public int synergyActiveCnt;
    public int synergyInt;
    public string trait;

    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { } 
    public void OnPointerClick(PointerEventData eventData)
    {
        SynergyManager.instance.ShowSynergyInfo(sprite, synergyName, synergyDescription, synergyText, synergyActiveCnt, synergyInt, trait);
    }
}
