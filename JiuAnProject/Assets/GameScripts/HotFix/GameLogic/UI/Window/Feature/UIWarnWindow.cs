/****************************************************
  文件：UIWarnWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:33:08
  功能：
*****************************************************/

using TEngine;
using TMPro;

namespace GameLogic
{
    [Window(UILayer.UI)]
    public class UIWarnWindow : UISecondWindow
    {
        private UIWarnWidget _warnWidget;
        protected override void OnCreate()
        {
            //初始化大标题
            m_textTitle.text = "报警统计";
            
            //2.初始化菜单栏
            m_scrollRectTopBtn.Clear();
            var record_btn = m_scrollRectTopBtn.Add(BUTTON_ITEM1);
            record_btn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "记录";
            
            //3.创建Camera Widget
            _warnWidget = CreateWidgetByPath<UIWarnWidget>(m_goContainer.transform, "UIWarnWidget");
        }
    }
}