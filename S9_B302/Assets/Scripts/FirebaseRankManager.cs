using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseRankManager : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseAuthManager firebaseAuthManager;
    FirebaseDatabaseManager firebaseDatabaseManager;
    DatabaseReference reference;
    public string rankType;
    int totalNumberOfData;
    private int myRank;
    public GameObject UIContent;
    public GameObject contentItem;
    public GameObject myRankItem;
    public GameObject infContentItem;
    public GameObject infMyRankItem;
    public TextMeshProUGUI DifficultyUI;

    List<GameObject> itemList = new List<GameObject>();

    // Start is called before the first frame update
    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        // GetUserCount();
    }

    
    public void GetUserCount()
    {
        reference.Child("ClearData").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                totalNumberOfData = (int)snapshot.ChildrenCount;
            }
        });
    }
    public void GetRankingEasy()
    {
        DifficultyUI.text = "- Easy";
        infMyRankItem.SetActive(false);
        myRankItem.SetActive(true);
        foreach (GameObject itemObject in itemList)
        {
            Destroy(itemObject);
        }
        itemList.Clear();

        TextMeshProUGUI[] myInfo = myRankItem.GetComponentsInChildren<TextMeshProUGUI>();
        myInfo[0].text = "X";
        myInfo[1].text = "랭킹 정보가 없습니다";
        myInfo[2].text = "XX:XX:XX";

        reference
        .Child("ClearData")
        .OrderByChild("Difficulty/Easy/TimeScore")
        .LimitToFirst(10)
        .StartAt(1)
        .GetValueAsync()
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                
                int rank = 0;
                int preRank = 1;
                int preTime = 0;
                foreach (DataSnapshot childsnapshot in snapshot.Children)
                {
                    if (!childsnapshot.HasChild("UserId"))
                    {
                        continue;
                    }

                    GameObject newUI = Instantiate(contentItem, UIContent.transform);
                    itemList.Add(newUI);
                    TextMeshProUGUI[] texts = newUI.GetComponentsInChildren<TextMeshProUGUI>();

                    texts[1].text = childsnapshot.Key.ToString();
                    Int64 rawTimeData = (Int64)childsnapshot.Child("Difficulty/Easy/TimeScore").Value;
                    int minute = (int)rawTimeData / 6000;
                    int second = (int)(rawTimeData / 100) % 60;
                    int milsec = (int)rawTimeData % 100;
                    texts[2].text = TimeConverter(minute, second, milsec);

                    if (preTime != rawTimeData)
                    {
                        rank += preRank;
                        preRank = 1;
                    }
                    else
                    {
                        preRank++;
                    }
                    texts[0].text = rank.ToString();

                    if ((string)childsnapshot.Child("UserId").Value == auth.CurrentUser.UserId)
                    {
                        myRank = rank;
                        Int64 myRawTimeData = (Int64)childsnapshot.Child("Difficulty/Easy/TimeScore").Value;
                        int myMinute = (int)myRawTimeData / 6000;
                        int mySecond = (int)(myRawTimeData / 100) % 60;
                        int myMilsec = (int)myRawTimeData % 100;
                        myInfo[0].text = myRank.ToString();
                        myInfo[1].text = childsnapshot.Key.ToString();
                        myInfo[2].text = TimeConverter(myMinute, mySecond, myMilsec);
                    }
                    preTime = (int)rawTimeData;
                }
            }
        });
    }

    public void GetRankingNormal()
    {
        int mycnt = 0;
        DifficultyUI.text = "- Normal";
        myRankItem.SetActive(true);
        infMyRankItem.SetActive(false);
        foreach (GameObject itemObject in itemList)
        {
            Destroy(itemObject);
        }
        itemList.Clear();
        
        TextMeshProUGUI[] myInfo = myRankItem.GetComponentsInChildren<TextMeshProUGUI>();
        myInfo[0].text = "X";
        myInfo[1].text = "랭킹 정보가 없습니다";
        myInfo[2].text = "XX:XX:XX";

        reference
        .Child("ClearData")
        .OrderByChild("Difficulty/Normal/TimeScore")
        .LimitToFirst(10)
        .StartAt(1)
        .GetValueAsync()
        .ContinueWithOnMainThread(task =>
        {
            
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int rank = 0;
                int preRank = 1;
                int preTime = 0;
                foreach (DataSnapshot childsnapshot in snapshot.Children)
                {
                    if (!childsnapshot.HasChild("UserId"))
                    {
                        continue;
                    }

                    mycnt++;
                    GameObject newUI = Instantiate(contentItem, UIContent.transform);
                    itemList.Add(newUI);
                    TextMeshProUGUI[] texts = newUI.GetComponentsInChildren<TextMeshProUGUI>();
                    texts[1].text = childsnapshot.Key.ToString();

                    Int64 rawTimeData = (Int64)childsnapshot.Child("Difficulty/Normal/TimeScore").Value;
                    int minute = (int)rawTimeData / 6000;
                    int second = (int)(rawTimeData / 100) % 60;
                    int milsec = (int)rawTimeData % 100;
                    texts[2].text = TimeConverter(minute, second, milsec);

                    if (preTime != rawTimeData)
                    {
                        rank += preRank;
                        preRank = 1;
                    }
                    else
                    {
                        preRank++;
                    }
                    texts[0].text = rank.ToString();
                    if ((string)childsnapshot.Child("UserId").Value == auth.CurrentUser.UserId)
                    {
                        myRank = rank;
                        TextMeshProUGUI[] myInfo = myRankItem.GetComponentsInChildren<TextMeshProUGUI>();
                        myInfo[0].text = myRank.ToString();
                        myInfo[1].text = childsnapshot.Key.ToString();
                        Int64 myRawTimeData = (Int64)childsnapshot.Child("Difficulty/Normal/TimeScore").Value;
                        int myMinute = (int)rawTimeData / 6000;
                        int mySecond = (int)(rawTimeData / 100) % 60;
                        int myMilsec = (int)rawTimeData % 100;
                        myInfo[2].text = TimeConverter(myMinute, mySecond, myMilsec);
                    }
                    preTime = (int)rawTimeData;
                }
            }
            else if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log(task.Exception);
            }
        });
    }

    public string TimeConverter(int minute, int second, int milsec) 
    {
        
        string convertedTime = $"{minute.ToString("00")}:{second.ToString("00")}:{milsec.ToString("00")}";
        return convertedTime;
    }

    public void GetRankingHard()
    {
        DifficultyUI.text = "- Hard";
        myRankItem.SetActive(true);
        infMyRankItem.SetActive(false);

        foreach (GameObject itemObject in itemList)
        {
            Destroy(itemObject);
        }
        itemList.Clear();

        TextMeshProUGUI[] myInfo = myRankItem.GetComponentsInChildren<TextMeshProUGUI>();
        myInfo[0].text = "X";
        myInfo[1].text = "랭킹 정보가 없습니다";
        myInfo[2].text = "XX:XX:XX";

        reference
        .Child("ClearData")
        .OrderByChild("Difficulty/Hard/TimeScore")
        .LimitToFirst(10)
        .StartAt(1)
        .GetValueAsync()
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int rank = 0;
                int preRank = 1;
                int preTime = 0;
                foreach (DataSnapshot childsnapshot in snapshot.Children)
                {
                    if (!childsnapshot.HasChild("UserId"))
                    {
                        continue;
                    }

                    GameObject newUI = Instantiate(contentItem, UIContent.transform);
                    itemList.Add(newUI);
                    TextMeshProUGUI[] texts = newUI.GetComponentsInChildren<TextMeshProUGUI>();
                    Int64 rawTimeData = (Int64)childsnapshot.Child("Difficulty/Hard/TimeScore").Value;
                    int minute = (int)rawTimeData / 6000;
                    int second = (int)(rawTimeData / 100) % 60;
                    int milsec = (int)rawTimeData % 100;
                    texts[1].text = childsnapshot.Key.ToString();
                    texts[2].text = TimeConverter(minute, second, milsec);
                    

                    if (preTime != rawTimeData)
                    {
                        rank += preRank;
                        preRank = 1;
                    }
                    else
                    {
                        preRank++;
                    }
                    texts[0].text = rank.ToString();
                    if ((string)childsnapshot.Child("UserId").Value == auth.CurrentUser.UserId)
                    {
                        myRank = rank;
                        Int64 myRawTimeData = (Int64)childsnapshot.Child("Difficulty/Hard/TimeScore").Value;
                        int myMinute = (int)rawTimeData / 6000;
                        int mySecond = (int)(rawTimeData / 100) % 60;
                        int myMilsec = (int)rawTimeData % 100;
                        TextMeshProUGUI[] myInfo = myRankItem.GetComponentsInChildren<TextMeshProUGUI>();
                        myInfo[0].text = myRank.ToString();
                        myInfo[1].text = childsnapshot.Key.ToString();
                        myInfo[2].text = TimeConverter(myMinute, mySecond, myMilsec);
                    }
                    preTime = (int)rawTimeData;
                }
            }
        });

    }

    public void GetRankingInfinite()
    {
        DifficultyUI.text = "- Infinite";
        myRankItem.SetActive(false);
        infMyRankItem.SetActive(true);

        foreach (GameObject itemObject in itemList)
        {
            Destroy(itemObject);
        }
        itemList.Clear();

        TextMeshProUGUI[] myInfo = myRankItem.GetComponentsInChildren<TextMeshProUGUI>();
        myInfo[0].text = "X";
        myInfo[1].text = "랭킹 정보가 없습니다";
        myInfo[2].text = "XX:XX:XX";

        List<DataSnapshot> snapshotList = new List<DataSnapshot>();
        reference.Child("ClearData")
        .OrderByChild("Difficulty/Infinite/StageScore")
        .LimitToLast(totalNumberOfData)
        .StartAt(1)
        .GetValueAsync()
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot childsnapshot in snapshot.Children)
                {
                    if (!childsnapshot.HasChild("UserId"))
                    {
                        continue;
                    }
                    snapshotList.Add(childsnapshot);
                }
                snapshotList.Reverse();
                int rank = 0;
                int preRank = 1;
                int preTime = 0;
                foreach (DataSnapshot childsnapshot in snapshotList)
                {
                    GameObject newUI = Instantiate(infContentItem, UIContent.transform);
                    itemList.Add(newUI);
                    TextMeshProUGUI[] texts = newUI.GetComponentsInChildren<TextMeshProUGUI>();
                    Int64 rawTimeData = (Int64)childsnapshot.Child("Difficulty/Infinite/StageScore").Value;
                    texts[1].text = childsnapshot.Key.ToString();
                    texts[2].text = rawTimeData.ToString();

                    if (preTime != rawTimeData)
                    {
                        rank += preRank;
                        preRank = 1;
                    }
                    else
                    {
                        preRank++;
                    }
                    texts[0].text = rank.ToString();
                    if ((string)childsnapshot.Child("UserId").Value == auth.CurrentUser.UserId)
                    {
                        myRank = rank;
                        TextMeshProUGUI[] myInfo = infMyRankItem.GetComponentsInChildren<TextMeshProUGUI>();
                        myInfo[0].text = myRank.ToString();
                        myInfo[1].text = childsnapshot.Key.ToString();
                        myInfo[2].text = childsnapshot.Child("Difficulty/Infinite/StageScore").Value.ToString();
                    }
                    preTime = (int)rawTimeData;
                }
            }
        });

    }
}
