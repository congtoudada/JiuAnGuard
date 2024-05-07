/****************************************************
  文件：UICameraWindow_Designer.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 15:18:40
  功能：
*****************************************************/

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    public partial class UICameraWindow : UISecondWindow
    {
        private List<RspCameraInfoDTO> _camInfoList;
        /// <summary>
        /// 请求摄像头列表
        /// </summary>
        public async UniTaskVoid SendReq_CameraList()
        {
            string data = null;
            try
            {
                data = await Utility.Http.Get(WebURL.GetFullURL("camera_list"));
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }
            if (string.IsNullOrEmpty(data)) return;
            _camInfoList = JsonConvert.DeserializeObject<List<RspCameraInfoDTO>>(data);
            for (int i = 0; i < _camInfoList.Count; i++)
            {
                //初始化顶部按钮
                var toggleItem = m_scrollRectTopToggleGroup.Add(TOGGLE_ITEM); //按钮
                toggleItem.GetComponent<Toggle>().group = m_scrollRectTopToggleGroup.content.GetComponent<ToggleGroup>();
                toggleItem.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
                    "监控" + _camInfoList[i].id; //按钮名称
                var i1 = i; //索引（由lambda捕获）
                toggleItem.GetComponent<Toggle>().onValueChanged.AddListener(visible =>
                {
                    if (visible)
                        ChangePosText(_camInfoList[i1].pos);
                });
            }

            m_scrollRectTopToggleGroup.GetComponentInChildren<Toggle>().isOn = true;
        }

        private void ChangePosText(string pos)
        {
            m_textPos.text = "拍摄点：" + pos;
        }
        
    }
}

