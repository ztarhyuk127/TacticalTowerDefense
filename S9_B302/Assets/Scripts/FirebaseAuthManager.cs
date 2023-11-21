using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth; // 로그인/회원가입 등에 사용
    public FirebaseUser user; // 인증이 완료된 유저 정보
    public FirebaseDatabaseManager firebaseDatabaseManager;
    public FirebaseDatabase firebaseDatabase;

    public TMP_InputField email;   // 이메일을 input하는 필드
    public TMP_InputField password; // 비밀번호를 input하는 필드
    public TMP_InputField nickname; // 닉네임을 input하는 필드

    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    public string userId;
    public string useremail;
    public string username;

    public Color redColor = new Color(1f, 0.1924528f, 0.1924528f, 1f);
    public Color greenColor = new Color(0.350058f, 1f, 0.1924528f, 1f);
    
    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        // DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.GetReference("ClearData");
    }

    public void IsLoggedIn()
    {
        if (auth.CurrentUser != null)
        {
            Debug.Log("로그인 alright");
            Debug.Log(auth.CurrentUser.Email);
        }
        else
        {
            Debug.Log("Not 로그인");
        }
    }

    public async void Create(string email, string password, string nickname)
    {
        await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                
                AuthResult authResult = task.Result;
                FirebaseUser newUser = authResult.User;
                string userId = auth.CurrentUser.UserId;
                GameManager.Instance.isSignupAllowed = true;
                Debug.Log("GameManager : ok to signin");
                FirebaseDatabaseManager.instance.InitializeUserInfo(email, nickname, userId);
            }
            else
            {
                Debug.Log("오류 : " + task.Exception);
            }
        });

    }

    public void Login(string email, string password)
    {
        MainSceneUIManager.Instance.loginErrorMessage.color = greenColor;
        MainSceneUIManager.Instance.loginErrorMessage.text = "";
        MainSceneUIManager.Instance.loginErrorMessage.text = "로그인 중...";
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                AuthResult authResult = task.Result;
                user = authResult.User;
                userId = user.UserId;
                useremail = user.Email;
                FirebaseDatabaseManager.instance.ReadInfo();
                GameManager.Instance.isLoginAllowed = true;
            }
            else
            {
                Debug.Log(task.Exception.InnerException);
                foreach (var exception in task.Exception.InnerExceptions)
                {
                    string errorMessage = "로그인에 실패했습니다: ";
                    Firebase.FirebaseException firebaseException = exception as Firebase.FirebaseException;
                    if (firebaseException != null)
                    {
                        Debug.Log(firebaseException.ErrorCode);
                        if (firebaseException.ErrorCode == 11)
                        {
                            errorMessage += "유효하지 않은 이메일 주소입니다.";
                        }
                        else if (firebaseException.ErrorCode == 1)
                        {
                            errorMessage += "이메일 혹은 비밀번호가 틀렸습니다.";
                        }
                        else
                        {
                            errorMessage += "알 수 없는 오류가 발생했습니다.";
                        }
                    }
                    MainSceneUIManager.Instance.loginErrorMessage.color = redColor;
                    MainSceneUIManager.Instance.loginErrorMessage.text = errorMessage;
                    return;
                }
            }
        });
    }

    public void Logout()
    {
        auth.SignOut();
        useremail = "";
        userId = "";
        username = "";
        Debug.Log("로그아웃");
    }
}
