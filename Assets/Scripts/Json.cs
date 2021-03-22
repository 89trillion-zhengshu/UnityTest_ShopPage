using UnityEngine;
using LitJson;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

public class Json : MonoBehaviour
{
    private static string filePath = "Assets/Resources/data.json";
    private void Start()
    {
    }

    public static JsonInfo ReadJson(){
        //文件流 读取
        StreamReader sr = new StreamReader(filePath);

        //读取的Json字符串
        string readContent = sr.ReadToEnd();

        //反序列化操作
        JsonInfo json = JsonMapper.ToObject<JsonInfo>(readContent);

        return json;
    }


    
}
