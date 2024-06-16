/****************************************************
  文件：UIWarnRecordWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月09日 15:32:36
  功能：
*****************************************************/

using System.Collections.Generic;

namespace GameLogic
{
    public class UIWarnRecordWidget : UIRecordWidget
    {
        protected override PageType pageType => PageType.Warn;

        protected override void OnCreate()
        {
            base.OnCreate();
      
            //填充状态
            List<string> statusList = new List<string>()
            {
                "全部警告",
                "打电话",
                "未佩戴安全帽",
                "代刷卡",
                "区域入侵"
            };
            m_dpStatus.ClearOptions();
            m_dpStatus.AddOptions(statusList);
            m_dpStatus.value = 0;
        }
        
        protected override string GetStatusStr(int status)
        {
            return RuntimeWarnWidget.GetWarnStr(status);
        }
    }
}