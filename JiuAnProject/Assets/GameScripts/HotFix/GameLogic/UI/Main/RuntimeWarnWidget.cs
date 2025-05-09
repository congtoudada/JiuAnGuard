﻿/****************************************************
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
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AudioType = TEngine.AudioType;

namespace GameLogic
{
    public class RuntimeWarnWidget : UIWidget
    {
        private int _maxWarnCount = UIConstant.MaxWarnCount;
        private float _reqRuntimeWarnInterval = 1f;
        private bool _isWarnMusic = true;
        #region 脚本工具生成的代码
        private GameObject m_goBG;
        private ScrollRect m_scrollRectRuntimeInfo;
        public GameObject m_goNew;
        private Button m_btnAllKnown;
        private Button m_btnAudio;
        private Button m_btnRightSwitch;
        protected override void ScriptGenerator()
        {
            m_goBG = FindChild("m_goBG").gameObject;
            m_scrollRectRuntimeInfo = FindChildComponent<ScrollRect>("m_goBG/m_scrollRectRuntimeInfo");
            m_goNew = FindChild("m_goBG/m_goNew").gameObject;
            m_btnAllKnown = FindChildComponent<Button>("m_goBG/Horizontal/m_btnAllKnown");
            m_btnAudio = FindChildComponent<Button>("m_goBG/Horizontal/m_btnAudio");
            m_btnRightSwitch = FindChildComponent<Button>("m_btnRightSwitch");
            m_btnAllKnown.onClick.AddListener(UniTask.UnityAction(OnClickAllKnownBtn));
            m_btnAudio.onClick.AddListener(UniTask.UnityAction(OnClickAudioBtn));
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
                m_goNew.SetActive(false);
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
        private async UniTaskVoid OnClickAudioBtn()
        {
            _isWarnMusic = !_isWarnMusic;
            GameModule.Setting.SetBool(nameof(_isWarnMusic), _isWarnMusic);
            m_btnAudio.transform.FindChildComponent<TextMeshProUGUI>("Text (TMP)").text = _isWarnMusic ? "报警声:开" : "报警声:关";
        }
        #endregion

        private int _taskID;
        protected override void OnCreate()
        {
#if UNITY_EDITOR
            GameModule.Setting.RemoveSetting(nameof(_reqRuntimeWarnInterval));
#endif
            _isWarnMusic = GameModule.Setting.GetBool(nameof(_isWarnMusic), true);
            m_btnAudio.transform.FindChildComponent<TextMeshProUGUI>("Text (TMP)").text = _isWarnMusic ? "报警声:开" : "报警声:关";
            _reqRuntimeWarnInterval = GameModule.Setting.GetFloat(nameof(_reqRuntimeWarnInterval), _reqRuntimeWarnInterval);
            m_scrollRectRuntimeInfo.Clear(); //清空已有数据
            m_goNew.SetActive(false);
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
            if (string.IsNullOrEmpty(json)) return;  //不存在新报警消息则返回
            //否则更新列表并视情况播放报警声
            List<RspRuntimeWarnDTO> dtoList = JsonConvert.DeserializeObject<List<RspRuntimeWarnDTO>>(json);
            for (int i = 0; i < dtoList.Count; i++)
            {
                var widget = CreateWidgetByPath<RuntimeWarnItemWidget>(m_scrollRectRuntimeInfo.content,
                    nameof(RuntimeWarnItemWidget));
                widget.Refresh(this, dtoList[i]);
            }

            if (dtoList.Count > 0)
            {
                UISimpleTipWindow.Show("新报警: " + GetWarnStr(dtoList[^1].status));
                
                if (!m_goNew.activeSelf)
                {
                    m_goNew.SetActive(true);
                }

                if (_isWarnMusic)
                {
                    GameModule.Audio.Stop(AudioType.UISound, false);
                    GameModule.Audio.Play(AudioType.UISound, "warn_music", volume: 0.5f, bInPool: true);
                }
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

