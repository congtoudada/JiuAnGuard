using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TEngine;

namespace GameLogic
{
    [Window(UILayer.UI)]
    class UIMainWindow : UIWindow
    {
        
        #region 脚本工具生成的代码
        private Button m_btnWarn;
        private Button m_btnWander;
        private Button m_btnEdit;
        protected override void ScriptGenerator()
        {
            m_btnWarn = FindChildComponent<Button>("Head/m_btnWarn");
            m_btnWander = FindChildComponent<Button>("Head/m_btnWander");
            m_btnEdit = FindChildComponent<Button>("Head/m_btnEdit");
            m_btnWarn.onClick.AddListener(UniTask.UnityAction(OnClickWarnBtn));
            m_btnWander.onClick.AddListener(UniTask.UnityAction(OnClickWanderBtn));
            m_btnEdit.onClick.AddListener(UniTask.UnityAction(OnClickEditBtn));
        }
        #endregion

        #region 事件
        private async UniTaskVoid OnClickWarnBtn()
        {
            await UniTask.Yield();
        }
        private async UniTaskVoid OnClickWanderBtn()
        {
            await UniTask.Yield();
        }
        private async UniTaskVoid OnClickEditBtn()
        {
            await UniTask.Yield();
        }
        #endregion
        
        #region 自定义
        
        
        //创建面板时调用一次
        protected override void OnCreate()
        {
            
        }
        #endregion
    }
}
