/****************************************************
  文件：WebRequest.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 16:41:12
  功能：
*****************************************************/

namespace GameLogic
{
    public static class WebURL
    {
        // public const string BASE_URL = "http://127.0.0.1:9012/unity/";
        public const string WEB_URL = "http://localhost:8080/unity/";
        public const string SERVER_IP = "10.0.5.195";
        public const string SERVER_USERNAME = "ps";
        public const string SERVER_PASSWORD = "1";
        // public const string BASE_URL = "http://192.168.43.245:8080/unity/";
        // public const string BASE_URL = "http://210.30.97.235:8080/unity/";

        public static string GetFullURL(string url)
        {
            return WEB_URL + url;
        }

        public static string GetReidURL()
        {
            // return "http://210.30.97.234:5000/process";
            // return "http://localhost:5000/process";
            return $"http://localhost:8080/process";
        }
    }
}