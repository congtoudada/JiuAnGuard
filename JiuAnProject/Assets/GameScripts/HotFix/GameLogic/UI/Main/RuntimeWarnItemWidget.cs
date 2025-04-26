/****************************************************
  文件：RuntimeWarnItemWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月09日 17:18:19
  功能：
*****************************************************/

using System.Text;
using Cysharp.Threading.Tasks;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    public class RuntimeWarnItemWidget : UIWidget
    {
        private RspRuntimeWarnDTO _info;
        private RuntimeWarnWidget _outer;
        #region 脚本工具生成的代码
        private TextMeshProUGUI m_textTitle;
        private TextMeshProUGUI m_textSub;
        private Button m_btnDetail;
        private Button m_btnKnown;
        protected override void ScriptGenerator()
        {
            m_textTitle = FindChildComponent<TextMeshProUGUI>("m_textTitle");
            m_textSub = FindChildComponent<TextMeshProUGUI>("m_textSub");
            m_btnDetail = FindChildComponent<Button>("m_btnDetail");
            m_btnKnown = FindChildComponent<Button>("m_btnKnown");
            m_btnDetail.onClick.AddListener(UniTask.UnityAction(OnClickDetailBtn));
            m_btnKnown.onClick.AddListener(UniTask.UnityAction(OnClickKnownBtn));
        }
        #endregion

        public void Refresh(RuntimeWarnWidget outer, RspRuntimeWarnDTO info)
        {
            _info = info;
            _outer = outer;
            m_textTitle.text = RuntimeWarnWidget.GetWarnStr(info.status);
            string recordTime = info.recordTime;
            var recordTimeArr = recordTime.Split("_");
            recordTimeArr[1] = recordTimeArr[1].Replace("-", ":");
            recordTime = recordTimeArr[0] + " " + recordTimeArr[1];
            m_textSub.text = recordTime;
            info.recordTime = recordTime;
            if (info.warnScore > UIConstant.WarnThreshold || info.warnScore < 0.01)
            {
                transform.GetComponent<Image>().SetSprite("main_right_recordItem");
            }
            else
            {
                transform.GetComponent<Image>().SetSprite("main_right_recordItem2");
            }
        }

        #region 事件
        private async UniTaskVoid OnClickDetailBtn()
        {
            UITipWindow.Show(m_textTitle.text, _info.shotImg, sub_text: GetSubString());
        }
        
        private async UniTaskVoid OnClickKnownBtn()
        {
            Destroy();
            if (parent.ListChild.Count == 0)
            {
                _outer.m_goNew.SetActive(false);
            }
        }
        #endregion

        private static StringBuilder _builder = new StringBuilder();
        private string GetSubString()
        {
            _builder.Clear();
            _builder.Append("时间: ").Append(_info.recordTime).AppendLine();
            if (!string.IsNullOrEmpty(_info.name))
            {
                _builder.Append("可疑人员: ").Append(_info.name).AppendLine();
            }
            else
            {
                _builder.Append("可疑人员: 未知").AppendLine();
            }
            _builder.Append("抓拍点: ").Append(_info.pos).AppendLine();
            _builder.Append("报警类型: ").Append(m_textTitle.text).AppendLine();
            if (_info.warnScore > 0)
            {
                _builder.Append("报警置信度: ").Append(_info.warnScore.ToString("0.00")).AppendLine();
            }
            return _builder.ToString();
        }
    }
}