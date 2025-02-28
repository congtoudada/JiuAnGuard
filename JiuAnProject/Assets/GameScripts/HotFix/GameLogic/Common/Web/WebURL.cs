/****************************************************
  文件：WebRequest.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 16:41:12
  功能：
*****************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Application = UnityEngine.Application;
using Path = System.IO.Path;

namespace GameLogic
{
    [Serializable]
    public class RemoteAddress
    {
        public string ServerIp;
        public string Username;
        public string Password;
    }
    public static class WebURL
    {
        private static List<RemoteAddress> _remoteList = null;
        public static string WEB_URL = "http://localhost:8080/unity/";
        public static List<RemoteAddress> RemoteList
        {
            get
            {
                if (_remoteList == null)
                {
                    //尝试从本地加载json
                    string path = Path.Combine(Application.streamingAssetsPath, "Remote.json");
                    if (!File.Exists(path))
                    {
                        _remoteList = new()
                        {
                            new RemoteAddress()
                            {
                                ServerIp = "10.0.5.190",
                                Username = "ps",
                                Password = "1"
                            }
                        };
                        string content = JsonConvert.SerializeObject(_remoteList);
                        File.WriteAllText(path, content);
                    }
                    else
                    {
                        string content = File.ReadAllText(path);
                        if (string.IsNullOrEmpty(content))
                        {
                            Debug.LogError("请确保文件非空: " + path);
                            return null;
                        }
                        _remoteList = JsonConvert.DeserializeObject<List<RemoteAddress>>(content);
                    }
                }
                return _remoteList;
            }
        }

        public static string GetFullURL(string url)
        {
            return WEB_URL + url;
        }

        public static string GetReidURL()
        {
            return $"http://localhost:8080/process";
        }
    }
}