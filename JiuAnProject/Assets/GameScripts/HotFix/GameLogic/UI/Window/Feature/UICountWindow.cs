/****************************************************
  文件：UICountWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:19:15
  功能：
*****************************************************/

using TEngine;
using TMPro;

namespace GameLogic
{
    [Window(UILayer.UI)]
    public class UICountWindow : UISecondWindow
    {
        private UICountWidget _countWidget;
        protected override void OnCreate()
        {
            //1.初始化大标题
            m_textTitle.text = "人员计数";
            m_scrollRectTopBtn.Clear();
            
            //2.初始化菜单栏
            m_scrollRectTopBtn.Clear();
            var runtime_btn = m_scrollRectTopBtn.Add(BUTTON_ITEM1);
            runtime_btn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "实时计数";
            
            var record_btn = m_scrollRectTopBtn.Add(BUTTON_ITEM1);
            record_btn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "记录";
            
            var group_btn = m_scrollRectTopBtn.Add(BUTTON_ITEM1);
            group_btn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "统计组";

            //3.创建Camera Widget
            _countWidget = CreateWidgetByPath<UICountWidget>(m_goContainer.transform, "UICountWidget");
        }
    }
}