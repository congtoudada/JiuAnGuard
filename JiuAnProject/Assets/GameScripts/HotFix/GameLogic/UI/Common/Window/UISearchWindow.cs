/****************************************************
  文件：UISearchWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月02日 17:44:50
  功能：
*****************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;

namespace GameLogic
{
    [Window(UILayer.UI)]
    class UISearchWindow : UIWindow
    {
        [Serializable]
        public class ReIDInfo
        {
            public string shotImg;
            public string camId;
            public string pos;
            public string recordTime;
        }

        private List<TextMeshProUGUI> m_textArray = new List<TextMeshProUGUI>();
        private List<RawImage> m_rimgArray = new List<RawImage>();
        #region 脚本工具生成的代码
        private TextMeshProUGUI m_textTitle;
        private TMP_InputField m_inputID;
        private Button m_btnBack;
        private RawImage m_rimgMain;
        private TextMeshProUGUI m_textSub;
        private RawImage m_rimgMain1;
        private TextMeshProUGUI m_textSub1;
        private RawImage m_rimgMain2;
        private TextMeshProUGUI m_textSub2;
        private RawImage m_rimgMain3;
        private TextMeshProUGUI m_textSub3;
        private Button m_btnConfirm;
        private Button m_btnCancel;
        private GameObject m_goUILoadingWidget;
        protected override void ScriptGenerator()
        {
            m_textTitle = FindChildComponent<TextMeshProUGUI>("Tip/m_textTitle");
            m_inputID = FindChildComponent<TMP_InputField>("Tip/m_inputID");
            m_btnBack = FindChildComponent<Button>("Tip/m_btnBack");
            m_rimgMain = FindChildComponent<RawImage>("Tip/Center/MainInfo/m_rimgMain");
            m_textSub = FindChildComponent<TextMeshProUGUI>("Tip/Center/MainInfo/m_rimgMain/m_textSub");
            m_rimgMain1 = FindChildComponent<RawImage>("Tip/Center/MainInfo/m_rimgMain1");
            m_textSub1 = FindChildComponent<TextMeshProUGUI>("Tip/Center/MainInfo/m_rimgMain1/m_textSub1");
            m_rimgMain2 = FindChildComponent<RawImage>("Tip/Center/MainInfo/m_rimgMain2");
            m_textSub2 = FindChildComponent<TextMeshProUGUI>("Tip/Center/MainInfo/m_rimgMain2/m_textSub2");
            m_rimgMain3 = FindChildComponent<RawImage>("Tip/Center/MainInfo/m_rimgMain3");
            m_textSub3 = FindChildComponent<TextMeshProUGUI>("Tip/Center/MainInfo/m_rimgMain3/m_textSub3");
            m_btnConfirm = FindChildComponent<Button>("Tip/m_btnConfirm");
            m_btnCancel = FindChildComponent<Button>("Tip/m_btnCancel");
            m_goUILoadingWidget = FindChild("m_goUILoadingWidget").gameObject;
            m_btnBack.onClick.AddListener(UniTask.UnityAction(OnClickBackBtn));
            m_btnConfirm.onClick.AddListener(UniTask.UnityAction(OnClickConfirmBtn));
            m_btnCancel.onClick.AddListener(UniTask.UnityAction(OnClickCancelBtn));
            
            m_textArray.Add(m_textSub);
            m_textArray.Add(m_textSub1);
            m_textArray.Add(m_textSub2);
            m_textArray.Add(m_textSub3);
            
            m_rimgArray.Add(m_rimgMain);
            m_rimgArray.Add(m_rimgMain1);
            m_rimgArray.Add(m_rimgMain2);
            m_rimgArray.Add(m_rimgMain3);
        }
        #endregion
        
        #region 事件
        private async UniTaskVoid OnClickBackBtn()
        {
            Close();
        }
        private async UniTaskVoid OnClickConfirmBtn()
        {
            if (string.IsNullOrEmpty(m_inputID.text))
            {
                UISimpleTipWindow.Show("请输入ID!");
                return;
            }

            try
            {
                //发送查询请求
                WWWForm form = new WWWForm();
                m_goUILoadingWidget.SetActive(true);
                // 发送查询请求
                form.AddField("query_id", m_inputID.text);
                string json = await Utility.Http.Post(WebURL.GetReidURL(), form, 60); //图片路径
                Log.Info("Search Person Response: " + json);
                m_goUILoadingWidget.SetActive(false);
                if (!string.IsNullOrEmpty(json))
                {
                    Log.Info("收到reid响应" + json);
                    List<ReIDInfo> reidList = JsonConvert.DeserializeObject<List<ReIDInfo>>(json);
                    if (reidList.Count == 0)
                    {
                        UISimpleTipWindow.Show("未找到该用户最近抓拍图像");
                    }
                    else
                    {
                        for (int i = 0; i < reidList.Count; i++)
                        {
                            SetText(i, $"{reidList[i].camId}.{reidList[i].pos}", reidList[i].recordTime);
                            SetTexture(i, reidList[i].shotImg);
                        }
                    }
                }
                else
                {
                    UISimpleTipWindow.Show("Reid服务响应异常！");
                }
            }
            catch (Exception e)
            {
                UISimpleTipWindow.Show("Reid服务请求异常！");
            }
           
        }
        private async UniTaskVoid OnClickCancelBtn()
        {
            Close();
        }
        #endregion
        
        protected override void OnCreate()
        {
            if (m_goUILoadingWidget.activeSelf)
            {
                m_goUILoadingWidget.SetActive(false);
            }
            m_textTitle.text = "人员搜索";
            m_textSub.text = "暂无信息";
            for (int i = 0; i < m_textArray.Count; i++)
            {
                m_textArray[i].text = "";
                // m_rimgArray[i].texture = null;
            }
            m_goUILoadingWidget.SetActive(false);
        }

        private void SetText(int idx, string camId, string timeStr)
        {
            if (idx < 0 || idx >= m_textArray.Count) return;
            m_textArray[idx].text = $"{camId}\n{timeStr}";
        }

        private async void SetTexture(int idx, string shotImg)
        {
            if (idx < 0 || idx >= m_rimgArray.Count) return;
            m_rimgArray[idx].texture = await Utility.Http.GetTexture(shotImg);
        }
        
    }
}

