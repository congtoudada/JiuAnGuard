/****************************************************
  文件：UIFaceRecordWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:56:58
  功能：
*****************************************************/

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TEngine;
using TMPro;
using UnityEngine.UI;

namespace GameLogic
{
    public class UIFaceRecordWidget : UIRecordWidget
    {
        protected override PageType pageType => PageType.Face;

        protected override void OnCreate()
        {
            base.OnCreate();
      
            //填充状态
            List<string> statusList = new List<string>()
            {
                "全部方向",
                "进",
                "出"
            };
            m_dpStatus.ClearOptions();
            m_dpStatus.AddOptions(statusList);
            m_dpStatus.value = 0;
        }
        
        protected override string GetStatusStr(int status)
        {
            switch (status)
            {
                case 1:
                    return "进";
                case 2:
                    return "出";
            }
            return "非法";
        }
        
        protected override RecordItemWidget.RenderTypeEnum GetRenderType(RspRecordItemDTO item)
        {
            if (item.name == "陌生人") return RecordItemWidget.RenderTypeEnum.Red;
            else return RecordItemWidget.RenderTypeEnum.Green;
        }
    }
}