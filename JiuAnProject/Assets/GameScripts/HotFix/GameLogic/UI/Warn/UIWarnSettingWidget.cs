using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using TEngine;
using TMPro;

namespace GameLogic
{
    class UIWarnSettingWidget : UIWidget
    {
        public class WarnSettingInfo
        {
            public int count = 0; //记录的日志数
            public int intrudeMode = 0; //入侵检测对象
            public string operationInfo = ""; //操作日志
        }
        public string OutputPath =>
            Path.Combine(Application.persistentDataPath, $"{nameof(WarnSettingInfo)}.json");

        private StringBuilder sb = new StringBuilder();
        private int count = 0;
        
        #region 脚本工具生成的代码
        private TextMeshProUGUI m_textCurrent;
        private TMP_Dropdown m_dpGroup;
        private TextMeshProUGUI m_textInfo;
        protected override void ScriptGenerator()
        {
            m_textCurrent = FindChildComponent<TextMeshProUGUI>("Top/HorizontalGroup/m_textCurrent");
            m_dpGroup = FindChildComponent<TMP_Dropdown>("Top/HorizontalGroup/m_dpGroup");
            m_textInfo = FindChildComponent<TextMeshProUGUI>("Main/m_textInfo");
            m_dpGroup.onValueChanged.AddListener(OnGroupValueChanged);
        }
        #endregion

        #region 事件
        private void OnGroupValueChanged(int value)
        {
            string option = value == 0 ? "所有人" : "非注册人员";
            AppendInfo($"{DateTime.UtcNow.AddHours(8)} 设置入侵报警对象为：{option}\n");
            SendIntrudeModeRequest(m_dpGroup.value).Forget();
        }
        #endregion

        private void AppendInfo(string content)
        {
            count++;
            if (count > 10)
            {
                count = 1;
                sb.Clear();
            }
            sb.Append($"{count}.{content}");
            m_textInfo.text = sb.ToString();
        }

        private async UniTaskVoid SendIntrudeModeRequest(int mode)
        {
            await Utility.Http.Get(WebURL.GetFullURL($"intrude?mode={mode}"));
        }

        protected override void OnCreate()
        {
            m_textInfo.text = "";
            m_dpGroup.SetValueWithoutNotify(0);
            if (File.Exists(OutputPath))
            {
                string json = File.ReadAllText(OutputPath);
                WarnSettingInfo info = JsonConvert.DeserializeObject<WarnSettingInfo>(json);
                count = info.count;
                m_dpGroup.SetValueWithoutNotify(info.intrudeMode);
                m_textInfo.text = info.operationInfo;
            }
            SendIntrudeModeRequest(m_dpGroup.value).Forget();
            sb.Append(m_textInfo.text);
        }

        protected override void OnDestroy()
        {
            WarnSettingInfo info = new WarnSettingInfo();
            info.intrudeMode = m_dpGroup.value;
            info.operationInfo = m_textInfo.text;
            info.count = count;
            string json = JsonConvert.SerializeObject(info);
            File.WriteAllText(OutputPath, json);
        }
    }
}