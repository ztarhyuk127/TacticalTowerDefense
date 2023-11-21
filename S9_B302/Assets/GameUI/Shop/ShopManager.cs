using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    // 싱글톤 디자인 패턴
    public static ShopManager instance;

    // UI 요소들
    public GameObject shopPanel;
    public GameObject charSellMenu;
    public GameObject[] charSellBtn;
    public GameObject inventoryMenu;
    public GameObject charToggleBtn;
    public GameObject itemToggleBtn;
    public GameObject rerollBtn;
    public Sprite toggleOn;
    public Sprite toggleOff;
    Animator panelAnimator;

    [Header("상점 캐릭터 프리팹 지정")]
    public GameObject[] charactorObjects_Co1;
    public GameObject[] charactorObjects_Co2;
    public GameObject[] charactorObjects_Co3;
    public GameObject[] charactorObjects_Co4;
    public GameObject[] charactorObjects_Co5;

    [Header("상점 캐릭터 cost 확률표")]
    public float[] probability_1 = new float[5] { 55.0f, 35.0f, 9.0f, 1.0f, 0.0f };
    public float[] probability_2 = new float[5] { 40.0f, 40.0f, 15.5f, 4.0f, 0.5f };
    public float[] probability_3 = new float[5] { 30.0f, 35.0f, 25.0f, 9.0f, 1.0f };
    public float[] probability_4 = new float[5] { 25.0f, 30.0f, 31.0f, 12.0f, 2.0f };
    public float[] probability_5 = new float[5] { 20.0f, 25.0f, 35.0f, 16.0f, 4.0f };
    public float[] probability_6 = new float[5] { 15.0f, 25.0f, 40.0f, 16.0f, 4.0f };
    public TextMeshProUGUI[] probabilityTexts;

    [Header("아이템 인벤토리")]
    public GameObject[] itemInventory;

    [Tooltip("도둑 시너지 활성화")]
    public bool isTheif = false;

    [Header("시너지 이미지 저장용")]
    public Sprite[] propertySprite;
    public Sprite[] roleSprite;

    [Header("발사형태 이미지 저장용")]
    public Sprite[] shotTypeSprites;

    void Awake()
    {
        instance = this;
        

        panelAnimator = shopPanel.GetComponent<Animator>();

        RerollCharactor();
        ToggleCharShop();
    }

    public void ShowShopUI()
    {
        panelAnimator.SetBool("ShowPanel", !panelAnimator.GetBool("ShowPanel"));
    }

    public void CloseShopUI()
    {
        panelAnimator.SetBool("ShowPanel", false);
    }

    public void BuyCharactorInShop(int charIdx)
    {
        for (int i = 0; i < PlayManager.instance.deckPos.Length; i++)
        {
            BuildGrid buildGrid = PlayManager.instance.deckPos[i].GetComponent<BuildGrid>();
            if (buildGrid.canBuild)
            {
                SellComponent sellComponent = charSellBtn[charIdx].GetComponent<SellComponent>();
                Tower tower = sellComponent.SellPrefab.gameObject.GetComponent<Tower>();
                if (sellComponent.isSelled)
                {
                    return;
                }

                if (!isTheif)
                {
                    // 보유 골드가 부족하여 구매 불가.
                    if (tower.towerCost > PlayManager.instance.gold)
                    {
                        PlayManager.instance.NotEnoughGoldPopup.SetActive(true);
                        StartCoroutine(PlayManager.instance.HideUI(PlayManager.instance.NotEnoughGoldPopup, 2f));
                        return;
                    }
                    PlayManager.instance.CreateNewDeckUnit(sellComponent.SellPrefab, i);
                    PlayManager.instance.gold -= tower.towerCost;
                }
                else
                {
                    PlayManager.instance.CreateNewDeckUnit(sellComponent.SellPrefab, i);
                    isTheif = false;
                    ShopCostFree();
                }

                probabilityCalculate(sellComponent);
                sellComponent.SellChar();
                AudioManager.instance.PlaySfx(AudioManager.Sfx.buyTower);
                return;
            }
        }
    }

    public void ShopCostFree()
    {
        for (int i = 0; i < charSellBtn.Length; i++)
        {
            SellComponent sellComponent = charSellBtn[i].GetComponent<SellComponent>();
            sellComponent.ChangeText(isTheif);
        }
    }

    public void ClickRerollBtn()
    {
        if (PlayManager.instance.gold < 2)
        {
            PlayManager.instance.NotEnoughGoldPopup.SetActive(true);
            StartCoroutine(PlayManager.instance.HideUI(PlayManager.instance.NotEnoughGoldPopup, 2f));
            return;
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.reroll);
        PlayManager.instance.gold -= 2;
        if (SynergyManager.instance.roleList[0].value == 6)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue <= 0.1f)
            {
                isTheif = true;
                ShopCostFree();
            }
        }
        else if (SynergyManager.instance.roleList[0].value >= 4)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue <= 0.05f)
            {
                isTheif = true;
                ShopCostFree();
            }
        }
        else if (SynergyManager.instance.roleList[0].value >= 3)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue <= 0.03f)
            {
                isTheif = true;
                ShopCostFree();
            }
        }
        PlayManager.instance.rerollCnt++;
        RerollCharactor();
    }

    public void RerollCharactor()
    {
        for (int i = 0; i < charSellBtn.Length; i++)
        {
            SellComponent sellComponent = charSellBtn[i].GetComponent<SellComponent>();

            probabilityCalculate(sellComponent);

            sellComponent.isSelled = false;
            sellComponent.GetNewChar();
        }

        float[] nowProbability = new float[5];

        if (PlayManager.instance.waveNum <= 5)
        {
            nowProbability = probability_1;
        }
        else if (PlayManager.instance.waveNum <= 10)
        {
            nowProbability = probability_2;
        }
        else if (PlayManager.instance.waveNum <= 15)
        {
            nowProbability = probability_3;
        }
        else if (PlayManager.instance.waveNum <= 20)
        {
            nowProbability = probability_4;
        }
        else if (PlayManager.instance.waveNum <= 25)
        {
            nowProbability = probability_5;
        }
        else
        {
            nowProbability = probability_6;
        }

        for (int i = 0; i < probabilityTexts.Length; i++)
        {
            probabilityTexts[i].text = $"{i + 1}Co\n{nowProbability[i]:F1}%";
        }
    }

    public GameObject RandomCharactorPickup(float[] unitProbabillity)
    {
        float randomValue = Random.Range(0f, 100f);
        float chance = unitProbabillity[0];
        
        if (randomValue < chance) 
        {
            return charactorObjects_Co1[Random.Range(0, charactorObjects_Co1.Length)];
        }
        chance += unitProbabillity[1];

        if (randomValue < chance)
        {
            return charactorObjects_Co2[Random.Range(0, charactorObjects_Co2.Length)];
        }
        chance += unitProbabillity[2];

        if (randomValue < chance)
        {
            return charactorObjects_Co3[Random.Range(0, charactorObjects_Co3.Length)];
        }
        chance += unitProbabillity[3];

        if (randomValue < chance)
        {
            return charactorObjects_Co4[Random.Range(0, charactorObjects_Co4.Length)];
        }
        chance += unitProbabillity[4];

        if (randomValue < chance)
        {
            return charactorObjects_Co5[Random.Range(0, charactorObjects_Co5.Length)];
        }
        
        return null;
    }

    public void ToggleCharShop()
    {
        inventoryMenu.SetActive(false);
        charSellMenu.SetActive(true);
        rerollBtn.SetActive(true);

        charToggleBtn.GetComponent<Image>().sprite = toggleOn;
        itemToggleBtn.GetComponent<Image>().sprite = toggleOff;
    }

    public void ToggleInventory()
    {
        charSellMenu.SetActive(false);
        inventoryMenu.SetActive(true);
        rerollBtn.SetActive(false);

        itemToggleBtn.GetComponent<Image>().sprite = toggleOn;
        charToggleBtn.GetComponent<Image>().sprite = toggleOff;
    }

    public void probabilityCalculate(SellComponent sellComponent)
    {

        // 1~5 웨이브
        if (PlayManager.instance.waveNum <= 5)
        {
            sellComponent.SellPrefab = RandomCharactorPickup(probability_1);
        }
        else if (PlayManager.instance.waveNum <= 10)
        {
            sellComponent.SellPrefab = RandomCharactorPickup(probability_2);
        }
        else if (PlayManager.instance.waveNum <= 15)
        {
            sellComponent.SellPrefab = RandomCharactorPickup(probability_3);
        }
        else if (PlayManager.instance.waveNum <= 20)
        {
            sellComponent.SellPrefab = RandomCharactorPickup(probability_4);
        }
        else if (PlayManager.instance.waveNum <= 25)
        {
            sellComponent.SellPrefab = RandomCharactorPickup(probability_5);
        }
        else
        {
            sellComponent.SellPrefab = RandomCharactorPickup(probability_6);
        }
    }
    
}
