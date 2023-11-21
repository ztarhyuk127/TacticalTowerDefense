using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI instance;
    private void Awake()
    {
        {
            if (instance != this && instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
