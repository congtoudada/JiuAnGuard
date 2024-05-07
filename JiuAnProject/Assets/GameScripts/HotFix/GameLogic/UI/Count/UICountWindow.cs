/****************************************************
  文件：UICountWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:19:15
  功能：
*****************************************************/

using System.Collections.Generic;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI)]
    public class UICountWindow : UISecondWindow
    {
        private List<string> pageNames = new List<string>()
        {
            "实时计数", "记录", "统计组"
        };

        private List<Toggle> pageToggles = new List<Toggle>();
        protected override void BindMemberProperty()
        {
            //1.初始化大标题
            m_textTitle.text = "人员计数";
            m_scrollRectTopToggleGroup.Clear();
            
            //2.初始化菜单栏.
            CreateWidgetByPath<UICountRuntimeWidget>(m_goContainer.transform, nameof(UICountRuntimeWidget), false);
            CreateWidgetByPath<UICountRecordWidget>(m_goContainer.transform, nameof(UICountRecordWidget), false);
            CreateWidgetByPath<UICountGroupWidget>(m_goContainer.transform, nameof(UICountGroupWidget), false);
            m_scrollRectTopToggleGroup.Clear();
            for (int i = 0; i < pageNames.Count; i++)
            {
                var pageToggle = m_scrollRectTopToggleGroup.Add(TOGGLE_ITEM).GetComponent<Toggle>();
                pageToggle.group = m_scrollRectTopToggleGroup.content.GetComponent<ToggleGroup>();
                pageToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = pageNames[i];
                var i1 = i;
                pageToggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        ListChild[i1].Visible = true;
                    }
                    else
                    {
                        ListChild[i1].Visible = false;
                    }
                });
                pageToggles.Add(pageToggle);
            }

            pageToggles[0].isOn = true;
        }
    }
}