/*
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image bgImg;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject currency;
    [SerializeField] private GameObject soldier;
    [SerializeField] private Image soldierImage;
    [SerializeField] private TextMeshProUGUI num;
    [SerializeField] private GameObject btnImg;
    [SerializeField] private TextMeshProUGUI btText;
    [SerializeField] private GameObject openObj;
    [SerializeField] private GameObject lockObj;
    [SerializeField] private GameObject btnObj;
    [SerializeField] private GameObject slectObj;
    [SerializeField] private Sprite[] bgImgs;
    [SerializeField] private Sprite[] soldierImgs;
    
    private int nums;
    private int isPurchased;
    private int costGold;

    public enum DailyRewardType
    {
        None = -1,
        Trophy = 0,
        Diamonds = 1,
        Coins = 2,
        Cards = 3,
        Chest = 4,
        BattlePassExp = 5,
        MasterLV = 6,
        MasterEXP = 7
    }

    private void Update()
    {
        if (purchased())
        {
            btnObj.gameObject.SetActive(false);
            slectObj.gameObject.SetActive(true);
        }
    }

    public void SetUp(DailyProduct dailyData)
    {
        if (dailyData == null)
        {
            bgImg.sprite = bgImgs[1];
            lockObj.gameObject.SetActive(true);
            openObj.gameObject.SetActive(false);
            return;
        }
        
        if (dailyData.type == (int)DailyRewardType.Coins && dailyData.isPurchased == -1)
        {
            lockObj.gameObject.SetActive(false);
            openObj.gameObject.SetActive(true);
            bgImg.sprite = bgImgs[0];
            title.text = "<color=#883600>Coins</color>";
            currency.gameObject.SetActive(true);
            soldier.gameObject.SetActive(false);
            nums = 1;
            isPurchased = dailyData.isPurchased;
            btnImg.gameObject.SetActive(false);
            btText.text = "FREE!";
        }
        else if(dailyData.type == (int)DailyRewardType.Cards && dailyData.isPurchased == -1)
        {
            int subtype = getSoldierImg(dailyData.subType);
            if (subtype != -1)
            {
                soldierImage.sprite = soldierImgs[subtype];
                lockObj.gameObject.SetActive(false);
                openObj.gameObject.SetActive(true);
                bgImg.sprite = bgImgs[1];
                title.text = "<color=#002363>ViKing Wattior</color>";
                currency.gameObject.SetActive(false);
                soldier.gameObject.SetActive(true);
                nums = dailyData.num;
                num.text = $"x{nums}";
                isPurchased = dailyData.isPurchased;
                btnImg.gameObject.SetActive(true);
                btText.text = $"{dailyData.costGold}";
            }
        }
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
        if (nums > 0)
        {
            nums -= 1;
            num.text = $"x{nums}";
        }
    }

    private bool purchased()
    {
        if (nums == 0)
        {
            isPurchased = 1;
            return true;
        }
        return false;
    }
}
*/
