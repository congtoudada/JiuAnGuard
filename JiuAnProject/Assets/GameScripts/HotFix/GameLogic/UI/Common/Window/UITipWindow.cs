using System;
using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Windows;
using File = System.IO.File;
using Object = UnityEngine.Object;

namespace GameLogic
{
    public class UITipData : MemoryObject
    {
        public string title = "";
        public string img_url = "";
        public string main_text = "";
        public string sub_text = "";
        public UnityAction confirmCallback = null;

        public void Init(string title = "", string img_url = "", string main_text = "", string sub_text = "",
            UnityAction confirmCallback = null)
        {
            this.title = title;
            this.img_url = img_url;
            this.main_text = main_text;
            this.sub_text = sub_text;
            this.confirmCallback = confirmCallback;
        }
        public override void RecycleToPool()
        {
            title = "";
            img_url = "";
            main_text = "";
            sub_text = "";
            UnityAction confirmCallback = null;
        }
    }
    
    [Window(UILayer.Tips)]
    class UITipWindow : UIWindow
    {
        #region 脚本工具生成的代码
        private TextMeshProUGUI m_textTitle;
        private Button m_btnBack;
        private RawImage m_rimgMain;
        private TextMeshProUGUI m_textMain;
        private TextMeshProUGUI m_textSub;
        private Button m_btnConfirm;
        private Button m_btnCancel;
        protected override void ScriptGenerator()
        {
            m_textTitle = FindChildComponent<TextMeshProUGUI>("Tip/m_textTitle");
            m_btnBack = FindChildComponent<Button>("Tip/m_btnBack");
            m_rimgMain = FindChildComponent<RawImage>("Tip/Center/MainInfo/m_rimgMain");
            m_textMain = FindChildComponent<TextMeshProUGUI>("Tip/Center/MainInfo/m_textMain");
            m_textSub = FindChildComponent<TextMeshProUGUI>("Tip/Center/m_textSub");
            m_btnConfirm = FindChildComponent<Button>("Tip/m_btnConfirm");
            m_btnCancel = FindChildComponent<Button>("Tip/m_btnCancel");
            m_btnBack.onClick.AddListener(UniTask.UnityAction(OnClickBackBtn));
            m_btnCancel.onClick.AddListener(UniTask.UnityAction(OnClickCancelBtn));
        }
        #endregion

        #region 事件
        private async UniTaskVoid OnClickBackBtn()
        {
            GameModule.UI.CloseUI<UITipWindow>();
        }
        private async UniTaskVoid OnClickCancelBtn()
        {
            GameModule.UI.CloseUI<UITipWindow>();
        }
        #endregion

        private Action confirmCallback;
        protected override async void OnRefresh()
        {
            Log.Info("UITipWindow Refresh!");
            if (UserData == null) return;
            UITipData data = (UITipData)UserData;
            m_textTitle.text = data.title;
            m_textMain.text = data.main_text;
            m_textSub.text = data.sub_text;
            if (!string.IsNullOrEmpty(data.img_url) && File.Exists(data.img_url))
            {
                m_rimgMain.enabled = true;
                var aspectRatioFitter = m_rimgMain.gameObject.GetComponent<AspectRatioFitter>();
                if (aspectRatioFitter)
                {
                    Object.Destroy(aspectRatioFitter);
                }
                var texture = await Utility.Http.GetTexture(data.img_url);
                if (texture)
                {
                    float aspectRatio = (float) texture.width / texture.height;
                    m_rimgMain.texture = texture;
                    aspectRatioFitter = m_rimgMain.gameObject.AddComponent<AspectRatioFitter>();
                    aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                    aspectRatioFitter.aspectRatio = aspectRatio;
                }
            }
            else
            {
                m_rimgMain.enabled = false;
            }
            m_btnConfirm.onClick.RemoveAllListeners();
            if (m_textSub.text == "")
            {
                m_textSub.gameObject.SetActive(false);
            }
            else
            {
                m_textSub.gameObject.SetActive(true);
            }
            if (data.confirmCallback != null)
            {
                m_btnConfirm.onClick.AddListener(() =>
                {
                    data.confirmCallback();
                    GameModule.UI.HideUI<UITipWindow>();
                });
            }
            else
            {
                m_btnConfirm.onClick.AddListener(()=>{GameModule.UI.HideUI<UITipWindow>();});
            }
        }

        public static void Show(string title = "", string img_url = "", string main_text = "", string sub_text = "",
            UnityAction confirmCallback = null)
        {
            UITipData data = MemoryPool.Acquire<UITipData>();
            data.Init(title, img_url, main_text, sub_text, confirmCallback);
            GameModule.UI.ShowUIAsync<UITipWindow>(data);
            MemoryPool.Release(data);
        }
    }
}