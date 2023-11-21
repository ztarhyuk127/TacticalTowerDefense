using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerItemActive : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int order;
    Tower tower;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tower = GetComponentInParent<Tower>();
    }

    void Update()
    {
        if (tower.items[order].hasItem)
        {
            spriteRenderer.sprite = PlayManager.instance.itemObjectList[(int)tower.items[order].itemClass].image.sprite;
        }
        else
        {
            spriteRenderer.sprite = null;
        }
    }
}
