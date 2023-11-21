using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    float opacityValue = 1f;

    void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (PlayManager.instance.isPauseGame || PlayManager.instance.isGameOver) return;

        damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, opacityValue);
        transform.position += PlayManager.instance.damageTextSpeed * Time.deltaTime * Vector3.up * PlayManager.instance.speedTime;

        opacityValue -= Time.deltaTime * 0.75f;
        if (opacityValue < 0f)
        {
            Destroy(gameObject);
        }
    }
}
