/****************************************************
  文件：UISecondWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 15:32:08
  功能：任务：1.初始化顶部菜单栏 2.初始化大标题 3.创建专属控件
*****************************************************/

using Cysharp.Threading.Tasks;
using TEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public abstract class UISecondWindow : UIWindow
    {

        protected const string TOGGLE_ITEM = "ToogleItem";
        
        #region 脚本工具生成的代码
        protected ScrollRect m_scrollRectTopToggleGroup;
        protected TextMeshProUGUI m_textTitle;
        private Button m_btnClose;
        protected GameObject m_goContainer;
        protected override void ScriptGenerator()
        {
            m_scrollRectTopToggleGroup = FindChildComponent<ScrollRect>("TopMenu/m_scrollRectTopToggleGroup");
            m_textTitle = FindChildComponent<TextMeshProUGUI>("SubPanel/m_textTitle");
            m_btnClose = FindChildComponent<Button>("SubPanel/m_btnClose");
            m_goContainer = FindChild("SubPanel/m_goContainer").gameObject;
            m_btnClose.onClick.AddListener(UniTask.UnityAction(OnClickCloseBtn));
        }
        #endregion

        #region 事件
        private async UniTaskVoid OnClickCloseBtn()
        {
            GameModule.UI.HideUI(this.GetType());
        }
        #endregion
    }
}