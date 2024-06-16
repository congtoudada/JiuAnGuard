/****************************************************
  文件：GroupItemWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月09日 11:15:38
  功能：
*****************************************************/

using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    public class GroupItemWidget : UIWidget
    {
        private RspCameraInfoDTO _info;
        public RspCameraInfoDTO Info => _info;

        public bool IsOn
        {
            get => m_toggleSelect.isOn;
            set => m_toggleSelect.isOn = value;
        }
        #region 脚本工具生成的代码
        private Toggle m_toggleSelect;
        private TextMeshProUGUI m_textInfo;
        protected override void ScriptGenerator()
        {
            m_toggleSelect = FindChildComponent<Toggle>("m_toggleSelect");
            m_textInfo = FindChildComponent<TextMeshProUGUI>("m_textInfo");
            m_toggleSelect.onValueChanged.AddListener(OnToggleSelectChange);
        }
        #endregion
        
        #region 事件
        private void OnToggleSelectChange(bool isOn)
        {
                
        }

        public void Refresh(RspCameraInfoDTO info)
        {
            m_toggleSelect.isOn = false;
            _info = info;
            m_textInfo.text = $"{info.id}:{info.pos}";
        }
        #endregion
    }
}