using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TEngine;

namespace GameLogic
{
    [Window(UILayer.Tips)]
    class UISplashWindow : UIWindow
    {
        class UISplashData
        {
            public float duration = 1.0f;
            public Action callback = null;
        };
        #region 脚本工具生成的代码
        private CanvasGroup m_cgBG;
        protected override void ScriptGenerator()
        {
            m_cgBG = FindChildComponent<CanvasGroup>("m_cgBG");
        }
        #endregion

        protected override void OnCreate()
        {
            m_cgBG.alpha = 1;
            UISplashData splashData = UserData as UISplashData;
            if (splashData == null)
            {
                splashData = new UISplashData();
            }
            DOTween.To(() => m_cgBG.alpha, alpha => m_cgBG.alpha = alpha, 0, splashData.duration).OnComplete(() =>
            {
                splashData.callback?.Invoke();
                Close();
            });
        }

        public static void Show(float duration = 1f, Action callback = null)
        {
            UISplashData data = new UISplashData();
            data.duration = duration;
            data.callback = callback;
            GameModule.UI.ShowUI<UISplashWindow>(data);
        }
    }
}