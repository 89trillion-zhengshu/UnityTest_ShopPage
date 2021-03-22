using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using script;
using TMPro;
using UnityEngine.UI;

public class ShopDialog : MonoBehaviour
{
    [SerializeField] private Text Countdown;
    [SerializeField] private RectTransform container;
    [SerializeField] private ShopItem shopItemPrefab;
    private List<RectTransform> ItemGroups;
    private List<ShopItem> ShopItems;
    private JsonInfo json;
    private int curCountTime;
    public static void Init()
    {
        GameObject ShopDialog = (GameObject)Resources.Load("Prefabs/ShopDialog");
        Instantiate(ShopDialog);
    }
    // Start is called before the first frame update

    private void Start()
    {
        LoadData();
        LoadCardItems();
        StartCoroutine(CountDownTimer());
    }

    private void LoadCardItems()
    {
        foreach (var dailyProduct in json.dailyProduct)
        {
            ShopItem item = Instantiate(shopItemPrefab, container);
            item.SetContent(dailyProduct);
        }
        
        
        curCountTime = TimeUtil.GetTimeSec(json.dailyProductCountDown);
    }
    
    private void LoadData()
    {
        json= Json.ReadJson();
    }
    
    IEnumerator CountDownTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            curCountTime -= 1;
            Countdown.text = "Refresh time: " + $"{TimeUtil.TimeFormatInHrsMinSec(curCountTime)}";
        }
    }

}
