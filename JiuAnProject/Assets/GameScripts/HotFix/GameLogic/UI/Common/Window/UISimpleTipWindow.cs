using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;

namespace GameLogic
{
    public class UISimpleTipData : MemoryObject
    {
        private string main_text;
        private float duration;

        public string MainText
        {
            get => main_text;
            set => main_text = value;
        }

        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        public override void InitFromPool()
        {
            main_text = "";
            duration = 1.5f;
        }
    }

    [Window(UILayer.Tips)]
    class UISimpleTipWindow : UIWindow
    {
        #region 脚本工具生成的代码
        private TextMeshProUGUI m_textMain;
        protected override void ScriptGenerator()
        {
            m_textMain = FindChildComponent<TextMeshProUGUI>("Tip/m_textMain");
        }
        #endregion

        private CancellationTokenSource cts; //取消Token初始化
        protected override void OnRefresh()
        {
            if (UserData == null) return;
            if (cts != null) cts.Cancel();
            cts = new CancellationTokenSource();
            UISimpleTipData data = (UISimpleTipData)UserData;
            m_textMain.text = data.MainText;
            AutoClose(data.Duration, cts).Forget();
        }

        private async UniTaskVoid AutoClose(float duration, CancellationTokenSource cts)
        {
            var ret = await UniTask.WaitForSeconds(duration, cancellationToken: cts.Token).SuppressCancellationThrow();
            if (!ret) //ret表示是否取消
            {
                GameModule.UI.HideUI<UISimpleTipWindow>();
            }
        }

        public static void Show(string content, float duration = 1.5f)
        {
            var data = MemoryPool.Alloc<UISimpleTipData>();
            data.MainText = content;
            data.Duration = duration;
            GameModule.UI.ShowUIAsync<UISimpleTipWindow>(data);
            MemoryPool.Dealloc(data);
        }
    }
}

