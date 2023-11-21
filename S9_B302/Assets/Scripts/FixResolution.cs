using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixResolution : MonoBehaviour
{
    private void Start()
    {
        SetResolution(); // 초기에 게임 해상도 고정
    }

    /* 해상도 설정하는 함수 */
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
