/****************************************************
  文件：UICountWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:19:15
  功能：
*****************************************************/

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI)]
    public class UICountWindow : UISecondWindow
    {
        protected override List<string> pageNames { get; } = new List<string>()
        {
            "实时计数", "记录", "统计组"
        };

        protected override string title { get; } = "人员计数";
        
        protected override void BindMemberProperty()
        {
            this.HideTimeToClose = 24 * 3600;
            //初始化菜单栏.
            CreateWidgetByPath<UICountRuntimeWidget>(m_goContainer.transform, nameof(UICountRuntimeWidget), false);
            CreateWidgetByPath<UICountRecordWidget>(m_goContainer.transform, nameof(UIRecordWidget), false);
            CreateWidgetByPath<UICountGroupWidget>(m_goContainer.transform, nameof(UICountGroupWidget), false);
        }
    }
}