using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixResolution : MonoBehaviour
{
    private void Start()
    {
        SetResolution(); // �ʱ⿡ ���� �ػ� ����
    }

    /* �ػ� �����ϴ� �Լ� */
    public void SetResolution()
    {
        int deviceWidth = Screen.width;
        int deviceHegith = Screen.height;
        float ratio = (float)deviceWidth / deviceHegith;

        if (ratio > 2)
        {
            Camera.main.orthographicSize = 25;
            Camera.main.transform.position = Vector3.up * 5 + Vector3.back * 20;
        }
        else
        {
            Camera.main.orthographicSize = 30;
            Camera.main.transform.position = Vector3.up * 10 + Vector3.back * 20;
        }
    }
}
