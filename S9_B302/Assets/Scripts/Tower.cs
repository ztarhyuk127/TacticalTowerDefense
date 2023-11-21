using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // 컴포넌트 정보
    DragAndDrop dragLogic;

    public Animator anim;

    [Header("타워 공격 정보")]
    // 타워 데미지 정보
    public float towerDamage = 3.0f;
    public float nowDamage;
    // 시너지가 변화시킬 공격력 계수
    public float damageRate = 1.0f;
    // 아이템이 변화시킬 공격력 계수
    public float itemDamageRate = 1.0f;

    // 치명타 확률
    public float criticalRate = 10.0f;
    public float criticalDamage = 1.5f;
    // 아이템이 변화시킬 치명타 계수
    public float itemCriticalRate;

    // 타워 공격 속도 정보
    public float fireDelay = 2.0f;
    public float nowFireDealy;
    // 시너지가 변화시킬 공격 속도 계수
    public float delayRate = 1.0f;
    // 아이템이 변화시킬 공격 속도 계수
    public float itemDelayRate = 1.0f;
    [Tooltip("타워가 공격할 수 있는 범위값")]
    public float normalfireRadius;
    public float fireRadius = 15f;
    private float fireTime = 0.0f;
    [Tooltip("타워가 배치되었는지 확인하는 변수")]
    public bool isTower;
    [Tooltip("타워 등급 설정 변수. rank=0은 1성, rank=1은 2성, rank=2는 3성이다.")]
    public int rank = 0;
    private List<GameObject> targets = new List<GameObject>();
    public float attackArmorDecrease = 1.0f;
    public float magicArmorDecrease = 1.0f;

    [Header("타워 공격 프리팹 설정")]
    [Tooltip("총알 프리팹 설정")]
    public GameObject bullet = null;
    [Tooltip("즉발형 이펙트 프리팹 설정")]
    public GameObject[] instantEffect;
    [Tooltip("즉발형 이펙트 프리팹 별 시간 설정")]
    public float[] instantEffectTime;
    [Tooltip("즉발형 이펙트 프리팹 별 위치 설정")]
    public Vector3[] instantEffectPosition;
    [Tooltip("범위형 이펙트 프리팹 설정")]
    public GameObject areaEffectPrefab;

    [Header("타워 공격 속성 지정")]
    public PlayManager.Property property;
    public PlayManager.Role role;
    public PlayManager.AttackType attackType;
    public PlayManager.ShotType ShotType;
    [Tooltip("수를 늘릴 경우 해당 공격은 멀티샷으로 들어감")]
    public int maxShotCnt = 1;
    [Tooltip("타워 구매시 드는 비용")]
    public int towerCost;

    public Inventory[] items;

    [Header("타워를 구분하기 위한 변수")]
    public int towerNum;

    [Header("아이템 관련 변수")]
    public bool isAPM = false;
    public int attackCount;
    public bool isAPS = false;
    public bool isMPS = false;

    [Header("타워 정보창 관련 변수")]
    public string towerName;
    [TextArea(3,10)]
    public string towerDescription;
    public GameObject[] towerRankSprite;

    private float adventurerRate = 1.0f;

    void Awake()
    {
        isTower = false;
        dragLogic = GetComponent<DragAndDrop>();

        anim = GetComponent<Animator>();

        nowDamage = towerDamage * (damageRate + itemDamageRate - 1);
        nowFireDealy = fireDelay * delayRate * itemDelayRate;
        fireRadius = normalfireRadius;
    }

    private void Update()
    {
        if (PlayManager.instance.isPauseGame || PlayManager.instance.isGameOver)
        {
            return;
        }

        fireTime += Time.deltaTime * PlayManager.instance.speedTime;
        anim.speed = 1f * PlayManager.instance.speedTime;

        if (fireTime > nowFireDealy && isTower && !dragLogic.isDragging)
        {
            targets.Clear();
            Collider2D[] colliderResults = Physics2D.OverlapCircleAll(transform.position, fireRadius);
            if (colliderResults.Length > 0)
            {
                foreach (Collider2D col in colliderResults)
                {
                    Monster monster = col.GetComponent<Monster>();
                    if (monster != null && !targets.Contains(col.gameObject))
                    {
                        targets.Add(col.gameObject);
                    }
                }
            }

            targets.Sort(compare1);
            if(targets.Count > 0)
            {
                fireTime = 0.0f;
                int cnt = 0;
                int shotCnt = 0;
                bool areaAttacked = false; // 범위공격 캐릭터가 공격했는지 판단하는 변수
                while (cnt + shotCnt < targets.Count && shotCnt < maxShotCnt)
                {
                    // 공격에 성공하는 조건문
                    if (targets[cnt + shotCnt].GetComponent<Monster>().hp > 0)
                    {
                        // 치명타
                        float randomValue = Random.Range(0.0f, 100.0f);
                        float damage = nowDamage;
                        string animationName = "Attack";
                        if (randomValue <= criticalRate + itemCriticalRate)
                        {
                            damage *= criticalDamage;
                            animationName = "Critical";
                            PlayManager.instance.criticalCnt++;
                        }
                        else
                        {
                            PlayManager.instance.normalCnt++;
                        }

                        // attack + Speed 아이템 들고 있다면 작동
                        if (isAPS)
                        {
                            damage += targets[cnt + shotCnt].GetComponent<Monster>().maxHp * 0.01f;
                        }

                        // attack + magic 아이템 들고 있다면 작동
                        if (isAPM)
                        {
                            if (attackCount < 100)
                            {
                                attackCount++;
                            }
                            else
                            {
                                PlayManager.instance.gold += 5;
                                PlayManager.instance.earnGoldCnt += 5;
                                attackCount = 0;
                            }
                        }
                        anim.SetTrigger(animationName);

                        PlayManager.instance.totalDamage[(int)property] += damage;
                        PlayManager.instance.totalDamage[(int)role + 7] += damage;

                        switch (ShotType)
                        {
                            case PlayManager.ShotType.Bullet:
                                CreateBullet(targets[cnt + shotCnt], damage);
                                break;
                            case PlayManager.ShotType.Instant:
                                InstantDamage(targets[cnt + shotCnt], damage);
                                break;
                            case PlayManager.ShotType.Area:
                                AreaAttack(targets, damage);
                                areaAttacked = true;
                                break;
                            case PlayManager.ShotType.Dot:
                                break;
                        }

                        // 모험가 시너지일 때 데미지 상승 로직
                        //if ((int)role == 4)
                        //{
                        //    if (SynergyManager.instance.roleList[4].value == 6)
                        //    {
                        //        nowDamage += towerDamage * 0.005f;
                        //    }
                        //    else if (SynergyManager.instance.roleList[4].value >= 3)
                        //    {
                        //        nowDamage += towerDamage * 0.0025f;
                        //    }
                        //}

                        // 모험가 시너지일 때 데미지 상승 로직
                        if ((int)role == 4)
                        {
                            if (SynergyManager.instance.roleList[4].value == 6)
                            {
                                adventurerRate += 0.005f;
                            }
                            else if (SynergyManager.instance.roleList[4].value >= 3)
                            {
                                adventurerRate += 0.0025f;
                            }
                        }

                        // 아이템 장착 시 공격속도 상승 로직
                        if (isMPS)
                        {
                            nowFireDealy = (nowFireDealy * 0.98f >= fireDelay * 0.4f) ? nowFireDealy * 0.98f : fireDelay * 0.4f;
                        }

                        shotCnt++;

                        if (ShotType == PlayManager.ShotType.Area && areaAttacked)
                        {
                            break;
                        }
                    }
                    else
                    {
                        cnt++;
                    }
                }
            }
        }
        // 웨이브가 종료 되면 갱신될 계수 복구
        if(PlayManager.instance.nowSpawnMonsterCount == 0)
        {
            nowDamage = towerDamage * (damageRate + itemDamageRate - 1);
            nowFireDealy = (fireDelay * delayRate * itemDelayRate >= fireDelay * 0.4f)
                        ? fireDelay * delayRate * itemDelayRate : fireDelay * 0.4f;

            adventurerRate = 1.0f;
        }

        // transform.localScale = Vector3.one * (2f + rank * 0.5f);

        for (int i = 0; i < towerRankSprite.Length; i++)
        {
            if (PlayManager.instance.rankActive[rank][i] == 1)
            {
                towerRankSprite[i].SetActive(true);
            }
            else
            {
                towerRankSprite[i].SetActive(false);
            }
        }

        // 타워의 데미지, 공격속도 값 등을 지속적으로 업데이트 한다.
        UpdateDamage();
        // UpdateDelay();
    }

    // sort 작업을 위한 함수
    int compare1(GameObject a, GameObject b)
    {
        Monster monsterA = a.GetComponent<Monster>();
        Monster monsterB = b.GetComponent<Monster>();
        return monsterA.mosterCountNum < monsterB.mosterCountNum? -1 : 1;
    }

    void CreateBullet(GameObject targetObject, float damage)
    {
        GameObject newBullet = Instantiate(bullet, transform.position + Vector3.back, Quaternion.identity);
        Bullet bulletLogic = newBullet.GetComponent<Bullet>();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.attack);
        bulletLogic.bulletDamage = damage;

        bulletLogic.targetObject = targetObject;
        bulletLogic.targetPosition = targetObject.transform.position;
        bulletLogic.attackType = attackType;
        bulletLogic.transform.localScale = new Vector3(0.5f, 0.5f);
        bulletLogic.attackArmorDecrease = attackArmorDecrease;
        bulletLogic.magicArmorDecrease = magicArmorDecrease;

        if (property == PlayManager.Property.Electro && SynergyManager.instance.propertyList[2].value >= 2)
        {
            bulletLogic.isLightning = true;
        }
    }

    public void InstantDamage(GameObject targetObject, float damage)
    {
        Monster monster = targetObject.GetComponent<Monster>();
        if (monster != null)
        {
            monster.OnDamaged(damage, attackType, attackArmorDecrease, magicArmorDecrease);

            if (property == PlayManager.Property.Electro && SynergyManager.instance.propertyList[2].value >= 2)
            {
                PlayManager.instance.ApplyLightningAttack(targetObject, damage, attackType, attackArmorDecrease, magicArmorDecrease);
            }
            StartCoroutine(PlayManager.instance.CreateEffectTime(instantEffect, instantEffectTime, monster.transform, instantEffectPosition));
        }
    }

    public void ItemActive(PlayManager.ItemClass targetItemClass)
    {
        switch (targetItemClass)
        {
            case PlayManager.ItemClass.Attack:
                if((int)attackType == 0)
                {
                    itemDamageRate += 0.1f;
                    nowDamage = towerDamage * (damageRate + itemDamageRate - 1);
                }
                break;
            case PlayManager.ItemClass.Magic:
                if((int)attackType == 1)
                {
                    itemDamageRate += 0.1f;
                    nowDamage = towerDamage * (damageRate + itemDamageRate - 1);
                }
                break;
            case PlayManager.ItemClass.Speed:
                itemDelayRate -= 0.1f;

                nowFireDealy = (fireDelay * delayRate * itemDelayRate >= fireDelay * 0.4f) 
                    ? fireDelay * delayRate * itemDelayRate : fireDelay * 0.4f;
                break;
            case PlayManager.ItemClass.Critical:
                itemCriticalRate += 10.0f;
                break;
            case PlayManager.ItemClass.APA:
                if ((int)attackType == 0)
                {
                    itemDamageRate += 0.25f;
                    nowDamage = towerDamage * (damageRate + itemDamageRate - 1);
                    // 방깎 적용
                    attackArmorDecrease -= 0.20f;
                }
                break;
            case PlayManager.ItemClass.APM:
                isAPM = true;

                itemDamageRate += 0.15f;
                nowDamage = towerDamage * (damageRate + itemDamageRate - 1);
                break;
            case PlayManager.ItemClass.APS:
                isAPS = true;

                itemDamageRate += 0.15f;
                nowDamage = towerDamage * (damageRate + itemDamageRate - 1);

                itemDelayRate -= 0.15f;

                nowFireDealy = (fireDelay * delayRate * itemDelayRate >= fireDelay * 0.4f)
                    ? fireDelay * delayRate * itemDelayRate : fireDelay * 0.4f;
                break;
            case PlayManager.ItemClass.APC:
                itemCriticalRate += 20.0f;
                criticalDamage += 0.3f; 
                break;
            case PlayManager.ItemClass.MPM:
                if ((int)attackType == 1)
                {
                    itemDamageRate += 0.25f;
                    nowDamage = towerDamage * (damageRate + itemDamageRate - 1);
                    // 마깎 적용
                    magicArmorDecrease -= 0.20f;
                }
                break;
            case PlayManager.ItemClass.MPS:
                isMPS = true;
                if ((int)attackType == 1)
                {
                    itemDamageRate += 0.2f;
                    nowDamage = towerDamage * (damageRate + itemDamageRate - 1);
                }

                itemDelayRate -= 0.1f;

                nowFireDealy = (fireDelay * delayRate * itemDelayRate >= fireDelay * 0.4f)
                    ? fireDelay * delayRate * itemDelayRate : fireDelay * 0.4f;
                break;
            case PlayManager.ItemClass.MPC:
                itemCriticalRate += 15.0f;
                criticalDamage += 0.35f;
                break;
            case PlayManager.ItemClass.SPS:
                itemDelayRate -= 0.25f;

                nowFireDealy = (fireDelay * delayRate * itemDelayRate >= fireDelay * 0.4f)
                    ? fireDelay * delayRate * itemDelayRate : fireDelay * 0.4f;


                fireRadius *= 2.0f;

                if (fireRadius > normalfireRadius * 2.0f)
                {
                    fireRadius = normalfireRadius * 2.0f;
                }
                break;
            case PlayManager.ItemClass.SPC:
                maxShotCnt = 3;
                itemCriticalRate += 12.0f;
                itemDelayRate -= 0.12f;

                nowFireDealy = (fireDelay * delayRate * itemDelayRate >= fireDelay * 0.4f)
                    ? fireDelay * delayRate * itemDelayRate : fireDelay * 0.4f;

                break;
            case PlayManager.ItemClass.CPC:
                itemCriticalRate = 100.0f;

                criticalDamage -= 0.25f;
                break;

        }
    }

    public void ItemInActive(PlayManager.ItemClass targetItemClass)
    {
        switch (targetItemClass)
        {
            case PlayManager.ItemClass.Attack:
                if ((int)attackType == 0)
                {
                    itemDamageRate -= 0.1f;
                    nowDamage = towerDamage * (damageRate + itemDamageRate - 1);
                }
                break;
            case PlayManager.ItemClass.Magic:
                if ((int)attackType == 1)
                {
                    itemDamageRate -= 0.1f;
                    nowDamage = towerDamage * (damageRate + itemDamageRate - 1);
                }
                break;
            case PlayManager.ItemClass.Speed:
                itemDelayRate += 0.1f;

                nowFireDealy = (fireDelay * delayRate * itemDelayRate >= fireDelay * 0.4f)
                    ? fireDelay * delayRate * itemDelayRate : fireDelay * 0.4f;
                break;
            case PlayManager.ItemClass.Critical:
                itemCriticalRate -= 10.0f;
                break;
            
        }
    }

    // 최종 데미지 산출 함수
    void UpdateDamage()
    {
        float rankDamage = towerDamage * Mathf.Pow(1.5f, rank);
        
        nowDamage = rankDamage * (itemDamageRate - 1 + damageRate + adventurerRate - 1);
    }

    void UpdateDelay()
    {
        nowFireDealy = (fireDelay * delayRate * itemDelayRate >= fireDelay * 0.4f)
                    ? fireDelay * delayRate * itemDelayRate : fireDelay * 0.4f;
    }

    void AreaAttack(List<GameObject> targets, float damage)
    {
        GameObject effect = Instantiate(areaEffectPrefab, transform.position, Quaternion.identity);
        StartCoroutine(PlayManager.instance.DestoryEffectTime(effect, 2f));

        foreach (GameObject target in targets)
        {
            Monster targetMonster = target.GetComponent<Monster>();
            targetMonster.OnDamaged(damage, attackType, attackArmorDecrease, magicArmorDecrease);
        }
    }
}