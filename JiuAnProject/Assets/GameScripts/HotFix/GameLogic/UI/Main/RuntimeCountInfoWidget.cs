using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;

namespace GameLogic
{
    class RuntimeCountInfoWidget : UIWidget
    {
        #region 脚本工具生成的代码
        private TextMeshProUGUI m_textCountTitle;
        private TextMeshProUGUI m_textCountIn;
        private TextMeshProUGUI m_textCountOut;
        private TextMeshProUGUI m_textCountRemain;
        protected override void ScriptGenerator()
        {
            m_textCountTitle = FindChildComponent<TextMeshProUGUI>("CountInfo/m_textCountTitle");
            m_textCountIn = FindChildComponent<TextMeshProUGUI>("CountInfo/Horizontal/m_textCountIn");
            m_textCountOut = FindChildComponent<TextMeshProUGUI>("CountInfo/Horizontal1/m_textCountOut");
            m_textCountRemain = FindChildComponent<TextMeshProUGUI>("CountInfo/Horizontal2/m_textCountRemain");
        }
        #endregion
        
        protected override void OnCreate()
        {
            UIGlobalDataInstance.Instance.OnCountChanged += OnCountChangedCallback;
            UIGlobalDataInstance.Instance.OnGroupIDChanged += OnGroupIDChangedCallback;
            OnGroupIDChangedCallback(UIGlobalDataInstance.Instance.CurrentGroupID);
            OnCountChangedCallback(0, 0, 0);
        }

        protected override void OnDestroy()
        {
            UIGlobalDataInstance.Instance.OnCountChanged -= OnCountChangedCallback;
            UIGlobalDataInstance.Instance.OnGroupIDChanged -= OnGroupIDChangedCallback;
        }

        private void OnGroupIDChangedCallback(string groupID)
        {
            m_textCountTitle.text = groupID;
        }
        private void OnCountChangedCallback(int inCount, int outCount, int remainCount)
        {
            m_textCountIn.text = inCount.ToString();
            m_textCountOut.text = outCount.ToString();
            m_textCountRemain.text = remainCount.ToString();
        }
        
    }
}