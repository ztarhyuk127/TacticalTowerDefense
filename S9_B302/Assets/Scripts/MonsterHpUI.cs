using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHpUI : MonoBehaviour
{
    public GameObject targetObject; // ����ٴ� ������Ʈ
    public Image healthBar; // ü�¹� UI ���

    void Update()
    {
        if (targetObject != null && healthBar != null)
        {
            // ������Ʈ ��ġ�� ȭ�� ��ǥ�� ��ȯ
            Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(targetObject.transform.position);
            // UI ��� ��ġ ������Ʈ
            healthBar.transform.position = targetScreenPosition + Vector3.up * 55;

            Monster monster = targetObject.GetComponent<Monster>();

            healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(monster.hp / monster.maxHp * 100, 20f);
        }
    }
}
