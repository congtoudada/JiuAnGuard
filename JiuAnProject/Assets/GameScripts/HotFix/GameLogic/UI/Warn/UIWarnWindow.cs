/****************************************************
  文件：UIWarnWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:33:08
  功能：
*****************************************************/

using System.Collections.Generic;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI)]
    public class UIWarnWindow : UISecondWindow
    {
        protected override List<string> pageNames { get; } = new List<string>()
        {
            "记录"
        };

        protected override string title { get; } = "报警统计";

        protected override void BindMemberProperty()
        {
            //初始化菜单栏.
            CreateWidgetByPath<UIWarnRecordWidget>(m_goContainer.transform, nameof(UIRecordWidget), false);
        }
    }
}