/****************************************************
  文件：UIFaceWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:32:57
  功能：
*****************************************************/

using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI)]
    public class UIFaceWindow : UISecondWindow
    {
        private UIFaceWidget _faceWidget;
        protected override void BindMemberProperty()
        {
            //初始化大标题
            m_textTitle.text = "身份识别";
            
            //2.初始化菜单栏
            m_scrollRectTopToggleGroup.Clear();
            var record_toggle = m_scrollRectTopToggleGroup.Add(TOGGLE_ITEM).GetComponent<Toggle>();
            record_toggle.group = m_scrollRectTopToggleGroup.content.GetComponent<ToggleGroup>();
            record_toggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "记录";
            record_toggle.isOn = true;
            
            //3.创建Camera Widget
            _faceWidget = CreateWidgetByPath<UIFaceWidget>(m_goContainer.transform, "UIFaceWidget");
        }
    }
}