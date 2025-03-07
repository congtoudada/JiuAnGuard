/****************************************************
  文件：LineScript.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2025年03月06日 21:36:53
  功能：
*****************************************************/

using System;
using UnityEngine;

namespace GameMain
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineScript : MonoBehaviour
    {
        public void SetLine(Vector3 startPoint, Vector3 endPoint)
        {
            var line = GetComponent<LineRenderer>();
            line.SetPosition(0, startPoint);
            line.SetPosition(1, endPoint);
        }
    }
}