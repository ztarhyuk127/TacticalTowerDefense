using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellComponent : MonoBehaviour
{
    public GameObject SellPrefab;
    public Image image;
    public TextMeshProUGUI charNameText;
    public TextMeshProUGUI costText;
    private Tower tower;
    Image componentImage;
    public Image properyImg;
    public Image roleImg;
    public GameObject[] imageObjects;
    public bool isSelled = false;
    public Image attackTypeImg;
    public Image shotTypeImg;

    public void GetNewChar()
    {
        tower = SellPrefab.GetComponent<Tower>();
        ChangeSprite();
        ChangeText(ShopManager.instance.isTheif);
    }

    public void ChangeSprite()
    {
        image.sprite = SellPrefab.GetComponent<SpriteRenderer>().sprite;
        image.color = SellPrefab.GetComponent<SpriteRenderer>().color;
        componentImage = GetComponent<Image>();
        componentImage.color = new Color(componentImage.color.r, componentImage.color.g, componentImage.color.b, 1);

        for (int i = 0; i < imageObjects.Length; i++)
        {
            imageObjects[i].SetActive(true);
        }
        properyImg.sprite = ShopManager.instance.propertySprite[(int)tower.property];
        roleImg.sprite = ShopManager.instance.roleSprite[(int)tower.role];
        attackTypeImg.sprite = PlayManager.instance.attackTypeSprite[(int)tower.attackType];
        shotTypeImg.sprite = ShopManager.instance.shotTypeSprites[(int)tower.ShotType];
    }

    public void ChangeText(bool set)
    {
        charNameText.text = $"{tower.towerName}\n";
        costText.text = $"Cost\n{(set ? $"Free\n({tower.towerCost})" : tower.towerCost)}";
    }

    public void SellChar()
    {
        image.sprite = null;
        image.color = new Color(0, 0, 0, 0);
        componentImage.color = new Color(componentImage.color.r, componentImage.color.g, componentImage.color.b, 0);
        charNameText.text = "";
        costText.text = "";
        for (int i = 0; i < imageObjects.Length; i++)
        {
            imageObjects[i].SetActive(false);
        }
        isSelled = true;
    }
}
