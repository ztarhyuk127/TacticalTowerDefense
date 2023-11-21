using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletDamage = 0.0f;
    public Vector3 targetPosition = Vector3.zero;
    public GameObject targetObject = null;
    public Vector3 endPosition = Vector3.zero;
    public float speed = 15f;
    public PlayManager.AttackType attackType;
    public bool isLightning = false;
    public float attackArmorDecrease = 1.0f;
    public float magicArmorDecrease = 1.0f;

    void Update()
    {
        if (PlayManager.instance.isPauseGame || PlayManager.instance.isGameOver)
        {
            return;
        }

        if (targetObject != null)
        {
            targetPosition = (targetObject.transform.position - transform.position).normalized;
            endPosition = targetObject.transform.position;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, endPosition);
            if (distance < 1.0f)
            {
                Destroy(gameObject);
            }
        }
        transform.position += speed * Time.deltaTime * targetPosition * PlayManager.instance.speedTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") && collision.gameObject == targetObject)
        {
            Monster monster = collision.gameObject.GetComponent<Monster>();
            monster.OnDamaged(bulletDamage, attackType, attackArmorDecrease, magicArmorDecrease);
            if (isLightning)
            {
                PlayManager.instance.ApplyLightningAttack(collision.gameObject, bulletDamage, attackType, attackArmorDecrease, magicArmorDecrease);
            }
            
            Destroy(gameObject);
        }

    }
}
