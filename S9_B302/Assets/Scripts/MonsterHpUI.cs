using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHpUI : MonoBehaviour
{
    public GameObject targetObject; // 따라다닐 오브젝트
    public Image healthBar; // 체력바 UI 요소

    void Update()
    {
        if (targetObject != null && healthBar != null)
        {
            // 오브젝트 위치를 화면 좌표로 변환
            Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(targetObject.transform.position);
            // UI 요소 위치 업데이트
            healthBar.transform.position = targetScreenPosition + Vector3.up * 55;

            Monster monster = targetObject.GetComponent<Monster>();

            healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(monster.hp / monster.maxHp * 100, 20f);
        }
    }
}
