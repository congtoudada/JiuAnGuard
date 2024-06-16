/****************************************************
  文件：UICameraWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 15:18:05
  功能：
*****************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI)]
    public partial class UICameraWindow : UISecondWindow
    {
        private TextMeshProUGUI m_textPos;
        private UICameraWidget _cameraWidget;
        protected override string title { get; } = "监控";
        
        protected override async void BindMemberProperty()
        {
            //创建Camera Widget
            _cameraWidget = CreateWidgetByPath<UICameraWidget>(m_goContainer.transform, "UICameraWidget");
            //初始化菜单栏
            m_textPos = transform.Find("SubPanel/m_textPos").GetComponent<TextMeshProUGUI>();
            //SendReq_CameraList().Forget(); //异步发送Web请求，请求摄像头数据，初始化顶部菜单栏
            if (UIGlobalDataInstance.Instance.CameraInfoDict.Count == 0)
            {
                bool ret = await UIGlobalDataInstance.Instance.SendCameraInfoReq();
                if (!ret) return;
            }

            RspCameraInfoDTO[] cameraList = UIGlobalDataInstance.Instance.CameraInfoDict.Values.ToArray();
            m_scrollRectTopToggleGroup.Clear();
            for (int i = 0; i < cameraList.Length; i++)
            {
                //初始化顶部按钮
                var toggleItem = m_scrollRectTopToggleGroup.Add(TOGGLE_ITEM); //按钮
                toggleItem.name = cameraList[i].id.ToString();
                toggleItem.GetComponent<Toggle>().group = m_scrollRectTopToggleGroup.content.GetComponent<ToggleGroup>();
                toggleItem.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
                    "监控" + cameraList[i].id; //按钮名称
                var i1 = Convert.ToInt32(toggleItem.name); //索引（由lambda捕获）
                toggleItem.GetComponent<Toggle>().onValueChanged.AddListener(visible =>
                {
                    if (visible)
                    {
                        RspCameraInfoDTO info = UIGlobalDataInstance.Instance.CameraInfoDict[i1];
                        m_textPos.text = info.pos;
                        _cameraWidget.Refresh(info);
                    }
                });
            }
            m_scrollRectTopToggleGroup.GetComponentInChildren<Toggle>().isOn = true;

            HideTimeToClose = 0;
        }
    }
}