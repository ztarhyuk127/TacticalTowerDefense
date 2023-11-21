using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneUIManager : MonoBehaviour
{

    public static MainSceneUIManager Instance;
    // Importing Firebase functions
    // ���� ���̾�̽� ��ũ��Ʈ ����
    [SerializeField] private FirebaseAuthManager firebaseAuthManager;
    [SerializeField] private FirebaseDatabaseManager firebaseDatabaseManager;
    [SerializeField] private FirebaseRankManager firebaseRankManager;
    private FirebaseAuth auth;

    // GameObject Field
    // UI �г�
    [SerializeField] private GameObject unauthMainPage;
    [SerializeField] private GameObject authMainPage;
    [SerializeField] private GameObject registerPage;
    [SerializeField] private GameObject loginPage;
    [SerializeField] private GameObject rankPage;
    [SerializeField] private GameObject selectDifficultyPage;
    [SerializeField] private GameObject gameQuitPage;
    [SerializeField] private GameObject settingsPage;
    [SerializeField] private GameObject tutorialPage;

    // Input Field
    // ȸ������, �α��� ��ǲ �ʵ�
    public TMP_InputField registerEmailField;
    public TMP_InputField registerPasswordField;
    public TMP_InputField registerNicknameField;
    public TMP_InputField loginEmailField;
    public TMP_InputField loginPasswordField;


    // Text Field
    // ȸ������, �α��� ���� ǥ�ø� ���� �ؽ�Ʈ
    public TextMeshProUGUI loginErrorMessage;
    public TextMeshProUGUI registerErrorMessage;
    public TextMeshProUGUI registerProgressMessage;


    // User Info
    // ���� ������ ������ ������ �����ϰ� �����ݴϴ�.
    private string username;


    // Move to register page from main page for unauthenticated user
    // act when user press register button at the main page for unauthenticated user
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        
        StartCoroutine(BackToMainPage());
        AudioManager.instance.PlayMainBgm(true);
        auth = FirebaseAuth.DefaultInstance;
    }

    IEnumerator BackToMainPage()
    {
        yield return null;
        while (GameManager.Instance.isGaming)
        {
            yield return new WaitForSeconds(1.0f);
            if (!GameManager.Instance.isGaming)
            {
                Debug.Log("������ ���� �Ϸ�");
                if (auth.CurrentUser != null)
                {
                    Debug.Log("�α��� ���� o");
                    MoveToAuthPage();
                    yield return null;
                }
                else
                {
                    Debug.Log("�α��� ���� x");
                    MoveToUnauthPage();
                    yield return null;
                }
                yield break;
            }
        }
        if (!GameManager.Instance.isGaming)
        {
            Debug.Log("������ ���� �Ϸ�");
            if (auth.CurrentUser != null)
            {
                Debug.Log("�α��� ���� o");
                MoveToAuthPage();
                yield return null;
            }
            else
            {
                Debug.Log("�α��� ���� x");
                MoveToUnauthPage();
                yield return null;
            }
            yield break;
        }
    }
    public void MoveToRegisterPage()
    {
        unauthMainPage.SetActive(false);
        registerPage.SetActive(true);
    }

    public void MoveToSettingsPage()
    {
        if (!GameManager.Instance.isGaming)
        {
            SettingsUI.instance.gameObject.SetActive(true);
        }
    }

    public void BackFromSettingsPage()
    {
        if (!GameManager.Instance.isGaming)
        {
            if (auth.CurrentUser != null)
            {
                settingsPage.SetActive(false);
            }
            else
            {
                settingsPage.SetActive(false);
            }
        }
        
    }

    public void MoveToLoginPageFromRegisterPage()
    {
        registerPage.SetActive(false);
        loginPage.SetActive(true);
    }

    // Back to main page for unauthenticated user from register page
    // act when user press Go Back button at the register page
    public void BackToUnauthFromRegister()
    {
        registerErrorMessage.text = "";
        registerProgressMessage.text = "";
        unauthMainPage.SetActive(true);
        registerPage.SetActive(false);
    }

    // Move to Login page from main page for unauthenticated user
    // act when user press Login button at the main page for unauthenticated user
    public void MoveToLoginPage()
    {
        unauthMainPage.SetActive(false);
        loginPage.SetActive(true);
        loginErrorMessage.text = "";
    }

    // Back to main page for unauthenticated user from Login page
    // act when user press Go Back button at the Login page
    public void BackToUnauthFromLogin()
    {
        loginErrorMessage.text = "";
        unauthMainPage.SetActive(true);
        loginPage.SetActive(false);
    }

    public void BackToAuthFromRank()
    {
        authMainPage.SetActive(true);
        rankPage.SetActive(false);
    }

    public void BackToAuthFromGameStart()
    {
        authMainPage.SetActive(true);
        selectDifficultyPage.SetActive(false);
    }

    // Move to main page for authenticated user when user logged in
    // act when user Logged into Firebase Auth System
    public void MoveToAuthPage()
    {
        loginPage.SetActive(false);
        authMainPage.SetActive(true);
    }

    // Move to main page for unauthenticated user when user logged out
    // act when user logged out Firebase Auth System
    public void MoveToUnauthPage()
    {
        authMainPage.SetActive(false);
        unauthMainPage.SetActive(true);
    }

    public void MoveToRankPage()
    {
        loginPage.SetActive(false);
        rankPage.SetActive(true);
    }
    public void SelectEasyRank()
    {
        firebaseRankManager.GetRankingEasy();
    }

    public void SelectNormalRank()
    {
        firebaseRankManager.GetRankingNormal();
    }

    public void SelectHardRank()
    {
        firebaseRankManager.GetRankingHard();
    }

    public void SelectInfiniteRank()
    {
        firebaseRankManager.GetRankingInfinite();
    }
    public void GameStartEasy()
    {
        GameManager.Instance.isGaming = true;
        GameManager.Instance.gameDifficulty = "Easy";
        AudioManager.instance.PlayMainBgm(false);
        LoadSceneManager.CallNextScene("Easy Stage");
        
    }

    public void GameStartNormal()
    {
        GameManager.Instance.isGaming = true;
        GameManager.Instance.gameDifficulty = "Normal";
        AudioManager.instance.PlayMainBgm(false);
        LoadSceneManager.CallNextScene("Normal Stage");
    }

    public void GameStartHard()
    {
        GameManager.Instance.isGaming = true;
        GameManager.Instance.gameDifficulty = "Hard";
        AudioManager.instance.PlayMainBgm(false);
        LoadSceneManager.CallNextScene("Hard Stage");
    }

    public void GameStartInfinite()
    {
        GameManager.Instance.isGaming = true;
        GameManager.Instance.gameDifficulty = "Infinite";
        AudioManager.instance.PlayMainBgm(false);
        LoadSceneManager.CallNextScene("Infinity Stage");
    }



    public void onClickRegister()
    {
        registerProgressMessage.text = "";
        registerErrorMessage.text = "";
        if (registerEmailField.text.Trim() != "")
        {
            if (registerPasswordField.text.Trim().Length >= 6)
            {
                if (registerNicknameField.text.Trim().Length < 2)
                {
                    registerErrorMessage.text = "�г����� �ʹ� ª���ϴ�. �г����� �ּ� 2��, �ִ� 12���Դϴ�.";
                    return;
                }
                else if (registerNicknameField.text.Trim().Length > 12)
                {
                    registerErrorMessage.text = "�г����� �ʹ� ��ϴ�. �г����� �ּ� 2��, �ִ� 12���Դϴ�.";
                    return;
                }
                else if (registerNicknameField.text.Trim().Length <= 12 && registerNicknameField.text.Trim().Length >= 2)
                {
                    FirebaseDatabaseManager.instance.CheckNickname(registerNicknameField.text.Trim());
                    StartCoroutine(CheckValidNickname());
                }
            }
            else
            {
                registerErrorMessage.text = "��й�ȣ�� �ʹ� ª���ϴ�.";
                return;
            }
        }
        else
        {
            registerErrorMessage.text = "��ȿ���� ���� �̸��� �ּ��Դϴ�.";
            return;
        }
    }

    IEnumerator CheckValidNickname()
    {
        int cnt = 0;
        registerProgressMessage.text = "�г��� �ߺ� �˻���...";
        while (cnt < 2)
        {
            yield return new WaitForSeconds(1.0f);
            registerProgressMessage.text = "";
            if (FirebaseDatabaseManager.instance.isValidNickname)
            {
                registerProgressMessage.text = "�г��� �ߺ� �˻� �Ϸ�!";
                FirebaseDatabaseManager.instance.isValidNickname = false;
                firebaseAuthManager.Create(
                    registerEmailField.text,
                    registerPasswordField.text,
                    registerNicknameField.text
                );
                StartCoroutine(checkIsSignupAllowed());
                yield break;
            }
            else
            {
                registerProgressMessage.text = "";
                registerErrorMessage.text = "�ߺ��� �г����Դϴ�.";
                
            }
            cnt++;
        }
        yield return null;
    }
    IEnumerator checkIsSignupAllowed()
    {
        Debug.Log("ȸ������ �ڷ�ƾ");
        int cnt = 0;
        while (!GameManager.Instance.isSignupAllowed)
        {
            yield return new WaitForSeconds(1.0f);
            if (GameManager.Instance.isSignupAllowed)
            {
                Debug.Log("ȸ������ �Ϸ�!");
                MoveToLoginPageFromRegisterPage();
                yield break;
            }
            cnt++;
            if (cnt >= 5)
            {
                Debug.Log("ȸ������ �ε� �Ұ�. ���ͳ� �����ε�?");
                yield break;
            }
        }
        if (GameManager.Instance.isSignupAllowed)
        {
            Debug.Log("ȸ������ �Ϸ�!");
            MoveToLoginPageFromRegisterPage();
            yield break;
        }
        yield return null;
    }

    IEnumerator checkIsLoginAllowed()
    {
        Debug.Log("�α��� �ڷ�ƾ");
        int cnt = 0;
        while (!GameManager.Instance.isLoginAllowed)
        {
            yield return new WaitForSeconds(1.0f);
            
            if (GameManager.Instance.isLoginAllowed)
            {
                Debug.Log("�α��α��� �Ϸ�!");
                MoveToAuthPage();
                yield break;
            }
            
            cnt++;
            if (cnt >= 5)
            {
                Debug.Log("�α��� �ε� �Ұ�. ���ͳ� �����ε�?");
                yield break;
            }
            Debug.Log(cnt);
        }
        if (GameManager.Instance.isLoginAllowed)
        {
            Debug.Log("�α��α��� �Ϸ�!");
            MoveToAuthPage();
            yield break;
        }
        yield return null;
    }

    public void onClickLogIn()
    {
        if (loginEmailField.text.Trim() != "")
        {
            if (loginPasswordField.text.Trim().Length >= 6)
            {
                firebaseAuthManager.Login(loginEmailField.text, loginPasswordField.text);
                StartCoroutine(checkIsLoginAllowed());
            }
            else
            {
                Debug.Log("��й�ȣ�� �ʹ� ª���ϴ�.");
                return;
            }
        }
        else
        {
            Debug.Log("��ȿ���� ���� �̸����Դϴ�.");
            return;
        }
    }
    public void onClickLogout()
    {
        firebaseAuthManager.Logout();
        MoveToUnauthPage();
    }
    public void onClickGameStart()
    {
        authMainPage.SetActive(false);
        selectDifficultyPage.SetActive(true);
    }
    public void onClickRank()
    {
        authMainPage.SetActive(false);
        rankPage.SetActive(true);
        firebaseRankManager.GetRankingInfinite();
    }

    // Quit game when user press quit button
    public void OpenGameQuitPage()
    {
        gameQuitPage.SetActive(true);
    }

    public void CloseGameQuitPage()
    {
        gameQuitPage.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void tutorialActive()
    {
        tutorialPage.SetActive(true);
    }

}
