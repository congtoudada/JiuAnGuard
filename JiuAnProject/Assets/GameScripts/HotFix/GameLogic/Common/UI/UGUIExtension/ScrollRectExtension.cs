/****************************************************
  文件：ScrollRectExtension.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 15:55:36
  功能：
*****************************************************/

using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public static class ScrollRectExtension
    {
        public static void Clear(this ScrollRect sr)
        {
            foreach (Transform item in sr.content)
            {
                Object.Destroy(item.gameObject);
            }
        }

        public static GameObject Add(this ScrollRect sr, GameObject obj)
        {
            obj.transform.parent = sr.content;
            return obj;
        }
        
        public static GameObject Add(this ScrollRect sr, string location)
        {
            return GameModule.Resource.LoadGameObject(location, sr.content);
        }
    }
}