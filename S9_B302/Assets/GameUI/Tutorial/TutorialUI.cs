using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public Sprite[] tutorialImageList;
    public int nowPage;

    // [TextArea(3, 15)]
    // public string[] tutorialDescriptionList;

    // public string tutorialPageNum;

    public Image tutorialImage;
    // public TextMeshProUGUI tutorialDescription;
    // public TextMeshProUGUI tutorialPage;
    public Button nextPage;
    public Button prevPage;

    // Start is called before the first frame update
    void Start()
    {
        // GameObject gameObject = this.gameObject;
        tutorialImage.sprite = tutorialImageList[0];
        // tutorialDescription.text = tutorialDescriptionList[0];
        nowPage = 0;
        // tutorialPageNum = $"{nowPage + 1} / {tutorialImageList.Length}";
        // tutorialPage.text = tutorialPageNum;
    }

    public void nextTutorialPage()
    {
        if(nowPage < tutorialImageList.Length - 1)
        {
            nowPage++;
            tutorialImage.sprite = tutorialImageList[nowPage];
            // tutorialDescription.text = tutorialDescriptionList[nowPage];
            // tutorialPageNum = $"{nowPage + 1} / {tutorialImageList.Length}";
            // tutorialPage.text = tutorialPageNum;
        }
        else
        {
            nowPage = 0;
            tutorialImage.sprite = tutorialImageList[nowPage];
            /*
            tutorialDescription.text = tutorialDescriptionList[nowPage];
            tutorialPageNum = $"{nowPage + 1} / {tutorialImageList.Length}";
            tutorialPage.text = tutorialPageNum;
            */
        }
    }

    public void prevTutorialPage()
    {
        if(nowPage > 0)
        {
            nowPage--;
            tutorialImage.sprite = tutorialImageList[nowPage];
            /*
            tutorialDescription.text = tutorialDescriptionList[nowPage];
            tutorialPageNum = $"{nowPage + 1} / {tutorialImageList.Length}";
            tutorialPage.text = tutorialPageNum;\
            */
        }
        else
        {
            nowPage = tutorialImageList.Length - 1;
            tutorialImage.sprite = tutorialImageList[nowPage];
            /*
            tutorialDescription.text = tutorialDescriptionList[nowPage];
            tutorialPageNum = $"{nowPage + 1} / {tutorialImageList.Length}";
            tutorialPage.text = tutorialPageNum;
            */
        }
    }

    public void closeTutorial()
    {
        gameObject.SetActive(false);
    }
}
