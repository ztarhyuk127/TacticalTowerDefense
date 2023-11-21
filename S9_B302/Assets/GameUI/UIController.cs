using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    public enum UIType { InventoryPanel, InfoPanel, SynergyInfoPanel, PausePanel };
    public UIType uiType;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject(Input.mousePosition))
        {
            // 패널 밖에서 마우스 클릭한 경우 패널을 닫음
            if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
            {
                switch (uiType)
                {
                    case UIType.InventoryPanel:
                        ShopManager.instance.CloseShopUI();
                        break;
                    case UIType.InfoPanel:
                        PlayManager.instance.CloseItemInfoUI();
                        PlayManager.instance.CloseTowerInfoUI();
                        break;
                    case UIType.SynergyInfoPanel:
                        SynergyManager.instance.CloseSynergyInfo();
                        break;
                    case UIType.PausePanel:
                        PlayManager.instance.PauseResumeGame(false);
                        break;
                }
            }
        }
    }
    public bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = touchPos;

        List<RaycastResult> results = new List<RaycastResult>();


        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
}
