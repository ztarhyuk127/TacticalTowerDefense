using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynergyManager : MonoBehaviour
{
    [System.Serializable]
    public struct SynergyInfo
    {
        public string name;
        public int value;
    }

    // 현재 켜져있는 시너지 수
    [SerializeField]
    public SynergyInfo[] propertyList = new SynergyInfo[7];
    [SerializeField]
    public SynergyInfo[] roleList = new SynergyInfo[6];

    public GameObject forestTower;

    public static SynergyManager instance;

    public float speedRate = 1.0f;

    public int waveCount = 0;

    public List<List<int>> towerPropertyCount = new List<List<int>>();
    public List<List<int>> towerRoleCount = new List<List<int>>();

    private float[] clownPicker_1 = new float[] { 45.0f, 35.0f, 15.0f, 4.0f, 1.0f };
    private float[] clownPicker_2 = new float[] { 40.0f, 32.5f, 20.0f, 6.0f, 1.5f };

    public SynergyIcons synergyIcons;
    public Animator synergyIconAnimator;
    public Sprite inactiveSprite;
    public Sprite activeSpriteGray;
    public Sprite activeSpriteBrown;
    public Sprite activeSpriteWhite;
    public Sprite activeSpriteGold;

    public GameObject synergyInfoPanel;
    public Image synergyInfoImage;
    public TextMeshProUGUI synergyInfoCnt;
    public TextMeshProUGUI synergyInfoName;
    public TextMeshProUGUI synergyInfoDescription;
    public GameObject[] synergyInfoTexts;

    public SynergyInfoUI fireInfoUI;
    public SynergyInfoUI iceInfoUI;
    public SynergyInfoUI windInfoUI;
    public SynergyInfoUI thunderInfoUI;
    public SynergyInfoUI poisonInfoUI;
    public SynergyInfoUI forestInfoUI;
    public SynergyInfoUI thiefInfoUI;
    public SynergyInfoUI priestInfoUI;
    public SynergyInfoUI knightInfoUI;
    public SynergyInfoUI clownInfoUI;
    public SynergyInfoUI adventurerInfoUI;

    public GameObject[] properyEffects;
    public GameObject[] roleEffects;

    

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < 7; i++)
        {
            propertyList[i].name = Enum.GetName(typeof(PlayManager.Property), i);
        }
        for (int j = 0; j < 6; j++)
        {
            roleList[j].name = Enum.GetName(typeof(PlayManager.Role), j);
        }

        towerPropertyCount.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0 });
        towerPropertyCount.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0 });
        towerPropertyCount.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0 });
        towerPropertyCount.Add(new List<int> { 0, 0, 0, 0, 0, 0 });
        towerPropertyCount.Add(new List<int> { 0, 0, 0, 0 });
        towerPropertyCount.Add(new List<int> { 0 });

        towerRoleCount.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0 });
        towerRoleCount.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0 });
        towerRoleCount.Add(new List<int> { 0, 0, 0, 0, 0, 0, 0 });
        towerRoleCount.Add(new List<int> { 0, 0, 0, 0, 0, 0 });
        towerRoleCount.Add(new List<int> { 0, 0, 0, 0 });
        towerRoleCount.Add(new List<int> { 0 });
    }

    // 시너지 검사
    public void SynergyCheck()
    {
        for (int i = 0; i < propertyList.Length; i++)
        {
            propertyList[i].value = 0;
        }
        for (int i = 0; i < roleList.Length; i++)
        {
            roleList[i].value = 0;
        }
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < towerPropertyCount[i].Count; j++)
            {
                towerPropertyCount[i][j] = 0;
                towerRoleCount[i][j] = 0;
            }
        }

        foreach (GameObject towerObject in PlayManager.instance.towerList)
        {
            Tower tower = towerObject.GetComponent<Tower>();
            if (tower.isTower)
            {
                if (towerPropertyCount[tower.towerCost - 1][tower.towerNum - 1] == 0)
                {
                    propertyList[(int)tower.property].value++;
                }
                towerPropertyCount[tower.towerCost - 1][tower.towerNum - 1]++;

                if (towerRoleCount[tower.towerCost - 1][tower.towerNum - 1] == 0)
                {
                    roleList[(int)tower.role].value++;
                }
                towerRoleCount[tower.towerCost - 1][tower.towerNum - 1]++;
            }
        }
    }

    // 속성 시너지 활성화
    public void propertySynergy(int propertyNum, bool isUpgrade)
    {
        // Fire 시너지
        if(propertyNum == 0)
        {
            if (propertyList[propertyNum].value == 8)
            {
                foreach(GameObject tower in PlayManager.instance.towerList)
                {
                    Tower searchTower = tower.GetComponent<Tower>();

                    if((int)searchTower.property == 0)
                    {
                        searchTower.damageRate = 1.15f;

                        searchTower.nowDamage = searchTower.towerDamage * (searchTower.damageRate + searchTower.itemDamageRate - 1);
                    }
                    else
                    {
                        searchTower.damageRate = 1.1f;

                        searchTower.nowDamage = searchTower.towerDamage * (searchTower.damageRate + searchTower.itemDamageRate - 1);
                    }
                }
                synergyIcons.fireBackCircle.sprite = activeSpriteGold;
                fireInfoUI.synergyActiveCnt = 4;
            }
            else if (propertyList[propertyNum].value >= 6)
            {
                foreach (GameObject tower in PlayManager.instance.towerList)
                {
                    Tower searchTower = tower.GetComponent<Tower>();

                    if ((int)searchTower.property == 0)
                    {
                        searchTower.damageRate = 1.15f;

                        searchTower.nowDamage = searchTower.towerDamage * (searchTower.damageRate + searchTower.itemDamageRate - 1);
                    }
                    else
                    {
                        searchTower.damageRate = 1.05f;

                        searchTower.nowDamage = searchTower.towerDamage * (searchTower.damageRate + searchTower.itemDamageRate - 1);
                    }
                }
                synergyIcons.fireBackCircle.sprite = activeSpriteWhite;
                fireInfoUI.synergyActiveCnt = 3;
            }
            else if(propertyList[propertyNum].value >= 4)
            {
                foreach (GameObject tower in PlayManager.instance.towerList)
                {
                    Tower searchTower = tower.GetComponent<Tower>();

                    if ((int)searchTower.property == 0)
                    {
                        searchTower.damageRate = 1.1f;

                        searchTower.nowDamage = searchTower.towerDamage * (searchTower.damageRate + searchTower.itemDamageRate - 1);
                    }
                }
                synergyIcons.fireBackCircle.sprite = activeSpriteBrown;
                fireInfoUI.synergyActiveCnt = 2;
            }
            else if(propertyList[propertyNum].value >= 2)
            {
                foreach (GameObject tower in PlayManager.instance.towerList)
                {
                    Tower searchTower = tower.GetComponent<Tower>();

                    if ((int)searchTower.property == 0)
                    {
                        searchTower.damageRate = 1.05f;

                        searchTower.nowDamage = searchTower.towerDamage * (searchTower.damageRate + searchTower.itemDamageRate - 1);
                    }
                }
                synergyIcons.fireBackCircle.sprite = activeSpriteGray;
                synergyIcons.fireImage.color = Color.white;
                fireInfoUI.synergyActiveCnt = 1;
            }
            else
            {
                foreach (GameObject tower in PlayManager.instance.towerList)
                {
                    Tower searchTower = tower.GetComponent<Tower>();

                    if ((int)searchTower.property == 0)
                    {
                        searchTower.damageRate = 1.0f;

                        searchTower.nowDamage = searchTower.towerDamage * (searchTower.damageRate + searchTower.itemDamageRate - 1);
                    }
                }
                synergyIcons.fireBackCircle.sprite = inactiveSprite;
                synergyIcons.fireImage.color = new Color(0.5f, 0.5f, 0.5f);
                fireInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (propertyList[propertyNum].value)
                {
                    case 2:
                    case 4:
                    case 6:
                    case 8:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradePropertyEffect(propertyNum);
                        break;
                }
            }
        }
        // Ice 시너지
        else if (propertyNum == 1)
        {
            if(propertyList[propertyNum].value == 4)
            {
                speedRate = 0.8f;

                synergyIcons.iceBackCircle.sprite = activeSpriteGold;
                iceInfoUI.synergyActiveCnt = 2;
            }
            else if(propertyList[propertyNum].value >= 2)
            {
                speedRate = 0.9f;

                synergyIcons.iceBackCircle.sprite = activeSpriteBrown;
                synergyIcons.iceImage.color = Color.white;
                iceInfoUI.synergyActiveCnt = 1;
            }
            else
            {
                speedRate = 1.0f;

                synergyIcons.iceBackCircle.sprite = inactiveSprite;
                synergyIcons.iceImage.color = new Color(0.5f, 0.5f, 0.5f);
                iceInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (propertyList[propertyNum].value)
                {
                    case 2:
                    case 4:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradePropertyEffect(propertyNum);
                        break;
                }
            }
        }
        // Electro 시너지
        else if (propertyNum == 2)
        {
            if(propertyList[propertyNum].value == 4)
            {
                synergyIcons.thunderBackCircle.sprite = activeSpriteGold;
                thunderInfoUI.synergyActiveCnt = 2;
            }
            else if (propertyList[propertyNum].value >= 2)
            {
                synergyIcons.thunderBackCircle.sprite = activeSpriteBrown;
                synergyIcons.thunderImage.color = Color.white;
                thunderInfoUI.synergyActiveCnt = 1;
            }
            else
            {
                synergyIcons.thunderBackCircle.sprite = inactiveSprite;
                synergyIcons.thunderImage.color = new Color(0.5f, 0.5f, 0.5f);
                thunderInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (propertyList[propertyNum].value)
                {
                    case 2:
                    case 4:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradePropertyEffect(propertyNum);
                        break;
                }
            }
        }
        // Wind 시너지
        else if (propertyNum == 3)
        {
            if(propertyList[propertyNum].value == 4)
            {
                foreach (GameObject tower in PlayManager.instance.towerList)
                {
                    Tower searchTower = tower.GetComponent<Tower>();

                    if ((int)searchTower.property == 3)
                    {
                        searchTower.delayRate = 0.7f;

                        searchTower.nowFireDealy = (searchTower.nowFireDealy >= searchTower.fireDelay * 0.4f) ? searchTower.fireDelay * (searchTower.delayRate + searchTower.itemDelayRate - 1) : searchTower.fireDelay * 0.4f;
                    }
                }
                synergyIcons.windBackCircle.sprite = activeSpriteGold;
                windInfoUI.synergyActiveCnt = 3;
            }
            else if(propertyList[propertyNum].value >= 3)
            {
                foreach (GameObject tower in PlayManager.instance.towerList)
                {
                    Tower searchTower = tower.GetComponent<Tower>();

                    if ((int)searchTower.property == 3)
                    {
                        searchTower.delayRate = 0.8f;

                        searchTower.nowFireDealy = (searchTower.nowFireDealy >= searchTower.fireDelay * 0.4f) ? searchTower.fireDelay * (searchTower.delayRate + searchTower.itemDelayRate - 1) : searchTower.fireDelay * 0.4f;
                    }
                }
                synergyIcons.windBackCircle.sprite = activeSpriteWhite;
                windInfoUI.synergyActiveCnt = 2;
            }
            else if(propertyList[propertyNum].value >= 2)
            {
                foreach (GameObject tower in PlayManager.instance.towerList)
                {
                    Tower searchTower = tower.GetComponent<Tower>();

                    if ((int)searchTower.property == 3)
                    {
                        searchTower.delayRate = 0.85f;

                        searchTower.nowFireDealy = (searchTower.nowFireDealy >= searchTower.fireDelay * 0.4f) ? searchTower.fireDelay * (searchTower.delayRate + searchTower.itemDelayRate - 1) : searchTower.fireDelay * 0.4f;
                    }
                }
                synergyIcons.windBackCircle.sprite = activeSpriteBrown;
                synergyIcons.windImage.color = Color.white;
                windInfoUI.synergyActiveCnt = 1;
            }
            else
            {
                synergyIcons.windBackCircle.sprite = inactiveSprite;
                synergyIcons.windImage.color = new Color(0.5f, 0.5f, 0.5f);
                windInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (propertyList[propertyNum].value)
                {
                    case 2:
                    case 3:
                    case 4:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradePropertyEffect(propertyNum);
                        break;
                }
            }
        }
        // Poison 시너지
        else if (propertyNum == 4)
        {
            if (propertyList[propertyNum].value == 5)
            {
                synergyIcons.poisonBackCircle.sprite = activeSpriteGold;
                poisonInfoUI.synergyActiveCnt = 3;
            }
            else if (propertyList[propertyNum].value == 4)
            {
                synergyIcons.poisonBackCircle.sprite = activeSpriteWhite;
                poisonInfoUI.synergyActiveCnt = 2;
            }
            else if (propertyList[propertyNum].value == 3)
            {
                PlayManager.instance.poisonCloud_1.SetActive(true);
                PlayManager.instance.poisonCloud_2.SetActive(true);
                PlayManager.instance.poisonCloud_3.SetActive(true);
                PlayManager.instance.poisonCloud_4.SetActive(true);

                synergyIcons.poisonBackCircle.sprite = activeSpriteBrown;
                synergyIcons.poisonImage.color = Color.white;
                poisonInfoUI.synergyActiveCnt = 1;
            }
            else
            {
                PlayManager.instance.poisonCloud_1.SetActive(false);
                PlayManager.instance.poisonCloud_2.SetActive(false);
                PlayManager.instance.poisonCloud_3.SetActive(false);
                PlayManager.instance.poisonCloud_4.SetActive(false);

                synergyIcons.poisonBackCircle.sprite = inactiveSprite;
                synergyIcons.poisonImage.color = new Color(0.5f, 0.5f, 0.5f);
                poisonInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (propertyList[propertyNum].value)
                {
                    case 3:
                    case 4:
                    case 5:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradePropertyEffect(propertyNum);
                        break;
                }
            }
        }
        // Forest 시너지
        else if (propertyNum == 5)
        {
            
            GameObject forestSynergyTower = GameObject.Find("Towers/ForestTower(Clone)");
            
            if (propertyList[propertyNum].value == 6)
            {
                Tower forestTowerControl = forestSynergyTower.GetComponent<Tower>();
                forestTowerControl.rank = 2;

                synergyIcons.forestBackCircle.sprite = activeSpriteGold;
                forestInfoUI.synergyActiveCnt = 3;
            }
            else if(propertyList[propertyNum].value >= 4)
            {
                Tower forestTowerControl = forestSynergyTower.GetComponent<Tower>();
                forestTowerControl.rank = 1;

                synergyIcons.forestBackCircle.sprite = activeSpriteWhite;
                forestInfoUI.synergyActiveCnt = 2;
            }
            else if(propertyList[propertyNum].value >= 2)
            {
                if (forestSynergyTower == null) synergyTowerPicker(0);
                else
                {
                    Tower forestTowerControl = forestSynergyTower.GetComponent<Tower>();
                    forestTowerControl.rank = 0;
                }

                synergyIcons.forestBackCircle.sprite = activeSpriteBrown;
                synergyIcons.forestImage.color = Color.white;
                forestInfoUI.synergyActiveCnt = 1;
            }
            else if(propertyList[propertyNum].value < 2)
            {
                if (forestSynergyTower != null)
                {
                    // 전체 타워 리스트에서 타워 제거
                    PlayManager.instance.towerList.Remove(forestSynergyTower);
                    DragAndDrop dragAndDrop = forestSynergyTower.GetComponent<DragAndDrop>();

                    // 현재 타워가 점유한 위치의 그리드에서 타워 제거
                    if (dragAndDrop.prebuildTarget != null)
                    {
                        BuildGrid prebuildGrid = dragAndDrop.prebuildTarget.GetComponent<BuildGrid>();
                        prebuildGrid.canBuild = true;
                    }
                    Destroy(forestSynergyTower);
                }

                synergyIcons.forestBackCircle.sprite = inactiveSprite;
                synergyIcons.forestImage.color = new Color(0.5f, 0.5f, 0.5f);
                forestInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (propertyList[propertyNum].value)
                {
                    case 2:
                    case 4:
                    case 6:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradePropertyEffect(propertyNum);
                        break;
                }
            }
        }
    }

    // 역할군 시너지 활성화
    public void roleSynergy(int roleNum, bool isUpgrade)
    {
        // Theif 시너지
        if (roleNum == 0)
        {
            if(roleList[roleNum].value == 6)
            {
                synergyIcons.thiefBackCircle.sprite = activeSpriteGold;
                thiefInfoUI.synergyActiveCnt = 3;
            }
            else if(roleList[roleNum].value >= 4)
            {
                synergyIcons.thiefBackCircle.sprite = activeSpriteWhite;
                thiefInfoUI.synergyActiveCnt = 2;
            }
            else if(roleList[roleNum].value >= 3)
            {
                synergyIcons.thiefBackCircle.sprite = activeSpriteBrown;
                synergyIcons.thiefImage.color = Color.white;
                thiefInfoUI.synergyActiveCnt = 1;
            }
            else
            {
                synergyIcons.thiefBackCircle.sprite = inactiveSprite;
                synergyIcons.thiefImage.color = new Color(0.5f, 0.5f, 0.5f);
                thiefInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (roleList[roleNum].value)
                {
                    case 3:
                    case 4:
                    case 6:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradeRoleEffect(roleNum);
                        break;
                }
            }
        }
        // Priest 시너지
        else if (roleNum == 1)
        {
            if(PlayManager.instance.waveNum - PlayManager.instance.priestWave == 5)
            {
                if (roleList[roleNum].value == 6)
                {
                    if (PlayManager.instance.maxHp - PlayManager.instance.hp >= 20)
                    {
                        PlayManager.instance.hp += 20;
                    }
                    else
                    {
                        PlayManager.instance.hp = PlayManager.instance.maxHp;
                    }
                }
                else if (roleList[roleNum].value >= 3)
                {
                    if (PlayManager.instance.maxHp - PlayManager.instance.hp >= 10)
                    {
                        PlayManager.instance.hp += 10;
                    }
                    else
                    {
                        PlayManager.instance.hp = PlayManager.instance.maxHp;
                    }
                }
            }
            if (roleList[roleNum].value == 6)
            {
                /*
                if(PlayManager.instance.maxHp - PlayManager.instance.hp >= 20)
                {
                    PlayManager.instance.hp += 20;
                }
                else
                {
                    PlayManager.instance.hp = PlayManager.instance.maxHp;
                }
                */
                synergyIcons.priestBackCircle.sprite = activeSpriteGold;
                priestInfoUI.synergyActiveCnt = 2;
            }
            else if (roleList[roleNum].value >= 3)
            {
                /*
                if (PlayManager.instance.maxHp - PlayManager.instance.hp >= 10)
                {
                    PlayManager.instance.hp += 10;
                }
                else
                {
                    PlayManager.instance.hp = PlayManager.instance.maxHp;
                }
                */
                synergyIcons.priestBackCircle.sprite = activeSpriteBrown;
                synergyIcons.priestImage.color = Color.white;
                priestInfoUI.synergyActiveCnt = 1;
            }
            else
            {
                synergyIcons.priestBackCircle.sprite = inactiveSprite;
                synergyIcons.priestImage.color = new Color(0.5f, 0.5f, 0.5f);
                priestInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (roleList[roleNum].value)
                {
                    case 3:
                    case 6:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradeRoleEffect(roleNum);
                        break;
                }
            }
        }
        // Knight 시너지
        else if (roleNum == 2)
        {
            if (roleList[roleNum].value == 7)
            {
                synergyIcons.knightBackCircle.sprite = activeSpriteGold;
                knightInfoUI.synergyActiveCnt = 2;
            }
            else if (roleList[roleNum].value >= 4)
            {
                synergyIcons.knightBackCircle.sprite = activeSpriteBrown;
                synergyIcons.knightImage.color = Color.white;
                knightInfoUI.synergyActiveCnt = 1;
            }
            else
            {
                synergyIcons.knightBackCircle.sprite = inactiveSprite;
                synergyIcons.knightImage.color = new Color(0.5f, 0.5f, 0.5f);
                knightInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (roleList[roleNum].value)
                {
                    case 4:
                    case 7:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradeRoleEffect(roleNum);
                        break;
                }
            }
        }
        // Crown 시너지
        else if (roleNum == 3)
        {
            
            if (roleList[roleNum].value == 6)
            {
                // synergyTowerPicker(2);

                synergyIcons.clownBackCircle.sprite = activeSpriteGold;
                clownInfoUI.synergyActiveCnt = 2;

            }
            else if (roleList[roleNum].value >= 4)
            {
                // synergyTowerPicker(1);

                synergyIcons.clownBackCircle.sprite = activeSpriteBrown;
                synergyIcons.clownImage.color = Color.white;
                clownInfoUI.synergyActiveCnt = 1;
            }
            else
            {
                synergyIcons.clownBackCircle.sprite = inactiveSprite;
                synergyIcons.clownImage.color = new Color(0.5f, 0.5f, 0.5f);
                clownInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (roleList[roleNum].value)
                {
                    case 4:
                    case 6:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradeRoleEffect(roleNum);
                        break;
                }
            }
        }
        // Adventurer 시너지
        else if (roleNum == 4)
        {
            if (roleList[roleNum].value == 6)
            {
                synergyIcons.adventurerBackCircle.sprite = activeSpriteGold;
                adventurerInfoUI.synergyActiveCnt = 2;
            }
            else if (roleList[roleNum].value >= 3)
            {
                synergyIcons.adventurerBackCircle.sprite = activeSpriteBrown;
                synergyIcons.adventurerImage.color = Color.white;
                adventurerInfoUI.synergyActiveCnt = 1;
            }
            else
            {
                synergyIcons.adventurerBackCircle.sprite = inactiveSprite;
                synergyIcons.adventurerImage.color = new Color(0.5f, 0.5f, 0.5f);
                adventurerInfoUI.synergyActiveCnt = 0;
            }

            if (isUpgrade)
            {
                switch (roleList[roleNum].value)
                {
                    case 3:
                    case 6:
                        AudioManager.instance.PlaySfx(AudioManager.Sfx.synergy);
                        SynergyUpgradeRoleEffect(roleNum);
                        break;
                }
            }
        }
    }

    public void synergyTowerPicker(int synergyNum)
    {
        for (int i = 0; i < PlayManager.instance.deckPos.Length; i++)
        {
            BuildGrid buildGrid = PlayManager.instance.deckPos[i].GetComponent<BuildGrid>();
            if (buildGrid.canBuild && synergyNum == 0)
            {
                PlayManager.instance.CreateNewDeckUnit(forestTower, i);
                return;
            }
            else if(buildGrid.canBuild && synergyNum == 1)
            {
                GameObject clownTower = ShopManager.instance.RandomCharactorPickup(clownPicker_1);
                PlayManager.instance.CreateNewDeckUnit(clownTower, i);
                return;
            }
            else if((buildGrid.canBuild && synergyNum == 2))
            {
                GameObject clownTower = ShopManager.instance.RandomCharactorPickup(clownPicker_2);
                PlayManager.instance.CreateNewDeckUnit(clownTower, i);
                return;
            }
        }
    }

    public void ShowSynergyIcons()
    {
        synergyIconAnimator.SetBool("ShowIcon", !synergyIconAnimator.GetBool("ShowIcon"));
    }

    public void ShowSynergyInfo(Sprite sprite, string name, string description, string[] text, int synergyActiveCnt, int synergyInt, string trait)
    {
        // 시너지 이미지 설정
        synergyInfoImage.sprite = sprite;

        switch (trait)
        {
            case "Propery":
                synergyInfoCnt.text = propertyList[synergyInt].value.ToString();
                break;
            case "Role":
                synergyInfoCnt.text = roleList[synergyInt].value.ToString();
                break;
        }

        // 시너지 이름 설정
        synergyInfoName.text = name;

        // 시너지 설명 설정
        synergyInfoDescription.text = description + $"\n\n\n[입힌데미지]\n{PlayManager.instance.totalDamage[synergyInt].ToString("F2")}";

        // 시너지 정보 텍스트 설정
        for (int i = 0; i < synergyInfoTexts.Length; i++)
        {
            if (i < text.Length)
            {
                synergyInfoTexts[i].GetComponent<TextMeshProUGUI>().text = text[i];
                if (i < synergyActiveCnt)
                {
                    synergyInfoTexts[i].GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                else
                {
                    synergyInfoTexts[i].GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0.5f);
                }
                synergyInfoTexts[i].SetActive(true);
            }
            else
            {
                synergyInfoTexts[i].SetActive(false);
            }
        }

        synergyInfoPanel.SetActive(true);
    }

    public void CloseSynergyInfo()
    {
        synergyInfoPanel.SetActive(false);
    }

    public void SynergyUpgradePropertyEffect(int property)
    {
        for (int i = 0; i < PlayManager.instance.towerList.Count; i++)
        {
            Tower tower = PlayManager.instance.towerList[i].GetComponent<Tower>();
            if ((int)tower.property == property)
            {
                GameObject effect = Instantiate(properyEffects[property], tower.transform.position, Quaternion.identity);
                StartCoroutine(PlayManager.instance.DestoryEffectTime(effect, 1f));
            }
        }
    }

    public void SynergyUpgradeRoleEffect(int role)
    {
        for (int i = 0; i < PlayManager.instance.towerList.Count; i++)
        {
            Tower tower = PlayManager.instance.towerList[i].GetComponent<Tower>();
            if ((int)tower.role == role)
            {
                GameObject effect = Instantiate(roleEffects[role], tower.transform.position, Quaternion.identity);
                StartCoroutine(PlayManager.instance.DestoryEffectTime(effect, 1f));
            }
        }
    }
}
