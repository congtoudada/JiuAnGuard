/****************************************************
  文件：RuntimeWarnWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月09日 17:09:59
  功能：
*****************************************************/

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    public class RuntimeWarnWidget : UIWidget
    {
        private int _maxWarnCount = 150;
        private float _reqRuntimeWarnInterval = 1f;
        #region 脚本工具生成的代码
        private Transform m_goBG;
        private ScrollRect m_scrollRectRuntimeInfo;
        private Button m_btnAllKnown;
        private Button m_btnRightSwitch;
        protected override void ScriptGenerator()
        {
            m_goBG = FindChildComponent<Transform>("m_goBG");
            m_scrollRectRuntimeInfo = FindChildComponent<ScrollRect>("m_goBG/m_scrollRectRuntimeInfo");
            m_btnAllKnown = FindChildComponent<Button>("m_goBG/m_btnAllKnown");
            m_btnRightSwitch = FindChildComponent<Button>("m_btnRightSwitch");
            m_btnAllKnown.onClick.AddListener(UniTask.UnityAction(OnClickAllKnownBtn));
            m_btnRightSwitch.onClick.AddListener(UniTask.UnityAction(OnClickRightSwitchBtn));
        }
        #endregion

        #region 事件
        private async UniTaskVoid OnClickAllKnownBtn()
        {
            UITipWindow.Show("全部知晓", main_text: "该操作不会删除数据库记录，仅仅是在前端界面移除。确认删除？", confirmCallback: () =>
            {
                while (ListChild.Count != 0)
                {
                    ListChild[^1].Destroy();
                }
            });
        }
        private async UniTaskVoid OnClickRightSwitchBtn()
        {
            if (m_goBG.gameObject.activeSelf)
            {
                m_goBG.gameObject.SetActive(false);
                m_btnRightSwitch.GetComponent<Image>().SetSprite("main_btn_left_toggle");
            }
            else
            {
                m_goBG.gameObject.SetActive(true);
                m_btnRightSwitch.GetComponent<Image>().SetSprite("main_btn_right_toggle");
            }
        }
        #endregion

        private int _taskID;
        protected override void OnCreate()
        {
#if UNITY_EDITOR
            GameModule.Setting.RemoveSetting(nameof(_reqRuntimeWarnInterval));
#endif
            _reqRuntimeWarnInterval = GameModule.Setting.GetFloat(nameof(_reqRuntimeWarnInterval), _reqRuntimeWarnInterval);
            m_scrollRectRuntimeInfo.Clear(); //清空已有数据
            _taskID = GameModule.Timer.AddTimer(ReqUpdate, _reqRuntimeWarnInterval, true, true);
        }

        private async void ReqUpdate(object[] args)
        {
            if (ListChild.Count > _maxWarnCount)
            {
                return;
            }
            // Log.Info("实时报警请求");
            string json = await Utility.Http.Get(WebURL.GetFullURL("runtime_warn"));
            if (string.IsNullOrEmpty(json)) return;
            List<RspRuntimeWarnDTO> dtoList = JsonConvert.DeserializeObject<List<RspRuntimeWarnDTO>>(json);
            for (int i = 0; i < dtoList.Count; i++)
            {
                var widget = CreateWidgetByPath<RuntimeWarnItemWidget>(m_scrollRectRuntimeInfo.content,
                    nameof(RuntimeWarnItemWidget));
                widget.Refresh(dtoList[i]);
            }

            if (ListChild.Count > _maxWarnCount)
            {
                UITipWindow.Show("报警数过多", main_text: "报警数过多，已暂停在前端实时展示！");
            }
        }

        protected override void OnDestroy()
        {
            GameModule.Timer.RemoveTimer(_taskID);
            GameModule.Setting.SetFloat(nameof(_reqRuntimeWarnInterval), _reqRuntimeWarnInterval);
        }
        
        public static string GetWarnStr(int status)
        {
            switch (status)
            {
                case 1:
                    return "打电话异常";
                case 2:
                    return "安全帽佩戴异常";
                case 3:
                    return "代刷卡异常";
                case 4:
                    return "区域入侵异常";
            }
            return "非法";
        }
    }
}

