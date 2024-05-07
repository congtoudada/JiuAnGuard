/****************************************************
  文件：WebRequest.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 16:41:12
  功能：
*****************************************************/

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TEngine;
using UnityEngine;
using UnityEngine.Networking;

namespace GameLogic
{
    public static class WebURL
    {
        public const string BASE_URL = "http://127.0.0.1:9012/unity/";

        public static string GetFullURL(string url)
        {
            return BASE_URL + url;
        }
        
        // /// <summary>
        // /// Get请求
        // /// </summary>
        // public static async UniTaskVoid Get(string url, Action<string> callback)
        // {
        //     UnityWebRequest www = UnityWebRequest.Get(url);
        //
        //     await www.SendWebRequest();
        //     if (www.result == UnityWebRequest.Result.Success)
        //     {
        //         // Log.Info("Received info | " + www.downloadHandler.text);
        //         callback?.Invoke(www.downloadHandler.text);
        //     }
        //     else
        //     {
        //         Log.Error("Error | " + www.error);
        //     }
        // }
        //
        // /// <summary>
        // /// Post请求
        // /// </summary>
        // public static async UniTaskVoid Get(string url, Action<string> callback, WWWForm form)
        // {
        //     UnityWebRequest www = UnityWebRequest.Post("服务器api接口", form);
        //
        //     await www.SendWebRequest();
        //     if (www.result == UnityWebRequest.Result.Success)
        //     {
        //         // Log.Info("Received info | " + www.downloadHandler.text);
        //         callback?.Invoke(www.downloadHandler.text);
        //     }
        //     else
        //     {
        //         Log.Error("Error | " + www.error);
        //     }
        // }
        //
        // /// <summary>
        // /// Post请求
        // /// </summary>
        // public static async UniTaskVoid Get(string url, Action<string> callback, Dictionary<string, string> data)
        // {
        //     WWWForm form = new WWWForm();
        //     foreach (var pair in data)
        //     {
        //         form.AddField(pair.Key, pair.Value);
        //     }
        //     UnityWebRequest www = UnityWebRequest.Post("服务器api接口", form);
        //     await www.SendWebRequest();
        //     if (www.result == UnityWebRequest.Result.Success)
        //     {
        //         // Log.Info("Received info | " + www.downloadHandler.text);
        //         callback?.Invoke(www.downloadHandler.text);
        //     }
        //     else
        //     {
        //         Log.Error("Error | " + www.error);
        //     }
        // }
    }
}