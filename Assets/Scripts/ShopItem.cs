using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : ShopItemBase
{
    [SerializeField] private Image bgImg;
    [SerializeField] private Text title;
    [SerializeField] private Image displayObj;
    [SerializeField] private Text num;
    [SerializeField] private GameObject btnImg;
    [SerializeField] private Text btText;
    [SerializeField] private GameObject lockObj;
    [SerializeField] private GameObject btnObj;
    [SerializeField] private GameObject collected;
    [SerializeField] private GameObject btnFrame;
    [SerializeField] private Sprite[] bgImgs;
    public void SetContent(DailyProduct dailyData)
    {
        string titletext;
        titletext = GetTitle(dailyData.type, dailyData.subType);
        displayObj.sprite = setImage(titletext);
        title.text = titletext;
        if (dailyData.type == (int) RewardType.Locked)
        {
            bgImg.sprite = bgImgs[1];
            displayObj.GetComponent<RectTransform>().sizeDelta=new Vector2(69f,95f);
            lockObj.gameObject.SetActive(true);
            btnObj.gameObject.SetActive(false);
            return;
        }
        if (dailyData.costGold != 0)
        {
            btText.text = $"{dailyData.costGold}";
            num.gameObject.SetActive(true);
            num.text = $"x{dailyData.num}";
            bgImg.sprite = bgImgs[1];
        }
        else
        {
            bgImg.sprite = bgImgs[0];
            Destroy(btnImg);
            btText.text = "FREE!";
            title.text="<color=#883600>"+titletext+"</color>";
        }
    }

    private void Start()
    {
        btnImg.transform.SetParent(btnFrame.transform,false);
        btText.transform.SetParent(btnFrame.transform,false);
    }

    private int getSoldierImg(int subtype)
    {
        switch (subtype)
        {
            case 7: return 0;
            case 13: return 1;
            case 18: return 2;
            case 20: return 3;
        }
        return -1;
    }

    public void onclick()
    {
        btnObj.SetActive(false);
        collected.SetActive(true);
    }
}
