/****************************************************
  文件：UIFaceWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:56:58
  功能：
*****************************************************/

using Cysharp.Threading.Tasks;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    public class UIFaceWidget : UIWidget
    {
        #region 脚本工具生成的代码
        private TextMeshProUGUI m_textBeginTime;
        private TextMeshProUGUI m_textEndTime;
        private TMP_Dropdown m_dpPos;
        private TMP_InputField m_inputName;
        private Button m_btnReset;
        private Button m_btnSearch;
        private ScrollRect m_scrollRectRecord;
        protected override void ScriptGenerator()
        {
            m_textBeginTime = FindChildComponent<TextMeshProUGUI>("Top/HorizontalGroup/DatePicker - Popup (Shared Calendar)/Input Field Container/Row/Cell/InputField/Text Area/m_textBeginTime");
            m_textEndTime = FindChildComponent<TextMeshProUGUI>("Top/HorizontalGroup/DatePicker - Popup (Shared Calendar) (1)/Input Field Container/Row/Cell/InputField/Text Area/m_textEndTime");
            m_dpPos = FindChildComponent<TMP_Dropdown>("Top/HorizontalGroup/m_dpPos");
            m_inputName = FindChildComponent<TMP_InputField>("Top/HorizontalGroup/m_inputName");
            m_btnReset = FindChildComponent<Button>("Top/HorizontalGroup/m_btnReset");
            m_btnSearch = FindChildComponent<Button>("Top/HorizontalGroup/m_btnSearch");
            m_scrollRectRecord = FindChildComponent<ScrollRect>("Main/m_scrollRectRecord");
            m_btnReset.onClick.AddListener(UniTask.UnityAction(OnClickResetBtn));
            m_btnSearch.onClick.AddListener(UniTask.UnityAction(OnClickSearchBtn));
        }
        #endregion

        #region 事件
        private async UniTaskVoid OnClickResetBtn()
        {
            await UniTask.Yield();
        }
        private async UniTaskVoid OnClickSearchBtn()
        {
            await UniTask.Yield();
        }
        #endregion
    }
}