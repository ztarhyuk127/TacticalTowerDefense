using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    System.Action _OnClickConformButton, _OnClickCancelButton;
    public static GameManager Instance;

    [SerializeField]private FirebaseAuthManager authManager;
    private FirebaseAuth auth;

    [Header("auth")]
    public bool isLoginAllowed = false;
    public bool isSignupAllowed= false;

    [Header("ingame")]
    public bool isGaming = false;
    public string gameDifficulty = "infinite";
    public GameObject settingsUI;
    public float masterVolume, backgroundVolume, sfxVolume;
    public GameObject updateUI;

    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            masterVolume = PlayerPrefs.GetFloat("masterVolume");
            backgroundVolume = PlayerPrefs.GetFloat("bgmVolume");
            sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
        else
        {
            masterVolume = 0.5f;
            backgroundVolume = 0.5f;
            sfxVolume = 0.5f;
            Debug.Log("No volume value");
        }

        Invoke(nameof(CheckVersion), 1f);
    }
    private void Start()
    {
        SettingsUI.instance.gameObject.SetActive(true);
        SettingsUI.instance.gameObject.SetActive(false);

    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioManager.instance.sliders[0].value);
        PlayerPrefs.SetFloat("bgmVolume", AudioManager.instance.sliders[1].value);
        PlayerPrefs.SetFloat("sfxVolume", AudioManager.instance.sliders[2].value);
        Debug.Log("게임 꺼져욧");
        

    }

    public void SettingOn(System.Action OnClickConformButton, System.Action OnClickCancelButton)
    {
        settingsUI.SetActive(true);
        _OnClickConformButton = OnClickConformButton;
        _OnClickCancelButton = OnClickCancelButton;
    }

    public void SettingOff()
    {
        settingsUI.SetActive(false);
    }
    
    public void GetUpdateVersion()
    {
        // 여기에 구글 플레이스토어 주소를 적습니다.
        // 지금은 플레이스토어가 없기에 배포용 홈페이지 주소를 적습니다.
        Application.OpenURL("https://k9b302.p.ssafy.io");
        Application.Quit();
    }

    private void CheckVersion()
    {
        DatabaseReference versionReference = FirebaseDatabase.DefaultInstance.GetReference("Version");
        versionReference.GetValueAsync().ContinueWithOnMainThread((task) =>
        {
            if (task.IsCompleted)
            {
                string serverVersion = task.Result.Value.ToString();
                if (serverVersion != Application.version)
                {
                    updateUI.SetActive(true);
                    return;
                }
                Debug.Log("버전같음!");
            }
        });
    }
}
