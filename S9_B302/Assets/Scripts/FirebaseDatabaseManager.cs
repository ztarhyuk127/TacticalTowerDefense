using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;


public class FirebaseDatabaseManager : MonoBehaviour
{
    public static FirebaseDatabaseManager instance;
    
    public FirebaseAuth auth;
    public FirebaseAuthManager authManager;
    public string Difficulty;
    public string username;
    public string userId;
    public bool isValidNickname;
    public TextMeshProUGUI usernameText;
    private string customKey;
    Int64 currentEasyTime;
    Int64 currentNormalTime;
    Int64 currentHardTime;
    Int64 currentInfiniteTime;
    Int64 currentInfiniteStage;

    Dictionary<string, object>  DictInfo = new Dictionary<string, object>();
    Dictionary<string, object> DictDifficulty = new Dictionary<string, object>();

    Dictionary<string, object> DictEasyScore = new Dictionary<string, object>();
    Dictionary<string, object> DictHardScore = new Dictionary<string, object>();
    Dictionary<string, object> DictNormalScore = new Dictionary<string, object>();
    Dictionary<string, object> DictInfiniteScore = new Dictionary<string, object>();
    FirebaseUser myUser;
    private void Awake()
    {
        myUser = authManager.user;
        auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null)
        {
            customKey = auth.CurrentUser.UserId;
            ReadInfo();
        }

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

