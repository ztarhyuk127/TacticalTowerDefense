using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager instance;

    [SerializeField] TextMeshProUGUI progressText;
    float progress;
    public static string nextScene;
    private FirebaseAuth auth;
    private void Awake()
    {
        instance = this;
        StartCoroutine(LoadScene());
        auth = FirebaseAuth.DefaultInstance;
    }

    // if you want to call scene with loading scene, use this one
    // for example : LoadSceneManager.CallNextScene("SceneNameYouWantToGo")
    public static void CallNextScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading", LoadSceneMode.Single);
    }


    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation asyncOp;
        asyncOp = SceneManager.LoadSceneAsync(nextScene);
        asyncOp.allowSceneActivation = false;
        while (!asyncOp.isDone)
        {
            yield return null;
            progress = asyncOp.progress;
            progressText.text = $"{System.Math.Round(progress, 2) * 100.0f}%";
            if (asyncOp.progress >= 0.9f)
            {
                progressText.text = "100%"; 
                asyncOp.allowSceneActivation = true;
                if (nextScene == "Main") 
                { 
                    GameManager.Instance.isGaming = false;
                }
                else 
                {
                    GameManager.Instance.isGaming = true;
                }
                yield return null;
            }
        }
    }
}
