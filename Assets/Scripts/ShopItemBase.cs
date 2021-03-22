using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemBase : MonoBehaviour
{
    private string imagePath = "Assets/StreamingAssets/";
    protected Sprite setImage(string name)
    {
        double startTime = (double) Time.time;
        //创建文件读取流
        if (name == "")
        {
            name = "shop_lock";
        }

        string Path = imagePath + name + ".png";

        FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
 
        //释放文件读取流
        fileStream.Close();
        //释放本机屏幕资源
        fileStream.Dispose();
        fileStream = null;
 
        //创建Texture
        int width = 222;
        int height = 277;
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);
 
        //创建Sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        startTime = (double)Time.time - startTime;
        Debug.Log("IO加载" + startTime);

        return sprite;
    }
    
    protected string GetTitle(int type, int subtype)
    {
        string title;
        switch (type)
        {
            case 1: title = "Coins";
                return title;
            case 2: title = "Diamonds";
                return title;
            case 8: title = "";
                return title;
            default:
                break;
        }
        switch (subtype)
        {
            case 7:
                title = "Taurus Withcer";
                break;
            case 13:
                title = "Fire Mage";
                break;
            case 18:
                title = "Goblikazes";
                break;
            case 20:
                title = "Demon";
                break;
            default:
                title = "";
                break;
        }
        return title;
    }
}

