using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    // 싱글톤 디자인 패턴
    public static PlayManager instance;

    [Tooltip("밸런스 테스트 위한 변수들")]
    public float[] totalDamage;
    public float monsterHP;
    public string[] towerName;
    public int normalCnt;
    public int criticalCnt;

    [Header("오브젝트 참조용")]
    public GameObject TowerParent;
    public GameObject Monster;
    public GameObject nextWaveClickButton;
    public GameObject waveText;
    public GameObject goldText;
    public GameObject healthText;
    public GameObject timerText;
    public GameObject restTimeText;
    public GameObject pauseBackgroundImg;
    public GameObject pausePanel;
    public GameObject gameoverBackground;
    public GameObject gameoverPanel;
    public GameObject gameclearPanel;
    public GameObject itemInventoryPanel;
    public GameObject itemInfoPanel;
    public GameObject itemCombinePanel;
    public GameObject towerInfoPanel;
    public GameObject[] canbuildObjects;
    public Sprite[] attackTypeSprite;
    public GameObject damageText;
    public GameObject DamageCanvas;
    public GameObject NotEnoughGoldPopup;
    public GameObject ItemObtainPanel;
    public GameObject StageStartPopupPanel;
    public GameObject BossPopupPanel;
    public GameObject BossHealthBar;
    public GameObject gameResultPanel;
    public GameObject towerRadiusPrefab;
    public static GameObject towerRadiusObject;
    public GameObject speedBtnActive;

    [Header("deck 관련 변수")]
    public GameObject[] deckPos;

    // 타워 시너지 및 공격타입 관련 정보 정의
    public enum Property { Fire, Ice, Electro, Wind, Poison, Forest, Special };
    public enum Role { Thief, Priest, Knight, Clown, Adventurer, Special };
    public enum AttackType { Attack, Magic };
    public enum ShotType { Bullet, Instant, Area, Dot };

    public enum ArmorType { AttackArmor, MagicArmor, BossArmor };

    [Header("웨이브 및 몬스터 관련 정보")]
    public GameObject startPoint;
    public GameObject[] spawnMonsterObject;
    public int spawnMonsterCount = 10;
    public int maxSpawnMonsterCount = 10;
    public int nowSpawnMonsterCount = 0;
    private float spawnTime = 1.5f;
    public bool startSpawn = false;
    public bool endSpawn = false;
    public float nextSpawnTime = 10f;
    public double gameTimer = 0f;
    public GameObject[] WayPoints;
    public GameObject monsterHitEffect;

    [Header("플레이어 관련 정보")]
    public int hp;
    Coroutine waveWaitTimeCoroutine;
    public int maxHp;
    public int gold;
    public int giveGold;
    public int maxInterestRate;
    public List<GameObject> towerList = new List<GameObject>();

    [Header("현재 웨이브")]
    public int waveNum = 0;
    public int maxWaveNum = 50;

    [Header("아이템 관련 변수")]
    public Item[] itemObjectList;
    public bool isItemDroped = false;
    public float itemDropChance;
    public enum ItemClass { Empty, Attack, Magic, Speed, Critical, APA, APM, APS, APC, MPM, MPS, MPC, SPS, SPC, CPC };

    [Header("독구름 오브젝트")]
    public GameObject poisonCloud_1;
    public GameObject poisonCloud_2;
    public GameObject poisonCloud_3;
    public GameObject poisonCloud_4;

    [Header("번개 오브젝트")]
    public GameObject thunderObject;

    [Header("toggle 변수들")]
    public bool isPauseGame = false;
    public bool isGameOver = false;
    public bool isGameClear = false;

    [Header("정보창 관련 변수들")]
    public Animator itemInfoUIAnimator;
    public Animator towerInfoUIAnimator;
    private Tower targetUITower;
    private Item targetUIItem;
    private StageStartPopup stageStartPopup;
    private Animator stageStartAnim;
    public Image bossHealthOverride;
    public Monster bossMonster;
    public bool isBoss = false;
    
    [Header("기타 여러 변수")]
    public float damageTextSpeed;
    public int[][] rankActive = new int[3][] { new int[3]{ 0,1,0 }, new int[3] { 1,0,1 }, new int[3] { 1,1,1 } };
    public bool isPriest = false;
    public int priestWave = 0;
    public FirebaseAuth auth;
    public GameObject settingsUI;
    public float speedTime = 1f;

    [Header("결과창에 보여줄 변수")]
    public TextMeshProUGUI resultWaveCntText;
    public TextMeshProUGUI resultTimeText;
    public TextMeshProUGUI resultGoldText;
    public TextMeshProUGUI resultRerollCntText;
    public int rerollCnt = 0;
    public int earnGoldCnt = 0;

    public TextMeshProUGUI[] synergyDamages;
    public TextMeshProUGUI totalDamageText;
    public TextMeshProUGUI itemDropText;
    public int itemDropCnt = 0;

    void Awake()
    {
        instance = this;

        startPoint = GameObject.Find("Start");
        hp = maxHp;

        waveWaitTimeCoroutine = StartCoroutine(NextWaveWaitTime());
        AudioManager.instance.PlayBgm(true);

        auth = FirebaseAuth.DefaultInstance;

        stageStartPopup = StageStartPopupPanel.GetComponent<StageStartPopup>();
        stageStartAnim = StageStartPopupPanel.GetComponent<Animator>();

        totalDamage = new float[13];
        priestWave = maxWaveNum;

        towerRadiusObject = Instantiate(towerRadiusPrefab, Vector3.zero, Quaternion.identity);
        towerRadiusObject.transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (isGameOver || isPauseGame || isGameClear) return;

        CheckNextWave();

        if (startSpawn)
        {
            StartCoroutine(SpawnMonster());
        }

        UpdateTimer();
        goldText.GetComponent<TextMeshProUGUI>().text = $"{gold}\n+({Math.Clamp(gold / 10, 0, maxInterestRate)})";
        healthText.GetComponent<TextMeshProUGUI>().text = $"{hp}";

        if (hp <= 0)
        {
            StopAllCoroutines();

            DefeatGame();
        }

        // 정보창을 지속적으로 갱신
        TowerInfoUISet();
        ItemInfoUISet();

        // 보스 체력바 갱신
        if (isBoss && bossMonster != null)
        {
            bossHealthOverride.fillAmount = 1 - (bossMonster.hp / bossMonster.maxHp);
        }
    }

    public void ClickNextWave()
    {
        if (waveWaitTimeCoroutine != null)
        {
            StopCoroutine(waveWaitTimeCoroutine);
        }
        NextWave();
    }

    public void CheckNextWave()
    {
        if (nowSpawnMonsterCount == 0 && endSpawn)
        {
            endSpawn = false;
            isItemDroped = false;
            if (isBoss)
            {
                StartCoroutine(HideUI(BossHealthBar, 0f));
                isBoss = false;
            }
            waveNum++;
            
            // Victory!!
            if (waveNum >= maxWaveNum)
            {
                if (GameManager.Instance.gameDifficulty == "Easy")
                {
                    FirebaseDatabaseManager.instance.SendClearDataEasyMode((Int64)gameTimer);
                }
                else if (GameManager.Instance.gameDifficulty == "Normal")
                {
                    FirebaseDatabaseManager.instance.SendClearDataNormalMode((Int64)gameTimer);
                }

                else if (GameManager.Instance.gameDifficulty == "Hard")
                {
                    FirebaseDatabaseManager.instance.SendClearDataHardMode((Int64)gameTimer);
                }
                
                AudioManager.instance.PlaySfx(AudioManager.Sfx.gameClear);

                isGameClear = true;
                StartCoroutine(ShowPanel(gameclearPanel, 3f));
                GameResult();
                return;
            }

            if (SynergyManager.instance.roleList[1].value >= 3)
            {
                if(!isPriest)
                {
                    priestWave = waveNum;
                    isPriest = true;
                }
                else
                {
                    if(waveNum - priestWave == 5)
                    {
                        SynergyManager.instance.roleSynergy(1, false);
                        priestWave = waveNum;
                        isPriest = false;
                    }
                }
                
            }

            if(SynergyManager.instance.roleList[3].value == 6 && SynergyManager.instance.waveCount == 2)
            {
                SynergyManager.instance.synergyTowerPicker(2);
                SynergyManager.instance.waveCount = 0;
            }
            else if(SynergyManager.instance.roleList[3].value >= 4 && SynergyManager.instance.waveCount == 2)
            {
                SynergyManager.instance.synergyTowerPicker(1);
                SynergyManager.instance.waveCount = 0;
            }
            else
            {
                SynergyManager.instance.waveCount++;
            }
            int earnGold = Math.Clamp(gold / 10, 0, maxInterestRate) + giveGold;

            gold += earnGold;
            earnGoldCnt += earnGold;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.waveEndGetCoin);
            ShopManager.instance.RerollCharactor();

            waveWaitTimeCoroutine = StartCoroutine(NextWaveWaitTime());
        }
    }

    IEnumerator NextWaveWaitTime()
    {
        int time = 0;
        nextWaveClickButton.SetActive(true);
        while (time <= nextSpawnTime)
        {
            restTimeText.GetComponent<TextMeshProUGUI>().text = $"Next {nextSpawnTime - time}s";
            yield return new WaitForSeconds(1f / speedTime);

            if (!isGameOver && !isPauseGame) time++;
        }
        NextWave();
    }

    void NextWave()
    {
        waveText.GetComponent<TextMeshProUGUI>().text = $"Wave {waveNum + 1}";
        spawnMonsterCount = maxSpawnMonsterCount;

        Monster nowMonster = spawnMonsterObject[waveNum%5].GetComponent<Monster>();

        stageStartPopup.monsterImage.sprite = nowMonster.GetComponent<SpriteRenderer>().sprite;
        stageStartPopup.monsterSpeed.text = nowMonster.moveSpeed.ToString();
        if (nowMonster.armorType == ArmorType.AttackArmor)
        {
            stageStartPopup.monsterAttackArmor.text = (nowMonster.attackArmor + waveNum * 5f).ToString();
            stageStartPopup.monsterMagicArmor.text = (nowMonster.magicArmor + waveNum * 2f).ToString();
            stageStartPopup.monsterHealth.text = (nowMonster.maxHp * (1 + (waveNum / 5) * 2)).ToString();
        }
        else if (nowMonster.armorType == ArmorType.MagicArmor)
        {
            stageStartPopup.monsterAttackArmor.text = (nowMonster.attackArmor + waveNum * 2f).ToString();
            stageStartPopup.monsterMagicArmor.text = (nowMonster.magicArmor + waveNum * 5f).ToString();
            stageStartPopup.monsterHealth.text = (nowMonster.maxHp * (1 + (waveNum / 5) * 2)).ToString();
        }
        else
        {
            stageStartPopup.monsterAttackArmor.text = (nowMonster.attackArmor + waveNum * 5f).ToString();
            stageStartPopup.monsterMagicArmor.text = (nowMonster.magicArmor + waveNum * 5f).ToString();
            stageStartPopup.monsterHealth.text = (nowMonster.maxHp * (1 + waveNum / 5)).ToString();
        }

        stageStartAnim.SetBool("Show", true);
        StartCoroutine(HideUI(stageStartAnim, "Show", 5f));

        if ((waveNum + 1) % 5 == 0)
        {
            spawnMonsterCount = 1;
            isBoss = true;
            BossHealthBar.SetActive(true);
            BossPopupPanel.SetActive(true);
            StartCoroutine(HideUI(BossPopupPanel, 3f));
        }
        nowSpawnMonsterCount += spawnMonsterCount;
        startSpawn = true;
        nextWaveClickButton.SetActive(false);
    }

    IEnumerator SpawnMonster()
    {
        startSpawn = false;
        while (spawnMonsterCount > 0)
        {
            yield return new WaitForSeconds(spawnTime / speedTime);
            if (!isGameOver && !isPauseGame)
            {
                GameObject monster = Instantiate(spawnMonsterObject[waveNum%5], startPoint.transform.position, startPoint.transform.rotation, Monster.transform);
                Monster monsterLogic = monster.GetComponent<Monster>();
                monsterLogic.mosterCountNum = waveNum * maxSpawnMonsterCount + maxSpawnMonsterCount - spawnMonsterCount;
                spawnMonsterCount--;

                if (isBoss)
                {
                    bossMonster = monsterLogic;
                }
            }
        }
        endSpawn = true;
    }

    public void CreateNewDeckUnit(GameObject newObject, int idx)
    {
        GameObject createdObject = Instantiate(newObject, deckPos[idx].transform.position + Vector3.back * 2, deckPos[idx].transform.rotation, TowerParent.transform);
        BuildGrid buildGrid = deckPos[idx].GetComponent<BuildGrid>();
        buildGrid.canBuild = false;
        buildGrid.nowBuild = createdObject;

        DragAndDrop dragLogic = createdObject.GetComponent<DragAndDrop>();
        dragLogic.prebuildTarget = deckPos[idx];

        CheckRankSum(createdObject);

        Tower tower = createdObject.GetComponent<Tower>();
        SynergyManager.instance.SynergyCheck();
        SynergyManager.instance.propertySynergy((int)tower.property, false);
        SynergyManager.instance.roleSynergy((int)tower.role, false);
    }

    // 게임 정지
    public void PauseResumeGame(bool set)
    {
        if (set == true)
        {
            AudioManager.instance.PlayBgm(false);
        }
        else
        {
            AudioManager.instance.PlayBgm(true);
        }
        pauseBackgroundImg.SetActive(set);
        pausePanel.SetActive(set);
        isPauseGame = set;
    }

    // 게임에서 졌습니다.
    public void DefeatGame()
    {
        // 효과음.
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.gameOver);

        foreach(GameObject tower in towerList)
        {
            Tower tow = tower.GetComponent<Tower>();

            tow.anim.SetTrigger("Death");
        }

        isGameOver = true;
        gameoverBackground.SetActive(true);
        gameoverBackground.GetComponent<Animator>().SetBool("GameOver", true);

        if (GameManager.Instance.gameDifficulty == "Infinite")
        {
            FirebaseDatabaseManager.instance.SendClearDataInfiniteMode((Int64)gameTimer, waveNum);
        }

        GameResult();
    }

    public void ExitGame()
    {
        // SceneManager를 통해서 씬 전환하기
        LoadSceneManager.CallNextScene("Main");
        GameManager.Instance.gameDifficulty = "Main";
        AudioManager.instance.PlayBgm(false);
    }

    public void OnSettings()
    {
        SettingsUI.instance.gameObject.SetActive(true);
    }
    public void OffSettings()
    {
        SettingsUI.instance.gameObject.SetActive(false);
    }

    // 타워 합성하는 로직
    public void CheckRankSum(GameObject targetObject)
    {
        Tower targetTower = targetObject.GetComponent<Tower>();
        int targetRank = targetTower.rank;
        if (targetRank == 2)
        {
            towerList.Add(targetObject);
            return;
        }
        
        List<GameObject> targetList = new List<GameObject>();
        int rankCnt = 1;

        foreach (GameObject tower in towerList)
        {
            Tower nowTower = tower.GetComponent<Tower>();
            if (nowTower.rank == targetRank && nowTower.gameObject.name == targetObject.name)
            {
                targetList.Add(tower);
                rankCnt++;
            }
        }

        if (rankCnt == 3)
        {
            foreach (GameObject tower in targetList)
            {
                DragAndDrop dragAndDrop = tower.GetComponent<DragAndDrop>();
                if (dragAndDrop.prebuildTarget != null)
                {
                    BuildGrid buildGrid = dragAndDrop.prebuildTarget.GetComponent<BuildGrid>();
                    buildGrid.canBuild = true;
                }
                towerList.Remove(tower);

                Tower towerLogic = tower.GetComponent<Tower>();

                int i= 0, j= 0;
                while (i < targetTower.items.Length && j < towerLogic.items.Length)
                {
                    if (towerLogic.items[j].hasItem)
                    {
                        // 아이템이 있지만 합성 가능한 아이템인 경우
                        if (targetTower.items[i].hasItem)
                        {
                            ItemClass newItemClass = ItemPlusCheck(targetTower.items[i].itemClass, towerLogic.items[j].itemClass);
                            if (newItemClass != ItemClass.Empty)
                            {
                                targetTower.ItemInActive(targetTower.items[i].itemClass);
                                targetTower.items[i].itemClass = newItemClass;
                                targetTower.ItemActive(newItemClass);

                                towerLogic.items[j].itemClass = ItemClass.Empty;
                                towerLogic.items[j].hasItem = false;
                                i++;
                            }
                        }
                        // 아이템을 넣는 공간이 있을 경우
                        else
                        {
                            targetTower.items[i].hasItem = true;
                            targetTower.items[i].itemClass = towerLogic.items[j].itemClass;
                            targetTower.ItemActive(targetTower.items[i].itemClass);

                            towerLogic.items[j].itemClass = ItemClass.Empty;
                            towerLogic.items[j].hasItem = false;
                            i++;
                        }
                    }
                    j++;
                }
                // 합성된 타워에 옮기고 나머지 아이템은 인벤토리에 옮기기
                MoveItemToInventory(towerLogic);

                Destroy(tower);
            }
            targetTower.rank++;
            CheckRankSum(targetObject);
        }
        else
        {
            towerList.Add(targetObject);
        }
    }

    public void DropItem()
    {
        int randomItemIdx = UnityEngine.Random.Range(1, 5); // drop되는 아이템은 일반 아이템까지만으로 허용
        int inventoryIdx;
        for (inventoryIdx = 0; inventoryIdx < ShopManager.instance.itemInventory.Length; inventoryIdx++)
        {
            Inventory inventory = ShopManager.instance.itemInventory[inventoryIdx].GetComponent<Inventory>();
            if (inventory != null && !inventory.hasItem)
            {
                Item newItem = Instantiate(itemObjectList[randomItemIdx], inventory.GetComponent<RectTransform>().position, Quaternion.identity, itemInventoryPanel.transform);
                //newItem.MoveToCenter(inventory.GetComponent<RectTransform>().position);
                inventory.hasItem = true;
                inventory.itemClass = newItem.itemClass;
                newItem.preInventory = inventory;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.collectItem);
                ItemObtainPanel.SetActive(true);
                StartCoroutine(HideUI(ItemObtainPanel, 2f));
                itemDropCnt++;
                return;
            }
        }
        // 아이템을 더 이상 담을 수 없다고 알려주기?
    }

    public void MoveItemToInventory(Tower tower)
    {
        int i = 0, j = 0;
        while (i < ShopManager.instance.itemInventory.Length && j < tower.items.Length)
        {
            Inventory itemInventory = ShopManager.instance.itemInventory[i].gameObject.GetComponent<Inventory>();

            // 인벤토리가 칸이 없으면 다음칸으로
            if (itemInventory.hasItem)
            {
                i++;
                continue;
            }

            // 타워에 아이템이 비어있으면 다음칸으로
            if (tower.items[j].itemClass == ItemClass.Empty)
            {
                j++;
                continue;
            }

            // 아이템 옮기기
            Item moveItem = Instantiate(itemObjectList[(int)tower.items[j].itemClass], itemInventory.GetComponent<RectTransform>().position, Quaternion.identity, itemInventoryPanel.transform);
            //moveItem.MoveToCenter(itemInventory.GetComponent<RectTransform>().position);
            itemInventory.hasItem = true;
            itemInventory.itemClass = moveItem.itemClass;
            moveItem.preInventory = itemInventory;

            // 각각 다음칸으로 이동
            i++;
            j++;
        }
    }

    public ItemClass ItemPlusCheck(ItemClass targetItemClass, ItemClass mixItemClass)
    {
        ItemClass returnItemClass = ItemClass.Empty;
        switch (targetItemClass)
        {
            case ItemClass.Attack:
                if (mixItemClass == ItemClass.Attack)
                {
                    returnItemClass = ItemClass.APA;
                }
                else if (mixItemClass == ItemClass.Magic)
                {
                    returnItemClass = ItemClass.APM;
                }
                else if (mixItemClass == ItemClass.Speed)
                {
                    returnItemClass = ItemClass.APS;
                }
                else if (mixItemClass == ItemClass.Critical)
                {
                    returnItemClass = ItemClass.APC;
                }
                break;
            case ItemClass.Magic:
                if (mixItemClass == ItemClass.Attack)
                {
                    returnItemClass = ItemClass.APM;
                }
                else if (mixItemClass == ItemClass.Magic)
                {
                    returnItemClass = ItemClass.MPM;
                }
                else if (mixItemClass == ItemClass.Speed)
                {
                    returnItemClass = ItemClass.MPS;
                }
                else if (mixItemClass == ItemClass.Critical)
                {
                    returnItemClass = ItemClass.MPC;
                }
                break;
            case ItemClass.Speed:
                if (mixItemClass == ItemClass.Attack)
                {
                    returnItemClass = ItemClass.APS;
                }
                else if (mixItemClass == ItemClass.Magic)
                {
                    returnItemClass = ItemClass.MPS;
                }
                else if (mixItemClass == ItemClass.Speed)
                {
                    returnItemClass = ItemClass.SPS;
                }
                else if (mixItemClass == ItemClass.Critical)
                {
                    returnItemClass = ItemClass.SPC;
                }
                break;
            case ItemClass.Critical:
                if (mixItemClass == ItemClass.Attack)
                {
                    returnItemClass = ItemClass.APC;
                }
                else if (mixItemClass == ItemClass.Magic)
                {
                    returnItemClass = ItemClass.MPC;
                }
                else if (mixItemClass == ItemClass.Speed)
                {
                    returnItemClass = ItemClass.SPC;
                }
                else if (mixItemClass == ItemClass.Critical)
                {
                    returnItemClass = ItemClass.CPC;
                }
                break;
            default: break;
        }
        return returnItemClass;
    }

    public void OpenItemInfoUI(Item targetItem)
    {
        targetUIItem = targetItem;

        itemInfoUIAnimator.SetBool("ShowPanel", true);
        towerInfoUIAnimator.SetBool("ShowPanel", false);
    }

    public void CloseItemInfoUI()
    {
        itemInfoUIAnimator.SetBool("ShowPanel", false);
    }

    private void ItemInfoUISet()
    {
        if (targetUIItem == null) return;

        ItemInfoPanel infoPanel = itemInfoPanel.GetComponent<ItemInfoPanel>();
        infoPanel.itemImage.sprite = targetUIItem.image.sprite;
        infoPanel.itemName.text = targetUIItem.itemName;
        infoPanel.itemDescription.text = targetUIItem.itemDescription;
        infoPanel.itemParameter.text = targetUIItem.itemParameter;

        if (targetUIItem.itemClass == ItemClass.Attack || targetUIItem.itemClass == ItemClass.Magic ||
            targetUIItem.itemClass == ItemClass.Speed || targetUIItem.itemClass == ItemClass.Critical)
        {
            itemCombinePanel.SetActive(true);
            for (int i = 0; i < infoPanel.combines.Length; i++)
            {
                Combine combine = infoPanel.combines[i].GetComponent<Combine>();
                combine.material1.sprite = targetUIItem.image.sprite;
                combine.inventory1.itemClass = targetUIItem.itemClass;
                combine.material2.sprite = itemObjectList[i + 1].image.sprite;
                combine.inventory2.itemClass = itemObjectList[i + 1].itemClass;
                Item resultItem = itemObjectList[(int)ItemPlusCheck(targetUIItem.itemClass, itemObjectList[i + 1].itemClass)];
                combine.result.sprite = resultItem.image.sprite;
                combine.inventoryResult.itemClass = resultItem.itemClass;
            }
        }
        else
        {
            itemCombinePanel.SetActive(false);
        }
    }

    public void OpenTowerInfoUI(Tower targetTower)
    {
        targetUITower = targetTower;

        towerInfoUIAnimator.SetBool("ShowPanel", true);
        itemInfoUIAnimator.SetBool("ShowPanel", false);
    }

    public void CloseTowerInfoUI()
    {
        towerInfoUIAnimator.SetBool("ShowPanel", false);
    }

    private void TowerInfoUISet()
    {
        if (targetUITower == null) return;

        TowerInfoPanel infoPanel = towerInfoPanel.GetComponent<TowerInfoPanel>();
        infoPanel.towerImage.sprite = targetUITower.GetComponent<SpriteRenderer>().sprite;
        infoPanel.towerName.text = targetUITower.towerName;
        infoPanel.towerDescription.text = targetUITower.towerDescription;
        infoPanel.towerProperty.sprite = ShopManager.instance.propertySprite[(int)targetUITower.property];
        infoPanel.towerRole.sprite = ShopManager.instance.roleSprite[(int)targetUITower.role];
        infoPanel.towerShotType.sprite = ShopManager.instance.shotTypeSprites[(int)targetUITower.ShotType];
        if (targetUITower.towerCost >= 6)
        {
            infoPanel.towerCost.text = "-";
        }
        else
        {
            infoPanel.towerCost.text = targetUITower.towerCost.ToString();
        }
        infoPanel.towerAttackType.sprite = attackTypeSprite[(int)targetUITower.attackType];
        infoPanel.towerDamage.text = $"    {((targetUITower.attackType == AttackType.Attack) ? "물리" : "마법")} 데미지 " + Math.Round(targetUITower.nowDamage,2).ToString("F2");
        infoPanel.towerAttackSpeed.text = "    공격속도 1초에 " + Math.Round(1 / targetUITower.nowFireDealy, 2).ToString("F2") + "번";
        infoPanel.towerCriticalRate.text = "    크리티컬 확률 " + (targetUITower.criticalRate + targetUITower.itemCriticalRate).ToString() + "%";
        infoPanel.towerCriticalDamage.text = "    크리티컬 데미지 " + (targetUITower.criticalDamage * 100).ToString() + "%";
        for (int i = 0; i < targetUITower.items.Length; i++)
        {
            if (targetUITower.items[i] == null)
            {
                infoPanel.towerInventory[i].gameObject.SetActive(false);
            }
            else
            {
                infoPanel.towerInventory[i].gameObject.SetActive(true);
                infoPanel.towerInventory[i].itemClass = targetUITower.items[i].itemClass;
                infoPanel.towerInventory[i].image.sprite = itemObjectList[(int)targetUITower.items[i].itemClass].image.sprite;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (targetUITower.rank >= i)
            {
                infoPanel.rankStar[i].SetActive(true);
            }
            else
            {
                infoPanel.rankStar[i].SetActive(false);
            }
        }
    }

    public void ApplyLightningAttack(GameObject firstMonster, float damage, AttackType attackType, float attackArmorDecrease, float magicArmorDecrease)
    {
        int monsterCnt = 0;
        float[] damageRate;
        if (SynergyManager.instance.propertyList[2].value == 4)
        {
            monsterCnt = 3;
            damageRate =  new float[]{ 0, 0.5f, 0.25f, 0.125f };
        }
        else
        {
            monsterCnt = 2;
            damageRate = new float[] { 0, 0.25f, 0.125f };
        }

        GameObject nextTarget = firstMonster;
        List<GameObject> attackedList = new List<GameObject>
        {
            firstMonster
        };

        while (monsterCnt > 0)
        {
            Collider2D[] colliderResults = Physics2D.OverlapCircleAll(nextTarget.transform.position, 10f);
            if (colliderResults.Length > 0)
            {
                foreach (Collider2D col in colliderResults)
                {
                    Monster monster = col.gameObject.GetComponent<Monster>();
                    if (monster != null && !attackedList.Contains(col.gameObject) && monster.hp > 0)
                    {
                        attackedList.Add(col.gameObject);
                        nextTarget = col.gameObject;
                        break;
                    }
                }
            }
            else
            {
                break;
            }
            monsterCnt--;
        }

        StartCoroutine(LightningDamage(attackedList, damage, damageRate, attackType, attackArmorDecrease, magicArmorDecrease));
    }

    IEnumerator LightningDamage(List<GameObject> attackedList, float damage, float[] damageRate, AttackType attackType, float attackArmorDecrease, float magicArmorDecrease)
    {
        for (int i = 0; i < attackedList.Count; i++)
        {
            if (attackedList[i] != null)
            {
                Monster monster = attackedList[i].GetComponent<Monster>();
                if (monster != null)
                {
                    monster.OnDamaged(damage * damageRate[i], attackType, attackArmorDecrease, magicArmorDecrease);
                    totalDamage[2] += damage * damageRate[i];

                    if (i - 1 >= 0)
                    {
                        Vector3 position = (attackedList[i].transform.position + attackedList[i - 1].transform.position) / 2;
                        Vector3 direction = attackedList[i].transform.position - attackedList[i - 1].transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x);

                        GameObject thunderEffect = Instantiate(thunderObject, position, Quaternion.identity);
                        ParticleSystem.MainModule particle = thunderEffect.GetComponent<ParticleSystem>().main;

                        particle.startRotation = -angle;
                        StartCoroutine(DestoryEffectTime(thunderEffect, 2f));
                    }

                    yield return new WaitForSeconds(0.25f / speedTime);
                }
            }
        }
    }
    public void UpdateTimer()
    {
        gameTimer += Time.deltaTime * 100 * speedTime;
        int second = (int)(gameTimer / 100) % 60;
        int milsec = (int)(gameTimer % 100);
        int minute = (int)(gameTimer / 6000);

        timerText.GetComponent<TextMeshProUGUI>().text = $"{minute.ToString("00")}:{second.ToString("00")}:{milsec.ToString("00")}";
    }

    /* set = 0 : 모두 가림, set = 1 : 지을 수 있는 공간 표시 */
    public void BuildGridSet(float set)
    {
        for(int i = 0; i < canbuildObjects.Length; i++)
        {
            BuildGrid buildGrid = canbuildObjects[i].GetComponent<BuildGrid>();
            SpriteRenderer spriteRenderer = canbuildObjects[i].GetComponent<SpriteRenderer>();
            if (buildGrid.canBuild)
            {
                spriteRenderer.color = new Color(1, 1, 1, set);
            }
        }
    }

    public IEnumerator CreateEffectTime(GameObject[] effectObjects, float[] times, Transform targetTransform, Vector3[] fixPos)
    {
        yield return null;
        int cnt = 0;
        while (cnt < effectObjects.Length)
        {
            GameObject effectObject = Instantiate(effectObjects[cnt], targetTransform.position + fixPos[cnt], Quaternion.identity);
            yield return new WaitForSeconds(times[cnt]);
            cnt++;
            StartCoroutine(DestoryEffectTime(effectObject, 2f));
        }
    }

    public IEnumerator DestoryEffectTime(GameObject targetObject, float time)
    {
        yield return null;
        if (targetObject.TryGetComponent<ParticleSystem>(out var particle))
        {
            while (particle.isPlaying)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(time);
        }
        Destroy(targetObject);
    }

    public IEnumerator ShowPanel(GameObject panel, float time)
    {
        yield return new WaitForSeconds(time);
        panel.SetActive(true);
    }

    public IEnumerator HideUI(GameObject panel, float time)
    {
        yield return new WaitForSeconds(time);
        panel.SetActive(false);
    }

    public IEnumerator HideUI(Animator anim, string animation, float time)
    {
        yield return new WaitForSeconds(time);
        anim.SetBool(animation, false);
    }

    void GameResult()
    {
        resultWaveCntText.text = $"Wave {waveNum}";
        
        int second = (int)(gameTimer / 100) % 60;
        int milsec = (int)(gameTimer % 100);
        int minute = (int)(gameTimer / 6000);
        resultTimeText.text = $"{minute:00}:{second:00}:{milsec:00}";

        resultGoldText.text = $"{earnGoldCnt} (+{((earnGoldCnt - maxWaveNum * giveGold) > 0? (earnGoldCnt - maxWaveNum * giveGold) : 0)}) 원";
        resultRerollCntText.text = $"{rerollCnt} 번";

        string[] names = { "불", "얼음", "전기", "바람", "독", "숲", "스페셜", "도둑", "사제", "기사", "광대", "모험가", "스페셜" };
        float totalDamageSum = 0;
        for (int i = 0; i < 6; i++)
        {
            synergyDamages[i].text = $"{names[i]} : {totalDamage[i].ToString("F2")}";
            totalDamageSum += totalDamage[i];
        }
        for (int i = 7; i < names.Length-1; i++)
        {
            synergyDamages[i - 1].text = $"{names[i]} : {totalDamage[i].ToString("F2")}";
            totalDamageSum += totalDamage[i];
        }
        synergyDamages[11].text = $"{names[12]} : {totalDamage[6].ToString("F2")}";
        totalDamageSum += totalDamage[6];

        totalDamageText.text = $"{totalDamageSum.ToString("F2")}";
        itemDropText.text = $"{itemDropCnt} 번";
    }

    public void ShowResult()
    {
        StartCoroutine(ShowPanel(gameResultPanel, 0f));
        StartCoroutine(HideUI(gameclearPanel, 0f));
    }

    public void ClickTimeTwice()
    {
        if (speedBtnActive.activeSelf)
        {
            speedTime = 1f;
            speedBtnActive.SetActive(false);
        }
        else
        {
            speedTime = 2f;
            speedBtnActive.SetActive(true);
        }
    }
}
