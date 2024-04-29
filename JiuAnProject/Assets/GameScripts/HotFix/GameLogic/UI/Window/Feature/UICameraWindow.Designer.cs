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
using GameLogic.UI;
using Newtonsoft.Json;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    public partial class UICameraWindow : UISecondWindow
    {
        private List<CameraInfoDTO> _camInfoList;
        /// <summary>
        /// 请求摄像头列表
        /// </summary>
        public async UniTaskVoid SendReq_CameraList()
        {
            string data = null;
            try
            {
                data = await Utility.Http.Get(WebConstant.GetFullURL("camera_list"));
            }
            catch (Exception e)
            {
                Log.Warning(e);
            }
            if (string.IsNullOrEmpty(data)) return;
            _camInfoList = JsonConvert.DeserializeObject<List<CameraInfoDTO>>(data);
            for (int i = 0; i < _camInfoList.Count; i++)
            {
                //初始化顶部按钮
                var btn = m_scrollRectTopBtn.Add(BUTTON_ITEM1); //按钮
                btn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
                    "监控" + _camInfoList[i].id; //按钮名称
                var i1 = i; //索引（由lambda捕获）
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ChangePosText(_camInfoList[i1].pos);
                });
            }
            ChangePosText(_camInfoList[0].pos);
        }

        private void ChangePosText(string pos)
        {
            m_textPos.text = "拍摄点：" + pos;
        }
        
    }
}

