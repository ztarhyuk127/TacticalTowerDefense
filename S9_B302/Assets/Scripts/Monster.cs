using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Monster : MonoBehaviour
{

    Animator monAnim;

    // 맵 상의 웨이포인트 변수
    private GameObject[] WayPoints;
    private int nextPointIdx = 0;
    private Vector3 nextDirection = Vector3.zero;

    [Header("몬스터 정보")]
    [Tooltip("몬스터 최대 체력을 조절하는 변수")]
    public float maxHp = 0f;
    [Tooltip("몬스터 현재 체력을 나타내는 변수")]
    public float hp;
    [Tooltip("몬스터 이동속도를 조절하는 변수")]
    public float moveSpeed = 5.0f;
    [Tooltip("몬스터 물리 방어력을 나타내는 변수")]
    public int attackArmor = 0;
    [Tooltip("몬스터 마법 방어력을 나타내는 변수")]
    public int magicArmor = 0;

    [Tooltip("몬스터가 해당 웨이브의 몇번째인지 확인하는 변수")]
    public int mosterCountNum = 0;
    [Tooltip("몬스터가 마지막 지점에 도달했을 때 플레이어 체력이 감소하는 양")]
    public int monsterDamage = 1;

    [Tooltip("몬스터의 방어 타입")]
    public PlayManager.ArmorType armorType;

    private SpriteRenderer monsterRenderer;
    private bool isDead = false;

    void Awake()
    {

        monAnim = GetComponent<Animator>();

        // 씬 내에서 웨이포인트들을 찾는다.
        WayPoints = PlayManager.instance.WayPoints;

        // 웨이브 수에 따라서 몬스터를 강화한다.
        if(armorType == PlayManager.ArmorType.AttackArmor)
        {
            attackArmor = attackArmor + (int)(PlayManager.instance.waveNum * 5f);
            magicArmor = magicArmor + (int)(PlayManager.instance.waveNum * 2f);
            maxHp = maxHp * (1 + (PlayManager.instance.waveNum / 5) * 2);
        }
        else if(armorType == PlayManager.ArmorType.MagicArmor)
        {
            attackArmor = attackArmor + (int)(PlayManager.instance.waveNum * 2f);
            magicArmor = magicArmor + (int)(PlayManager.instance.waveNum * 5f);
            maxHp = maxHp * (1 + (PlayManager.instance.waveNum / 5) * 2);
        }
        else
        {
            attackArmor = attackArmor + (int)(PlayManager.instance.waveNum * 5f);
            magicArmor = magicArmor + (int)(PlayManager.instance.waveNum * 5f);
            maxHp = maxHp * (1 + PlayManager.instance.waveNum / 5);
        }
        

        // 몬스터의 체력을 최대로 채운다.
        // 기사 시너지 활성화 시 체력을 변경
        if (SynergyManager.instance.roleList[2].value == 7)
        {
            hp = maxHp * 0.9f;
            PlayManager.instance.totalDamage[8] += maxHp * 0.1f;
        }
        else if (SynergyManager.instance.roleList[2].value >= 4 && armorType == PlayManager.ArmorType.BossArmor)
        {
            hp = maxHp * 0.9f;
            PlayManager.instance.totalDamage[8] += maxHp * 0.1f;
        }
        else
        {
            hp = maxHp;
        }
        PlayManager.instance.monsterHP += maxHp;
        monsterRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        nextDirection = (WayPoints[nextPointIdx].transform.position - transform.position).normalized;
        // 시작 전 다음 웨이포인트로 지정
        NextWaypoint();
    }

    void Update()
    {
        if (PlayManager.instance.isPauseGame || PlayManager.instance.isGameOver || isDead)
        {
            return;
        }

        // 일정 방향으로 몬스터 움직임
        nextDirection = (WayPoints[nextPointIdx].transform.position - transform.position).normalized;
        transform.position += nextDirection * moveSpeed * SynergyManager.instance.speedRate * Time.deltaTime * PlayManager.instance.speedTime;
        

        // 웨이포인트 
        float distance = Vector2.Distance(transform.position, WayPoints[nextPointIdx].transform.position);
        if (Mathf.Abs(distance) < 1.0f)
        {
            NextWaypoint();
        }

        // 체력이 다 떨어지면 죽음처리.
        if (hp <= 0f)
        {
            float chance = Random.Range(0f, 100f);
            if(PlayManager.instance.waveNum <= 30 && chance <= PlayManager.instance.itemDropChance)
            {
                PlayManager.instance.DropItem();
            }
            else if(PlayManager.instance.waveNum > 30 && chance <= PlayManager.instance.itemDropChance / 10)
            {
                PlayManager.instance.DropItem();
            }
            /*
            if (!PlayManager.instance.isItemDroped && GameManager.Instance.gameDifficulty != "Infinite")
            {
                float chance = Random.Range(0f, 100f);
                if (chance < PlayManager.instance.itemDropChance)
                {
                    PlayManager.instance.isItemDroped = true;
                    PlayManager.instance.DropItem();
                }
            }
            else if(GameManager.Instance.gameDifficulty == "Infinite")
            {
                
            }
            */

            PlayManager.instance.nowSpawnMonsterCount--;
            isDead = true;
            monAnim.SetTrigger("Death");
            StartCoroutine(MonsterDestory(2.5f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Poison"))
        {
            InvokeRepeating("poisonDamage", 0.0f, 1.0f / PlayManager.instance.speedTime);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Poison"))
        {
            CancelInvoke("poisonDamage");
        }
    }


    void NextWaypoint()
    {
        nextPointIdx++;
        if (nextPointIdx < WayPoints.Length)
        {
            nextDirection = (WayPoints[nextPointIdx].transform.position - transform.position).normalized;
            if (nextDirection.x > 0.01f)
            {
                monsterRenderer.flipX = true;
            }
            else if (nextDirection.x < -0.01f)
            {
                monsterRenderer.flipX = false;
            }
        }
        else
        {
            // 마지막 웨이포인트(도착점)에 도달했을 때
            PlayManager.instance.nowSpawnMonsterCount--;
            PlayManager.instance.hp -= monsterDamage;
            if (PlayManager.instance.hp < 0)
            {
                PlayManager.instance.hp = 0;
            }

            isDead = true;
            monAnim.SetTrigger("Attack");
            StartCoroutine(MonsterDestory(0.5f));
        }
    }

    public void OnDamaged(float damage, PlayManager.AttackType attackType, float attackDecrease, float magicDecrease)
    {
        float reduction = 0f;

        // 물리, 마법방어력에 따른 데미지 감소 계산 공식.
        switch (attackType)
        {
            case PlayManager.AttackType.Attack:
                reduction = Mathf.Clamp(Mathf.Sqrt(10 * attackArmor * attackDecrease) / 100, 0f, 0.9f);
                break;
            case PlayManager.AttackType.Magic:
                reduction = Mathf.Clamp(Mathf.Sqrt(10 * magicArmor * magicDecrease) / 100, 0f, 0.9f);
                break;
        }
        if (hp > 0)
        {
            GameObject damageText = Instantiate(PlayManager.instance.damageText, Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 60, Quaternion.identity, PlayManager.instance.DamageCanvas.transform);
            DamageText text = damageText.GetComponent<DamageText>();
            text.damageText.text = System.Math.Round(damage * (1 - reduction), 2).ToString("F2");
            switch (attackType)
            {
                case PlayManager.AttackType.Attack:
                    text.damageText.color = Color.red;
                    break;
                case PlayManager.AttackType.Magic:
                    text.damageText.color = Color.blue;
                    break;
            }
            GameObject effect = Instantiate(PlayManager.instance.monsterHitEffect, transform.position, Quaternion.identity);
            StartCoroutine(PlayManager.instance.DestoryEffectTime(effect, 2f));
        }
        hp -= damage * (1 - reduction);
    }


    void poisonDamage()
    {
        if (PlayManager.instance.isPauseGame) return;

        float damage = 0f;
        if (SynergyManager.instance.propertyList[4].value == 5)
        {
            damage = (hp * 0.05f < 300) ? hp * 0.05f : 300;
        }
        else if (SynergyManager.instance.propertyList[4].value >= 4)
        {
            damage = (hp * 0.03f < 300) ? hp * 0.03f : 300;
        }
        else if (SynergyManager.instance.propertyList[4].value >= 3)
        {
            damage = (hp * 0.01f < 300) ? hp * 0.01f : 300;
        }
        PlayManager.instance.totalDamage[4] += damage;

        if (hp > 0)
        {
            GameObject damageText = Instantiate(PlayManager.instance.damageText, Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 60, Quaternion.identity, PlayManager.instance.DamageCanvas.transform);
            DamageText text = damageText.GetComponent<DamageText>();
            text.damageText.text = System.Math.Round(damage, 2).ToString("F2");
        }
        hp -= damage;
    }

    IEnumerator MonsterDestory(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
