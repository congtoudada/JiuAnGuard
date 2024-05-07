/****************************************************
  文件：UICameraWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 15:18:05
  功能：
*****************************************************/

using System.Diagnostics;
using Cysharp.Threading.Tasks;
using TEngine;
using TMPro;

namespace GameLogic
{
    [Window(UILayer.UI)]
    public partial class UICameraWindow : UISecondWindow
    {
        private UICameraWidget _cameraWidget;
        private TextMeshProUGUI m_textPos;
        
        protected override void BindMemberProperty()
        {
            //初始化大标题
            m_textTitle.text = "监控";
            
            //2.初始化菜单栏
            m_scrollRectTopToggleGroup.Clear();
            m_textPos = transform.Find("SubPanel/m_textPos").GetComponent<TextMeshProUGUI>();
            SendReq_CameraList().Forget(); //异步发送Web请求，请求摄像头数据，初始化顶部菜单栏
            
            //3.创建Camera Widget
            _cameraWidget = CreateWidgetByPath<UICameraWidget>(m_goContainer.transform, "UICameraWidget");
        }

        
    }
}