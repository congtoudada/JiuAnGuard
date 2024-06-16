/****************************************************
  文件：UIFaceWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:32:57
  功能：
*****************************************************/

using System.Collections.Generic;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI)]
    public class UIFaceWindow : UISecondWindow
    {
        protected override List<string> pageNames { get; } = new List<string>()
        {
            "记录"
        };

        protected override string title { get; } = "身份识别";

        protected override void BindMemberProperty()
        {
            //初始化菜单栏.
            CreateWidgetByPath<UIFaceRecordWidget>(m_goContainer.transform, nameof(UIRecordWidget), false);
        }
    }
}