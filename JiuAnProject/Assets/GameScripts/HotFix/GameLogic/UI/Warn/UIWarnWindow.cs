/****************************************************
  文件：UIWarnWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:33:08
  功能：
*****************************************************/

using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI)]
    public class UIWarnWindow : UISecondWindow
    {
        private UIWarnWidget _warnWidget;
        protected override void BindMemberProperty()
        {
            //初始化大标题
            m_textTitle.text = "报警统计";
            
            //2.初始化菜单栏
            m_scrollRectTopToggleGroup.Clear();
            var record_toggle = m_scrollRectTopToggleGroup.Add(TOGGLE_ITEM).GetComponent<Toggle>();
            record_toggle.group = m_scrollRectTopToggleGroup.content.GetComponent<ToggleGroup>();
            record_toggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "记录";
            record_toggle.isOn = true;
            
            //3.创建Camera Widget
            _warnWidget = CreateWidgetByPath<UIWarnWidget>(m_goContainer.transform, "UIWarnWidget");
        }
    }
}