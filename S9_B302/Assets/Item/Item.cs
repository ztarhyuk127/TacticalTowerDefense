using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("������ ����")]
    public string itemName;
    [TextArea(3, 10)]
    public string itemDescription;
    [TextArea(3, 10)]
    public string itemParameter;
    public PlayManager.ItemClass itemClass = PlayManager.ItemClass.Empty;
    public Image image;

    private RectTransform rectTransform;
    private Vector3 nowRectTransform;
    private Canvas canvas;
    [HideInInspector]
    public Inventory preInventory;
    private bool isDragging = false;

    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (!isDragging)
        {
            rectTransform.position = preInventory.GetComponent<RectTransform>().position;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        nowRectTransform = rectTransform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PlayManager.instance.isGameOver || PlayManager.instance.isPauseGame)
        {
            return;
        }

        isDragging = true;
        // rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        rectTransform.position = Input.mousePosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging) return;
        PlayManager.instance.OpenItemInfoUI(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (PlayManager.instance.isGameOver || PlayManager.instance.isPauseGame)
        {
            return;
        }
        Invoke(nameof(CancelDragging), 0.5f);

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            // �ٸ� UI ��Ҹ� �������� �� ó��
            if (result.gameObject != gameObject && result.gameObject.CompareTag("Inventory"))
            {
                Inventory inventory = result.gameObject.GetComponent<Inventory>();

                if (!inventory.hasItem)
                {
                    inventory.hasItem = true;
                    MoveToCenter(inventory.GetComponent<RectTransform>().position);
                    
                    if (preInventory != null)
                    {
                        preInventory.hasItem = false;
                        preInventory.itemClass = PlayManager.ItemClass.Empty;
                    }
                    preInventory = inventory;
                    return;
                }
            }
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector3.forward);
        if (hit.collider != null && hit.collider.CompareTag("Tower"))
        {
            // Ÿ�� �ȿ� �������� �ִ� ����
            Tower tower = hit.collider.GetComponent<Tower>();
            for (int i = 0; i < tower.items.Length; i++)
            {
                // �ռ� ������ �������� ��� ���� ��
                if (tower.items[i].hasItem)
                {
                    PlayManager.ItemClass newItemClass = PlayManager.instance.ItemPlusCheck(tower.items[i].itemClass, itemClass);
                    Debug.Log("itemClass : " + newItemClass);
                    if (newItemClass != PlayManager.ItemClass.Empty)
                    {
                        tower.ItemActive(newItemClass);
                        tower.ItemInActive(tower.items[i].itemClass);
                        tower.items[i].itemClass = newItemClass;
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.item);
                        if (preInventory != null)
                        {
                            preInventory.hasItem = false;
                            preInventory.itemClass = PlayManager.ItemClass.Empty;
                        }
                        Destroy(gameObject);
                        return;
                    }
                }

                // �κ��丮�� ������� ���� ��
                if (!tower.items[i].hasItem)
                {
                    tower.items[i].hasItem = true;
                    tower.items[i].itemClass = itemClass;
                    tower.ItemActive(itemClass);
                    if (preInventory != null)
                    {
                        preInventory.hasItem = false;
                        preInventory.itemClass = PlayManager.ItemClass.Empty;
                    }
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.item);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        MoveToCenter(nowRectTransform);
    }

    public void MoveToCenter(Vector3 targetUIPosition)
    {
        rectTransform.position = targetUIPosition;
    }

    private void CancelDragging()
    {
        isDragging = false;
    }
}
