/****************************************************
  文件：UISearchWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月02日 17:44:50
  功能：
*****************************************************/

using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;

namespace GameLogic
{
    [Window(UILayer.UI)]
    class UISearchWindow : UIWindow
    {
        #region 脚本工具生成的代码
        private TextMeshProUGUI m_textTitle;
        private Button m_btnBack;
        private RawImage m_rimgMain;
        private TextMeshProUGUI m_textMain;
        private TMP_InputField m_inputID;
        private TextMeshProUGUI m_textSub;
        private Button m_btnConfirm;
        private Button m_btnCancel;
        private GameObject m_goUILoadingWidget;
        protected override void ScriptGenerator()
        {
            m_textTitle = FindChildComponent<TextMeshProUGUI>("Tip/m_textTitle");
            m_btnBack = FindChildComponent<Button>("Tip/m_btnBack");
            m_rimgMain = FindChildComponent<RawImage>("Tip/Center/MainInfo/m_rimgMain");
            m_textMain = FindChildComponent<TextMeshProUGUI>("Tip/Center/MainInfo/m_textMain");
            m_inputID = FindChildComponent<TMP_InputField>("Tip/Center/Vertical/m_inputID");
            m_textSub = FindChildComponent<TextMeshProUGUI>("Tip/Center/Vertical/m_textSub");
            m_btnConfirm = FindChildComponent<Button>("Tip/m_btnConfirm");
            m_btnCancel = FindChildComponent<Button>("Tip/m_btnCancel");
            m_goUILoadingWidget = FindChild("m_goUILoadingWidget").gameObject;
            m_btnBack.onClick.AddListener(UniTask.UnityAction(OnClickBackBtn));
            m_btnConfirm.onClick.AddListener(UniTask.UnityAction(OnClickConfirmBtn));
            m_btnCancel.onClick.AddListener(UniTask.UnityAction(OnClickCancelBtn));
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
            //发送查询请求
            WWWForm form = new WWWForm();
            // 发送查询请求
            form.AddField("query_directory_or_id", m_inputID.text);
            string json = await Utility.Http.Post(WebURL.GetReidURL(), form); //图片路径
            //TODO:解析响应的内容
            if (json != null)
            {
                Log.Info("收到reid响应" + json);
                string filename = Path.GetFileNameWithoutExtension(json);
            }
            else
            {
                UISimpleTipWindow.Show("Reid服务异常！");
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
        }
    }
}