    public void InitializeUserInfo(string email, string nickname, string userId)
    {
        DictEasyScore["TimeScore"] = 0;
        DictNormalScore["TimeScore"] = 0;
        DictHardScore["TimeScore"] = 0;
        DictInfiniteScore["TimeScore"] = 0;
        DictInfiniteScore["StageScore"] = 0;

        DictDifficulty["Easy"] = DictEasyScore;
        DictDifficulty["Normal"] = DictNormalScore;
        DictDifficulty["Hard"] = DictHardScore;
        DictDifficulty["Infinite"] = DictInfiniteScore;
        DictInfo["Difficulty"] = DictDifficulty;
        DictInfo["Email"] = email;
        DictInfo["UserId"] = userId;
        username = nickname;

        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.GetReference($"ClearData/{username}");
        databaseReference.UpdateChildrenAsync(DictInfo).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("회원가입 데이터 초기화 완료.");
            }
            else if (task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
            }
            else if (task.IsFaulted)
            {
                Debug.Log(task.Exception.ToString());
            }
        });
    }

    public void CheckNickname(string nickname)
    {
        DatabaseReference InfoDB = FirebaseDatabase.DefaultInstance.GetReference("ClearData");
        InfoDB.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                int cnt = 0;
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot childsnap in snapshot.Children)
                {
                    if (nickname == childsnap.Key.ToString())
                    {
                        isValidNickname = false;
                        cnt++;
                        break;
                    }
                }
                if (cnt == 0)
                {
                    isValidNickname = true;
                }
                else
                {
                    isValidNickname = false;
                }
            }
        });
    }
         
    public void ReadInfo()
    {
        DatabaseReference InfoDB = FirebaseDatabase.DefaultInstance.GetReference("ClearData");
        InfoDB.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("ReadError : " + task.Exception);
            }
            else if (task.IsCanceled)
            {
                Debug.Log("ReadCancelled : " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot userInfo in snapshot.Children)
                {
                    if (!userInfo.HasChild("UserId"))
                    {
                        continue;
                    }
                    Debug.Log(userInfo.GetRawJsonValue());
                    Debug.Log(userInfo.Child("UserId").Value.ToString());
                    if (userInfo.Child("UserId").Value.ToString() == auth.CurrentUser.UserId)
                    {
                        Debug.Log(userInfo.Key.ToString());
                        username = userInfo.Key.ToString();
                        usernameText.text = "User: " + username;
                        foreach (DataSnapshot totalInfo in userInfo.Children)
                        {
                            if (totalInfo.Key == "UserId")
                            {
                                userId = totalInfo.Value.ToString();
                            }
                            if (totalInfo.Key == "Email")
                            {
                                string Email = totalInfo.Value.ToString();
                            }
                            if (totalInfo.Key == "Difficulty")
                            {
                                foreach (DataSnapshot DifficultyInfo in totalInfo.Children)
                                {

                                    if (DifficultyInfo.Key == "Easy")
                                    {
                                        currentEasyTime = (Int64)DifficultyInfo.Child("TimeScore").Value;
                                    }
                                    else if (DifficultyInfo.Key == "Hard")
                                    {
                                        currentHardTime = (Int64)DifficultyInfo.Child("TimeScore").Value;
                                    }
                                    else if (DifficultyInfo.Key == "Infinite")
                                    {
                                        Int64 infStage = (Int64)DifficultyInfo.Child("StageScore").Value;
                                        currentInfiniteStage = (Int64)infStage;
                                        currentInfiniteTime = (Int64)DifficultyInfo.Child("TimeScore").Value;
                                    }
                                    else if (DifficultyInfo.Key == "Normal")
                                    {
                                        currentNormalTime = (Int64)DifficultyInfo.Child("TimeScore").Value;
                                    }
                                }
                            }
                        }
                    }
                }
                Debug.Log("ReadInfo Completed");
            }
        });
    }
    public void GetUserInfo()
    {
        if (auth.CurrentUser != null)
        {
            Debug.Log(auth.CurrentUser.Email);
        }
    }

    public void SendClearDataEasyMode(Int64 TimeScore)
    {
        if (auth.CurrentUser != null)
        {
            Dictionary<string, object> currentEasyScore = new Dictionary<string, object>();
            DatabaseReference DifficultyData = FirebaseDatabase.DefaultInstance.GetReference("ClearData").Child(username).Child("Difficulty").Child("Easy");
            DifficultyData.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("simple error in get value of Easymode");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    currentEasyTime = (int)snapshot.Child("TimeScore").Value;
                    if (currentEasyTime == 0 || currentEasyTime > TimeScore)
                    {
                        currentEasyTime = TimeScore;
                        Debug.Log("반영됐어요.");
                    }
                    currentEasyScore["TimeScore"] = currentEasyTime;

                    DifficultyData.UpdateChildrenAsync(currentEasyScore).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled || task.IsFaulted)
                        {
                            Debug.Log("simple error in update value of easymode");
                        }
                        else if (task.IsCompleted)
                        {
                            Debug.Log("이지 모드 기록 반영.");
                        }
                    });
                }
            });
            
        }
    }
    public void SendClearDataNormalMode(Int64 TimeScore)
    {
        if (auth.CurrentUser != null)
        {
            Dictionary<string, object> currentNormalScore = new Dictionary<string, object>();

            DatabaseReference DifficultyData = FirebaseDatabase.DefaultInstance.GetReference("ClearData").Child(username).Child("Difficulty").Child("Normal");
            DifficultyData.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("simple error in get value of normalmode data");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    currentNormalTime = (int)snapshot.Child("TimeScore").Value;
                    if (currentNormalTime < 1 || currentNormalTime > TimeScore)
                    {
                        currentNormalTime = TimeScore;
                    }
                    currentNormalScore["TimeScore"] = currentNormalTime;
                    DifficultyData.UpdateChildrenAsync(currentNormalScore).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled || task.IsFaulted)
                        {
                            Debug.Log("simple error in update value of normalmode");
                        }
                        else if (task.IsCompleted)
                        {
                            Debug.Log("노말 모드 기록 반영.");
                        }
                    });
                }
            });

            
        }
    }
    public void SendClearDataHardMode(Int64 TimeScore)
    {
        if (auth.CurrentUser != null)
        {
            Dictionary<string, object> currentHardScore = new Dictionary<string, object>();
            DatabaseReference DifficultyData = FirebaseDatabase.DefaultInstance.GetReference("ClearData").Child(username).Child("Difficulty").Child("Hard");

            DifficultyData.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("simple error in get value of hardmode data");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    currentHardTime = (int)snapshot.Child("TimeScore").Value;
                    if (currentHardTime < 1 || currentHardTime > TimeScore)
                    {
                        currentHardTime = TimeScore;
                    }
                    currentHardScore["TimeScore"] = currentHardTime;

                    DifficultyData.UpdateChildrenAsync(currentHardScore).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled || task.IsFaulted)
                        {
                            Debug.Log("simple error in update value of hardmode");
                        }
                        else if (task.IsCompleted)
                        {
                            Debug.Log("하드 모드 기록 반영.");
                        }
                    });
                }
            });

            

        }
    }
    public void SendClearDataInfiniteMode(Int64 TimeScore, Int64 StageScore)
    {
        if (auth.CurrentUser != null)
        {
            Dictionary<string, object> currentInfiniteScore = new Dictionary<string, object>();

            DatabaseReference DifficultyData = FirebaseDatabase.DefaultInstance.GetReference("ClearData").Child(username).Child("Difficulty").Child("Infinite");

            DifficultyData.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log("simple error in get value of InfiniteMode data");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    Int64 abc = (Int64)snapshot.Child("TimeScore").Value;
                    Int64 def = (Int64)snapshot.Child("StageScore").Value;
                    currentInfiniteTime = (int)abc;
                    currentInfiniteStage = (int)def;

                    if (currentInfiniteTime < 1 || currentInfiniteTime > TimeScore)
                    {
                        if (currentInfiniteStage < 1 || currentInfiniteStage < StageScore)
                        {
                            currentInfiniteStage = StageScore;
                            currentInfiniteTime = TimeScore;
                        }
                        else if (currentInfiniteStage < 1 || currentInfiniteStage == StageScore)
                        {
                            currentInfiniteTime = TimeScore;

                        }
                    }
                    else if (currentInfiniteTime < 1 || currentInfiniteTime <= TimeScore)
                    {
                        if (currentInfiniteStage < 1 || currentInfiniteStage < StageScore)
                        {
                            currentInfiniteStage = StageScore;
                            currentInfiniteTime = TimeScore;
                        }
                    }

                    currentInfiniteScore["TimeScore"] = currentInfiniteTime;
                    currentInfiniteScore["StageScore"] = currentInfiniteStage;

                    DifficultyData.UpdateChildrenAsync(currentInfiniteScore).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled || task.IsFaulted)
                        {
                            Debug.Log("simple error in update value of InfiniteMode");
                        }
                        else if (task.IsCompleted)
                        {
                            Debug.Log("무한 모드 기록 반영.");
                        }
                    });
                }
            });
        }
    }
}
